/*============================================================================== 
 Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.
 
 Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vuforia;

namespace VuforiaUDT
{
    public class UDTEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
    {
        /// <summary>
        /// Can be set in the Unity inspector to reference an ImageTargetBehaviour 
        /// that is instantiated for augmentations of new User-Defined Targets.
        /// </summary>
        public ImageTargetBehaviour ImageTargetTemplate;

        public int LastTargetIndex
        {
            get { return (m_TargetCounter - 1) % MAX_TARGETS; }
        }


        const int MAX_TARGETS = 5;
        UserDefinedTargetBuildingBehaviour m_TargetBuildingBehaviour;
        QualityDialog m_QualityDialog;
        ObjectTracker m_ObjectTracker;
        TrackableSettings m_TrackableSettings;
        FrameQualityMeter m_FrameQualityMeter;

        // DataSet that newly defined targets are added to
        DataSet m_UDT_DataSet;

        // Currently observed frame quality
        ImageTargetBuilder.FrameQuality m_FrameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

        // Counter used to name newly created targets
        int m_TargetCounter;


        void Start()
        {
            m_TargetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();

            if (m_TargetBuildingBehaviour)
            {
                m_TargetBuildingBehaviour.RegisterEventHandler(this);
                Debug.Log("Registering User Defined Target event handler.");
            }

            m_FrameQualityMeter = FindObjectOfType<FrameQualityMeter>();
            m_TrackableSettings = FindObjectOfType<TrackableSettings>();
            m_QualityDialog = FindObjectOfType<QualityDialog>();

            if (m_QualityDialog)
            {
                m_QualityDialog.GetComponent<CanvasGroup>().alpha = 0;
            }
        }


        /// <summary>
        /// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
        /// </summary>
        public void OnInitialized()
        {
            m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            if (m_ObjectTracker != null)
            {
                // Create a new dataset
                m_UDT_DataSet = m_ObjectTracker.CreateDataSet();
                m_ObjectTracker.ActivateDataSet(m_UDT_DataSet);
            }
        }

        /// <summary>
        /// Updates the current frame quality
        /// </summary>
        public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
        {
            Debug.Log("Frame quality changed: " + frameQuality.ToString());
            m_FrameQuality = frameQuality;
            if (m_FrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_LOW)
            {
                Debug.Log("Low camera image quality");
            }

            m_FrameQualityMeter.SetQuality(frameQuality);
        }

        /// <summary>
        /// Takes a new trackable source and adds it to the dataset
        /// This gets called automatically as soon as you 'BuildNewTarget with UserDefinedTargetBuildingBehaviour
        /// </summary>
        public void OnNewTrackableSource(TrackableSource trackableSource)
        {
            m_TargetCounter++;

            // Deactivates the dataset first
            m_ObjectTracker.DeactivateDataSet(m_UDT_DataSet);

            // Destroy the oldest target if the dataset is full or the dataset 
            // already contains five user-defined targets.
            if (m_UDT_DataSet.HasReachedTrackableLimit() || m_UDT_DataSet.GetTrackables().Count() >= MAX_TARGETS)
            {
                IEnumerable<Trackable> trackables = m_UDT_DataSet.GetTrackables();
                Trackable oldest = null;
                foreach (Trackable trackable in trackables)
                {
                    if (oldest == null || trackable.ID < oldest.ID)
                        oldest = trackable;
                }

                if (oldest != null)
                {
                    Debug.Log("Destroying oldest trackable in UDT dataset: " + oldest.Name);
                    m_UDT_DataSet.Destroy(oldest, true);
                }
            }

            // Get predefined trackable and instantiate it
            ImageTargetBehaviour imageTargetCopy = Instantiate(ImageTargetTemplate);
            imageTargetCopy.gameObject.name = "UserDefinedTarget-" + m_TargetCounter;

            // Add the duplicated trackable to the data set and activate it
            m_UDT_DataSet.CreateTrackable(trackableSource, imageTargetCopy.gameObject);

            // Activate the dataset again
            m_ObjectTracker.ActivateDataSet(m_UDT_DataSet);

            // Extended Tracking with user defined targets only works with the most recently defined target.
            // If tracking is enabled on previous target, it will not work on newly defined target.
            // Don't need to call this if you don't care about extended tracking.
            StopExtendedTracking();
            m_ObjectTracker.Stop();
            m_ObjectTracker.ResetExtendedTracking();
            m_ObjectTracker.Start();

            // Make sure TargetBuildingBehaviour keeps scanning...
            m_TargetBuildingBehaviour.StartScanning();
        }


        /// <summary>
        /// Instantiates a new user-defined target and is also responsible for dispatching callback to 
        /// IUserDefinedTargetEventHandler::OnNewTrackableSource
        /// </summary>
        public void BuildNewTarget()
        {
            if (m_FrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM ||
                m_FrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH)
            {
                // create the name of the next target.
                // the TrackableName of the original, linked ImageTargetBehaviour is extended with a continuous number to ensure unique names
                string targetName = string.Format("{0}-{1}", ImageTargetTemplate.TrackableName, m_TargetCounter);

                // generate a new target:
                m_TargetBuildingBehaviour.BuildNewTarget(targetName, ImageTargetTemplate.GetSize().x);
            }
            else
            {
                Debug.Log("Cannot build new target, due to poor camera image quality");
                if (m_QualityDialog)
                {
                    StopAllCoroutines();
                    m_QualityDialog.GetComponent<CanvasGroup>().alpha = 1;
                    StartCoroutine(FadeOutQualityDialog());
                }
            }
        }


        IEnumerator FadeOutQualityDialog()
        {
            yield return new WaitForSeconds(1f);
            CanvasGroup canvasGroup = m_QualityDialog.GetComponent<CanvasGroup>();

            for (float f = 1f; f >= 0; f -= 0.1f)
            {
                f = (float)Math.Round(f, 1);
                Debug.Log("FadeOut: " + f);
                canvasGroup.alpha = (float)Math.Round(f, 1);
                yield return null;
            }
        }

        /// <summary>
        /// This method only demonstrates how to handle extended tracking feature when you have multiple targets in the scene
        /// So, this method could be removed otherwise
        /// </summary>
        void StopExtendedTracking()
        {
            // If Extended Tracking is enabled, we first disable it for all the trackables
            // and then enable it only for the newly created target
            bool extTrackingEnabled = m_TrackableSettings && m_TrackableSettings.IsExtendedTrackingEnabled();
            if (extTrackingEnabled)
            {
                StateManager stateManager = TrackerManager.Instance.GetStateManager();

                // 1. Stop extended tracking on all the trackables
                foreach (var tb in stateManager.GetTrackableBehaviours())
                {
                    var itb = tb as ImageTargetBehaviour;
                    if (itb != null)
                    {
                        itb.ImageTarget.StopExtendedTracking();
                    }
                }

                // 2. Start Extended Tracking on the most recently added target
                List<TrackableBehaviour> trackableList = stateManager.GetTrackableBehaviours().ToList();
                ImageTargetBehaviour lastItb = trackableList[LastTargetIndex] as ImageTargetBehaviour;
                if (lastItb != null)
                {
                    if (lastItb.ImageTarget.StartExtendedTracking())
                        Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
                }
            }
        }
    }
}
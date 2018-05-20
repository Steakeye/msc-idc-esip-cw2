using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Button = UnityEngine.UI.Button;
using Vuforia;
using DYG.utils;

namespace DYG.udt
{
    using Quality = ImageTargetBuilder.FrameQuality;

    public class UDTEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
    {
        /// <summary>
        /// Can be set in the Unity inspector to reference an ImageTargetBehaviour 
        /// that is instantiated for augmentations of new User-Defined Targets.
        /// </summary>
        public ImageTargetBehaviour ImageTargetTemplate;

        public FrameQualityMeter QualityMeter;

        public int LastTargetIndex
        {
            get { return (targetCounter - 1) % MAX_TARGETS; }
        }

        private const int MAX_TARGETS = 5;
        private UserDefinedTargetBuildingBehaviour targetBuildingBehaviour;
        private QualityDialog qualityDialog;
        private ObjectTracker objectTracker;
        private TrackableSettings trackableSettings;
        private Button saveButton;
        private CameraDevice cam;
        private TrackableSource udtTS;
        private Image.PIXEL_FORMAT udtPixelFormat;
        private Image udtImage;
        private DataSetTrackableBehaviour trackableBehaviour;
        private string leftOrRightTracker;

        // DataSet that newly defined targets are added to
        DataSet uDT_DataSet;

        // Currently observed frame quality
        ImageTargetBuilder.FrameQuality frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

        // Counter used to name newly created targets
        int targetCounter;


        void Start()
        {
            targetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();

            if (targetBuildingBehaviour)
            {
                targetBuildingBehaviour.RegisterEventHandler(this);
                Debug.Log("Registering User Defined Target event handler.");
            }

            trackableSettings = FindObjectOfType<TrackableSettings>();
            qualityDialog = findQualityDialog();
            saveButton = findSaveButton();

            if (qualityDialog)
            {
                CanvasGroup qualityMsgWrapper = qualityDialog.GetComponent<CanvasGroup>();
                qualityMsgWrapper.alpha = 0;
            }
        }

        /// <summary>
        /// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
        /// </summary>
        public void OnInitialized()
        {
            Debug.Log("Calling UDTEH OnInitialized");
                
            setupCamForSnapshot();
            
            objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            setupDataset();
        }

        public void InitObjectTracker()
        {
            objectTracker = TrackerManager.Instance.InitTracker<ObjectTracker>();

            setupDataset();            
        }

        /// <summary>
        /// Updates the current frame quality
        /// </summary>
        public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
        {
            frameQuality = frameQuality;

            if (!QualityMeter.IsRetry)
            {
                QualityMeter.SetQuality(frameQuality);
            }
        }

        /// <summary>
        /// Takes a new trackable source and adds it to the dataset
        /// This gets called automatically as soon as you 'BuildNewTarget with UserDefinedTargetBuildingBehaviour
        /// </summary>
        public void OnNewTrackableSource(TrackableSource trackableSource)
        {
            udtTS = trackableSource;

            cam.Stop();
            //VuforiaRenderer.Instance.Pause(true);
            QualityMeter.ToggleToRetry();
            saveButton.gameObject.SetActive(true);
        }

        public void SetDirection(string direction)
        {
            leftOrRightTracker = direction;
        }
        
        public void Capture()
        {
            if (QualityMeter.IsRetry)
            {
                cam.Start();
                targetBuildingBehaviour.StartScanning();
                QualityMeter.ToggleToRetry(false);
                saveButton.gameObject.SetActive(false);
            }
            else
            {
                buildNewTarget();
            }

        }
        
        public void SaveTrackableSource()
        {
            if (udtTS == null)
            {
                return;
            }

            createTrackableFromSource();
            persistUDTTrackerAndSnapshot();
            
            History historyInstance = FindObjectOfType<History>();

            if (historyInstance)
            {
                historyInstance.GoBack();
            }
        }

        private void setupCamForSnapshot()
        {
            Debug.Log("calling setupCamForSnapshot");
            cam = CameraDevice.Instance;

            if (cam != null)
            {
                setUDTPixelFormat();
                //Allow the Vugforia camera to be used to take a snapshot image  
                bool camFormatSet = cam.SetFrameFormat(udtPixelFormat, true);
            }
        }

        private void setupDataset()
        {
            if (objectTracker != null)
            {
                // Create a new dataset
                uDT_DataSet = objectTracker.CreateDataSet();
                objectTracker.ActivateDataSet(uDT_DataSet);
            }
        }
        
        /// <summary>
        /// Instantiates a new user-defined target and is also responsible for dispatching callback to 
        /// IUserDefinedTargetEventHandler::OnNewTrackableSource
        /// </summary>
        private void buildNewTarget()
        {
            if (frameQuality == Quality.FRAME_QUALITY_MEDIUM ||
                frameQuality == Quality.FRAME_QUALITY_HIGH)
            {
                // create the name of the next target.
                // the TrackableName of the original, linked ImageTargetBehaviour is extended with a continuous number to ensure unique names
                string targetName = string.Format("{0}-{1}", ImageTargetTemplate.TrackableName, targetCounter);

                // generate a new target:
                targetBuildingBehaviour.BuildNewTarget(targetName, ImageTargetTemplate.GetSize().x);
               
                captureUDTSnapshot();
            }
            else
            {
                Debug.Log("Cannot build new target, due to poor camera image quality");
                if (qualityDialog)
                {
                    StopAllCoroutines();
                    qualityDialog.gameObject.SetActive(true);
                    qualityDialog.GetComponent<CanvasGroup>().alpha = 1;
                    StartCoroutine(FadeOutQualityDialog());
                }
            }
        }

        private QualityDialog findQualityDialog()
        {
            QualityDialog[] dialogs = GO.findAllElements<QualityDialog>();

            if (dialogs.Length > 0)
            {
                return dialogs[0];
            }

            return null;
        }

        private Button findSaveButton()
        {
            Button[] buttons = GO.findAllElements<Button>();
            Button saveButton;

            saveButton = buttons.FirstOrDefault((Button but) => but.name == "ButtonSave");
            
            return saveButton;
        }

        private void createTrackableFromSource()
        {
            targetCounter++;

            // Deactivates the dataset first
            objectTracker.DeactivateDataSet(uDT_DataSet);

            // Destroy the oldest target if the dataset is full or the dataset 
            // already contains five user-defined targets.
            if (uDT_DataSet.HasReachedTrackableLimit() || uDT_DataSet.GetTrackables().Count() >= MAX_TARGETS)
            {
                IEnumerable<Trackable> trackables = uDT_DataSet.GetTrackables();
                Trackable oldest = null;
                foreach (Trackable trackable in trackables)
                {
                    if (oldest == null || trackable.ID < oldest.ID)
                    {
                        oldest = trackable;
                    }
                }

                if (oldest != null)
                {
                    Debug.Log("Destroying oldest trackable in UDT dataset: " + oldest.Name);
                    uDT_DataSet.Destroy(oldest, true);
                }
            }

            // Get predefined trackable and instantiate it
            ImageTargetBehaviour imageTargetCopy = Instantiate(ImageTargetTemplate);
            imageTargetCopy.gameObject.name = "UserDefinedTarget-" + leftOrRightTracker + "-" + targetCounter;

            // Add the duplicated trackable to the data set and activate it
            trackableBehaviour = uDT_DataSet.CreateTrackable(udtTS, imageTargetCopy.gameObject);

            DontDestroyOnLoad(trackableBehaviour);
            
            // Activate the dataset again
            objectTracker.ActivateDataSet(uDT_DataSet);

            // Extended Tracking with user defined targets only works with the most recently defined target.
            // If tracking is enabled on previous target, it will not work on newly defined target.
            // Don't need to call this if you don't care about extended tracking.
            StopExtendedTracking();
            objectTracker.Stop();
            objectTracker.ResetExtendedTracking();
            objectTracker.Start();

            // Make sure TargetBuildingBehaviour keeps scanning...
            targetBuildingBehaviour.StartScanning();
        }
        
        private void setUDTPixelFormat()
        {
            #if UNITY_EDITOR
            udtPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;        //Need Grayscale for Editor
            #else
            udtPixelFormat = Image.PIXEL_FORMAT.RGB888;               //Need RGB888 for mobile
            #endif       
            
            //VuforiaARController.Instance.RegisterVuforiaStartedCallback (OnVuforiaStarted);
        }

        private void captureUDTSnapshot()
        {
            //bool camFormatSet = cam.SetFrameFormat(udtPixelFormat, true);

            udtImage = cam.GetCameraImage(udtPixelFormat);
        }

        private void persistUDTTrackerAndSnapshot()
        {
            Texture2D udtTex = new Texture2D(0, 0);
            udtImage.CopyToTexture(udtTex);

            UDTData trackerAndSnapshot = new UDTData() { Texture = udtTex, TrackableBehaviour = trackableBehaviour };

            switch (leftOrRightTracker)
            {
                case "Left":
                {
                    Data.Instance.UDTLeft = trackerAndSnapshot;
                    break;
                }
                case "Right":
                {
                    Data.Instance.UDTRight = trackerAndSnapshot;
                    break;
                }
            }
        }

        IEnumerator FadeOutQualityDialog()
        {
            yield return new WaitForSeconds(1f);
            CanvasGroup canvasGroup = qualityDialog.GetComponent<CanvasGroup>();

            for (float f = 1f; f >= 0; f -= 0.1f)
            {
                f = (float)Math.Round(f, 1);
                Debug.Log("FadeOut: " + f);
                canvasGroup.alpha = (float)Math.Round(f, 1);
                yield return null;
            }
            
            qualityDialog.gameObject.SetActive(false);
        }

        /// <summary>
        /// This method only demonstrates how to handle extended tracking feature when you have multiple targets in the scene
        /// So, this method could be removed otherwise
        /// </summary>
        void StopExtendedTracking()
        {
            // If Extended Tracking is enabled, we first disable it for all the trackables
            // and then enable it only for the newly created target
            bool extTrackingEnabled = trackableSettings && trackableSettings.IsExtendedTrackingEnabled();
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
                    {
                        Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
                    }
                }
            }
        }
    }
}
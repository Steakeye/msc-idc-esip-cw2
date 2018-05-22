using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Button = UnityEngine.UI.Button;
using Vuforia;
using DYG.utils;
using Object = UnityEngine.Object;

namespace DYG.udt
{
    using Quality = ImageTargetBuilder.FrameQuality;

    public class UDTEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
    {
        private static UDTEventHandler _instance = null;
        private static readonly Object threadSafer = new Object();
		
        private static UDTEventHandler findLocalInstanceOrSceneInstance()
        {
            return _instance ?? FindObjectOfType<UDTEventHandler>(); 
        }
		
        public static UDTEventHandler Instance
        {
            get
            {
                lock (threadSafer)
                {
                    UDTEventHandler existingInstance = findLocalInstanceOrSceneInstance();
                    // Check if the instance of this class doesn't exist either as a member or in the scene
                    if (existingInstance == null)
                    {
                        //Create anew instance if one doesn't exist
                        GameObject go = new GameObject(typeof(UDTEventHandler).ToString());
                        _instance = go.AddComponent<UDTEventHandler>();
                    }
                    else if (_instance == null)
                    {
                        _instance = existingInstance;
                    }

                    return _instance;
                }
            }
        }
	
        // Use this for one-time only initialization
        void Awake() 
        {
            lock (threadSafer)
            {
                UDTEventHandler existingInstance = findLocalInstanceOrSceneInstance();
                if (existingInstance == null)
                {
                    _instance = this;
                } else if (existingInstance != this)
                {
                    //gameObject.AddComponent(this);
                    Destroy(this);
                    _instance.startAgain();
                    _instance.Start();
                }
                else
                {
                    _instance = existingInstance;
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

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
        private bool previouslyStarted = false;
        private string imageTargetTrackableName;
        
        // DataSet that newly defined targets are added to
        DataSet uDTDataSet;
        DataSet[] uDTDataSetArr;

        // Currently observed frame quality
        ImageTargetBuilder.FrameQuality frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

        // Counter used to name newly created targets
        int targetCounter;

        void Start()
        {
            targetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>() ?? 
                                      FindObjectOfType<UserDefinedTargetBuildingBehaviour>();

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

            if (imageTargetTrackableName == null)
            {
                imageTargetTrackableName = ImageTargetTemplate.TrackableName;
            }

            previouslyStarted = true;
        }

        public void StartFromScript()
        {
            startAgain();

            if (previouslyStarted)
            {
                Start();                
            }
        }
        
        private void startAgain()
        {
            QualityMeter = FindObjectOfType<FrameQualityMeter>();
            ImageTargetTemplate = FindObjectOfType<ImageTargetBehaviour>();
        }
        
        /// <summary>
        /// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
        /// </summary>
        public void OnInitialized()
        {
            Debug.Log("Calling UDTEH OnInitialized");
                
            setupCamForSnapshot();
                        
            if (objectTracker == null)
            {
                objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
                //DontDestroyOnLoad(objectTracker1);
            }

            if (!objectTracker.IsActive)
            {
                objectTracker.Start();
            }

            setupDataset();
        }

        /*public void InitObjectTracker()
        {
            objectTracker = TrackerManager.Instance.InitTracker<ObjectTracker>();

            //setupDataset();            
        }*/

        /*public void ActivateDataSets(DataSet[] dataSets)
        {
            if (objectTracker != null)
            {
                uDTDataSetArr = dataSets;
                    
                foreach (DataSet dataSet in dataSets)
                {
                    objectTracker.ActivateDataSet(dataSet);
                }
            }
        }*/

        /// <summary>
        /// Updates the current frame quality
        /// </summary>
        public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality updatedFrameQuality)
        {
            frameQuality = updatedFrameQuality;

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

        public void UnRegisterSelf()
        {
            if (targetBuildingBehaviour)
            {
                targetBuildingBehaviour.StopTrackerWhileScanning = false;
                targetBuildingBehaviour.UnregisterEventHandler(this);
                Debug.Log("Unregistering User Defined Target event handler.");
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
                if(uDTDataSet == null)
                {
                    // Create a new dataset
                    uDTDataSet = objectTracker.CreateDataSet();
                }

                if (objectTracker.GetDataSets().Contains(uDTDataSet))
                {
                    objectTracker.ActivateDataSet(uDTDataSet);                    
                }
                
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
                string targetName = string.Format("{0}-{1}-{2}", imageTargetTrackableName, leftOrRightTracker, targetCounter);

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
            objectTracker.DeactivateDataSet(uDTDataSet);

            // Destroy the oldest target if the dataset is full or the dataset 
            // already contains five user-defined targets.
            if (uDTDataSet.HasReachedTrackableLimit() || uDTDataSet.GetTrackables().Count() >= MAX_TARGETS)
            {
                IEnumerable<Trackable> trackables = uDTDataSet.GetTrackables();
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
                    uDTDataSet.Destroy(oldest, true);
                }
            }

            // Get predefined trackable and instantiate it
            ImageTargetBehaviour imageTargetCopy = Instantiate(ImageTargetTemplate);
            imageTargetCopy.gameObject.name = "UserDefinedTarget-" + leftOrRightTracker + "-" + targetCounter;

            // Add the duplicated trackable to the data set and activate it
            trackableBehaviour = uDTDataSet.CreateTrackable(udtTS, imageTargetCopy.gameObject);

            DontDestroyOnLoad(trackableBehaviour);
            
            // Activate the dataset again
            objectTracker.ActivateDataSet(uDTDataSet);

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

            UDTData trackerAndSnapshot = new UDTData() { 
                Texture = udtTex,
                TrackableDataSet = uDTDataSet,
                TrackableBehaviour = trackableBehaviour 
            };

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
        
        /**
         * TODO: Add an OnDestroy method to clean up?
          */
        private void OnDestroy()
        {
            if (objectTracker != null)
            {
                if (objectTracker.IsActive)
                {
                    objectTracker.Stop();
                }

                if (uDTDataSet != null)
                {
                    objectTracker.DeactivateDataSet(uDTDataSet);
                }
                /*
                else if (uDTDataSetArr != null)
                {
                    foreach (DataSet dataSet in uDTDataSetArr)
                    {
                        objectTracker.DeactivateDataSet(dataSet);
                    }
                }*/
            }
        }
    }
}
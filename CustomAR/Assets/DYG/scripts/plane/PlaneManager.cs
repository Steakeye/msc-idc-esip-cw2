using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vuforia;

namespace DYG.plane
{
    public class PlaneManager : MonoBehaviour
    {
        public PlaneFinderBehaviour PlaneFinder;
        public GameObject PlaneAugmentation;
        public Text onScreenMessage;

        public Transform Floor;

        // Placement Augmentation Size Range
        [Range(0.1f, 2.0f)]
        public float ProductSize = 0.65f;

        const string TITLE_GROUNDPLANE = "Ground Plane";

        const string unsupportedDeviceTitle = "Unsupported Device";
        const string unsupportedDeviceBody =
            "This device has failed to start the Positional Device Tracker. " +
            "Please check the list of supported Ground Plane devices on our site: " +
            "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";

        const string EMULATOR_GROUND_PLANE = "Emulator Ground Plane";

        Sprite iconGroundMode;

        StateManager stateManager;
        SmartTerrain smartTerrain;
        PositionalDeviceTracker positionalDeviceTracker;

        GameObject planeAnchor;

        float placementAugmentationScale;
        int AutomaticHitTestFrameCount;
        int anchorCounter;
        Vector3 ProductScaleVector;

        GraphicRaycaster graphicRayCaster;
        PointerEventData pointerEventData;
        EventSystem eventSystem;

        Camera mainCamera;
        Ray cameraToPlaneRay;
        RaycastHit cameraToPlaneHit;


        void Start()
        {
            Debug.Log("Start() called.");

            VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
            VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
            DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
            DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

            PlaneFinder.HitTestMode = HitTestMode.AUTOMATIC;

            placementAugmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : ProductSize;
            ProductScaleVector =
                new Vector3(placementAugmentationScale,
                    placementAugmentationScale,
                    placementAugmentationScale);

            // Enable floor collider if running on device; Disable if running in PlayMode
            Floor.gameObject.SetActive(!VuforiaRuntimeUtilities.IsPlayMode());

            iconGroundMode = Resources.Load<Sprite>("icon_ground_mode");

            mainCamera = Camera.main;
            graphicRayCaster = FindObjectOfType<GraphicRaycaster>();
            eventSystem = FindObjectOfType<EventSystem>();
        }

        /*void Update()
    {

    }*/

        void LateUpdate()
        {
            if (AutomaticHitTestFrameCount == Time.frameCount)
            {
                // We got an automatic hit test this frame

                // Set visibility of the surface indicator
                SetSurfaceIndicatorVisible(true);

                onScreenMessage.transform.parent.gameObject.SetActive(true);
                onScreenMessage.enabled = true;

                onScreenMessage.text = "Tap to place game area!";
            }
            else
            {

                SetSurfaceIndicatorVisible(false);

                onScreenMessage.transform.parent.gameObject.SetActive(true);
                onScreenMessage.enabled = true;

                onScreenMessage.text = "Point device towards a flat surface";
            }
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy() called.");

            VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
            VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
            DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
            DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
        }


        public void HandleAutomaticHitTest(HitTestResult result)
        {
            //Debug.Log("HandleAutomaticHitTest() called.");

            AutomaticHitTestFrameCount = Time.frameCount;
        }

        public void HandleInteractiveHitTest(HitTestResult result)
        {
            // If the PlaneFinderBehaviour's Mode is Automatic, then the Interactive HitTestResult will be centered.

            Debug.Log("HandleInteractiveHitTest() called.");

            if (result == null)
            {
                Debug.LogError("Null hit test result");
                return;
            }

            // Place object based on Ground Plane mode
            if (positionalDeviceTracker != null && positionalDeviceTracker.IsActive)
            {
                DestroyAnchors();

                planeAnchor = positionalDeviceTracker.CreatePlaneAnchor("MyPlaneAnchor_" + (++anchorCounter), result);
                planeAnchor.name = "PlaneAnchor";
            }

            if (!PlaneAugmentation.activeInHierarchy)
            {
                Debug.Log("Setting Plane Augmentation to Active");
                // On initial run, unhide the augmentation
                PlaneAugmentation.SetActive(true);
            }

            //Clear message
            onScreenMessage.text = "";

            Debug.Log("Positioning Plane Augmentation at: " + result.Position);
            // parent the augmentation to the anchor

            PlaneAugmentation.transform.SetParent(planeAnchor.transform);
            PlaneAugmentation.transform.localPosition = Vector3.zero;
            RotateTowardCamera(PlaneAugmentation);
        }

        public void ResetScene()
        {
            Debug.Log("ResetScene() called.");
        
            // reset augmentations
            PlaneAugmentation.transform.position = Vector3.zero;
            PlaneAugmentation.transform.localEulerAngles = Vector3.zero;
            PlaneAugmentation.SetActive(false);
        }

        public void ResetTrackers()
        {
            Debug.Log("ResetTrackers() called.");

            smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
            positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

            // Stop and restart trackers
            smartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
            positionalDeviceTracker.Stop();
            positionalDeviceTracker.Start();
            smartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
        }

        void DestroyAnchors()
        {
            if (!VuforiaRuntimeUtilities.IsPlayMode())
            {
                IEnumerable<TrackableBehaviour> trackableBehaviours = stateManager.GetActiveTrackableBehaviours();

                string destroyed = "Destroying: ";

                foreach (TrackableBehaviour behaviour in trackableBehaviours)
                {
                    Debug.Log(behaviour.name +
                              "\n" + behaviour.Trackable.Name +
                              "\n" + behaviour.Trackable.ID +
                              "\n" + behaviour.GetType());

                    if (behaviour is AnchorBehaviour)
                    {
                        PlaneAugmentation.transform.parent = null;

                        if (behaviour.Trackable.Name.Contains("PlaneAnchor"))
                        {
                            destroyed +=
                                "\nGObj Name: " + behaviour.name +
                                "\nTrackable Name: " + behaviour.Trackable.Name +
                                "\nTrackable ID: " + behaviour.Trackable.ID +
                                "\nPosition: " + behaviour.transform.position.ToString();

                            stateManager.DestroyTrackableBehavioursForTrackable(behaviour.Trackable);
                            stateManager.ReassociateTrackables();
                        }
                    }
                }

                Debug.Log(destroyed);
            }
            else
            {
                PlaneAugmentation.transform.parent = null;
                DestroyObject(planeAnchor);
            }

        }

        void SetSurfaceIndicatorVisible(bool isVisible)
        {
            Renderer[] renderers = PlaneFinder.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
            Canvas[] canvas = PlaneFinder.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

            foreach (Canvas c in canvas)
            {
                c.enabled = isVisible;
            }

            foreach (Renderer r in renderers)
            {
                r.enabled = isVisible;
            }
        }

        void RotateTowardCamera(GameObject augmentation)
        {
            var lookAtPosition = mainCamera.transform.position - augmentation.transform.position;
            lookAtPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookAtPosition);
            augmentation.transform.rotation = rotation;
        }

        void OnVuforiaStarted()
        {
            Debug.Log("OnVuforiaStarted() called.");

            stateManager = TrackerManager.Instance.GetStateManager();

            // Check trackers to see if started and start if necessary
            positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
            smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

            if (positionalDeviceTracker != null && smartTerrain != null)
            {
                if (!positionalDeviceTracker.IsActive)
                {
                    positionalDeviceTracker.Start();
                }

                if (positionalDeviceTracker.IsActive && !smartTerrain.IsActive)
                {
                    smartTerrain.Start();
                }
            }
            else
            {
                if (positionalDeviceTracker == null)
                {
                    Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
                }

                if (smartTerrain == null)
                {
                    Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");
                }
            }
        }

        void OnVuforiaPaused(bool paused)
        {
            Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

            if (paused)
            {
                ResetScene();
            }
        }


        void OnTrackerStarted()
        {
            Debug.Log("OnTrackerStarted() called.");

            positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
            smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

            if (positionalDeviceTracker != null)
            {
                if (!positionalDeviceTracker.IsActive)
                    positionalDeviceTracker.Start();

                Debug.Log("PositionalDeviceTracker is Active?: " + positionalDeviceTracker.IsActive +
                          "\nSmartTerrain Tracker is Active?: " + smartTerrain.IsActive);
            }
        }

        void OnDevicePoseStatusChanged(TrackableBehaviour.Status status)
        {
            Debug.Log("OnDevicePoseStatusChanged(" + status.ToString() + ")");
        }
    }
}
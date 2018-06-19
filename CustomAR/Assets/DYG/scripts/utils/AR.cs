using UnityEngine;
using Vuforia;

namespace DYG.utils
{
    public static class AR
    {
        public static void initVuforia()
        {
            Debug.Log("Calling initVuforia");
            //VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = false;
            VuforiaRuntime.Instance.InitVuforia();
        }

        public static void disableVuforiaBehaviour()
        {
            Debug.Log("Calling disableVuforiaBehaviour");
            setVuforiaBehaviour(false);
        }
        
        public static void enableVuforiaBehaviour()
        {
            Debug.Log("Calling enableVuforiaBehaviour");
            setVuforiaBehaviour(true);
        }
        
        private static void setVuforiaBehaviour(bool enabled)
        {
            Debug.Log("Calling setVuforiaBehaviour");

            VuforiaBehaviour vbInstance = VuforiaBehaviour.Instance;

            if (vbInstance != null)
            {
                bool currentState = vbInstance.enabled;
                if (currentState != enabled)
                {
                    vbInstance.enabled = enabled;
                }
            }
        }

        public  static void initVuforiaARCam()
        {
            Debug.Log("Calling initVuforiaARCam");
            Camera mainCamera = Camera.main;
            if (mainCamera)
            {
                //GO.findAllElements<Vuforia.AR>()
                VuforiaBehaviour arBehaviour = mainCamera.GetComponent<VuforiaBehaviour>();
                
                if (arBehaviour)
                {
                    arBehaviour.enabled = true;
                }
            }
        }
    }
}
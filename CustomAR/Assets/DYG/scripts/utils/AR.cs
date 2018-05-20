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
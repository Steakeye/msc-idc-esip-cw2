using DYG.utils;
using UnityEngine;
using Vuforia;

namespace DYG.scripts.CreateButton
{
    public class CreateButton : MonoBehaviour
    {
        private void Awake()
        {
            initVuforia();
            initVuforiaARCam();
        }

        private void initVuforia()
        {
            VuforiaRuntime.Instance.InitVuforia();
        }

        private void initVuforiaARCam()
        {
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
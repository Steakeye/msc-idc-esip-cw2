using System.Collections.Generic;
using DYG.udt;
using DYG.utils;
using UnityEngine;
using Vuforia;

namespace DYG.scripts.CreateButton
{
    public class CreateButton : MonoBehaviour
    {
        private void Awake()
        {
            loadSceneArgs();
            passDirectionToUDTHandler();
            initVuforia();
            initVuforiaARCam();
        }

        public UDTEventHandler UDTEvtHandler;
        
        private void loadSceneArgs()
        {
            string parentSceneName = gameObject.scene.name;
            Dictionary<string, string> sceneArgs;
            
            if (LoadArgs.HasArgs(parentSceneName))
            {
                sceneArgs = LoadArgs.GetArgs(parentSceneName);

                if (sceneArgs != null)
                {
                    //if (sceneArgs.ContainsKey(argsDirectionKey))
                    //{
                    sceneArgs.TryGetValue(argsDirectionKey, out directionValue);
                    //}
                }
            }
        }

        private void passDirectionToUDTHandler()
        {
            UDTEvtHandler.SetDirection(directionValue);
        }
        
        private void initVuforia()
        {
            Debug.Log("Calling initVuforia");
            //VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = false;
            VuforiaRuntime.Instance.InitVuforia();
        }

        private void initVuforiaARCam()
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

        private string directionValue;
        private const string argsDirectionKey = "direction";
        private const string argsDirectionValLeft = "Left";
        private const string argsDirectionValRight = "Right";
    }
}
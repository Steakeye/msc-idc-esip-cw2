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
            AR.initVuforia();
            AR.initVuforiaARCam();
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

        private string directionValue;
        private const string argsDirectionKey = "direction";
        private const string argsDirectionValLeft = "Left";
        private const string argsDirectionValRight = "Right";
    }
}
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
        
        private void Start()
        {
            UDTEventHandler udtEH = UDTEvtHandler;

            if (udtEH == null)
            {
                UDTEvtHandler = UDTEventHandler.Instance;
                passDirectionToUDTHandler();
            }
        }

        public void CaptureUDT()
        {
            UDTEvtHandler = UDTEventHandler.Instance;
            UDTEvtHandler.Capture();
        }
        
        public void SaveUDT()
        {
            UDTEvtHandler.SaveTrackableSource();
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

        private void OnDestroy()
        {
            UDTEvtHandler.UnRegisterSelf();
        }
        
        private string directionValue;
        private const string argsDirectionKey = "direction";
        private const string argsDirectionValLeft = "Left";
        private const string argsDirectionValRight = "Right";
    }
}
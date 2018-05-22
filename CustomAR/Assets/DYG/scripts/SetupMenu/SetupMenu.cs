using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DYG.utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Vuforia;

namespace DYG
{
	public class SetupMenu : MonoBehaviour
	{
		private static GameObject ARCamGO;
		private static bool ARCamCached = false;
		
		private static CameraDevice Cam;
		
		private void Awake()
		{
			if (ARCamGO == null)
			{
				ARCamGO = Instantiate(Resources.Load("ARCam")) as GameObject; 
			}
			
			if (!ARCamCached)
			{
				preserveVuforiaBehaviour();
				ARCamCached = true;
			}
			
			if (Cam == null)
			{
				Cam = CameraDevice.Instance;
			}

			if (!Cam.IsActive())
			{
				Cam.Start();
			}

			//AR.disableVuforiaBehaviour();
			
			if (playButton == null)
			{
				playButton = findPlayButton();
			}

			TrackerManager.Instance.GetStateManager().ReassociateTrackables();
		}

		private void preserveVuforiaBehaviour()
		{
			VuforiaBehaviour vb = VuforiaBehaviour.Instance;
			
			DontDestroyOnLoad(vb);
		}
		
		public void AllDataPresent()
		{
			//Debug.Log("calling AllDataPresent");
			if (playButton != null)
			{
				playButton.gameObject.SetActive(true);
			}
		}

		public void PlayGame()
		{
			SceneManager.LoadScene(playSceneName);
		}
		
		private Button findPlayButton()
		{
			Button[] buttons = GO.findAllElements<Button>();
			Button playButton;

			playButton = buttons.FirstOrDefault((but) =>
			{
				return but.name == "PlayButton";
			});
            
			return playButton;
		}

		private const string playSceneName = "PlayGame";
		private Button playButton;
		private static bool vbCached = false;
	}
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DYG.udt;
using DYG.utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

namespace DYG
{
	public class SetupMenu : MonoBehaviour {
		private void Awake()
		{
			if (playButton == null)
			{
				playButton = findPlayButton();
			}

			//checkUDTEH();
		}

		private void checkUDTEH()
		{
			UDTEventHandler udtEH = UDTEventHandler.Instance;

			if (true)
			{
			}
		}
		
		public void AllDataPresent()
		{
			//Debug.Log("calling AllDataPresent");
			playButton.gameObject.SetActive(true);
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
	}
}
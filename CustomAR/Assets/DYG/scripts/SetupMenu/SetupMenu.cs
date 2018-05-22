﻿using System.Collections;
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
	public class SetupMenu : MonoBehaviour {
		private void Awake()
		{
			//VuforiaBehaviour.Instance.enabled = false;
			
			if (!vbCached)
			{
				preserveVuforiaBehaviour();
				vbCached = true;
			}

			//AR.disableVuforiaBehaviour();
			
			if (playButton == null)
			{
				playButton = findPlayButton();
			}
		}

		private void preserveVuforiaBehaviour()
		{
			VuforiaBehaviour vb = VuforiaBehaviour.Instance;

			vb = vb.GetComponent<VuforiaBehaviour>();
			
			DontDestroyOnLoad(vb);
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
		private bool vbCached = false;
	}
}
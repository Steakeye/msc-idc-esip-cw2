using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DYG
{
	public class SetupButtons : MonoBehaviour {

		public void goToScene(string sceneName)
		{
			Scene foundScene = SceneManager.GetSceneByName(sceneName);

			if (foundScene.IsValid())
			{
				SceneManager.LoadScene(sceneName);
			}
		}

	}
}
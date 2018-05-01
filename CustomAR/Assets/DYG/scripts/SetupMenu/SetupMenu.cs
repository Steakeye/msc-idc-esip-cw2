using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupMenu : MonoBehaviour {

	// Use this for initialization
	/*void Start () {
		
	}*/
	
	// Update is called once per frame
	/*void Update () {
		
	}*/

	public void goToScene(string sceneName)
	{
		//Scene foundScene = SceneManager.GetSceneByName(sceneName);
		
		SceneManager.LoadScene(sceneName);
		//SceneManager.LoadScene("1-About");
		
		/*if (foundScene.IsValid())
		{
			Application.LoadLevel(foundScene.buildIndex);
		}*/
	}
}

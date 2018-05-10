using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DYG
{
	public class SetupMenu : MonoBehaviour {

		// Use this for initialization
		void Awake ()
		{
		}

		void OnEnable()
		{
			Debug.Log("OnEnable called");
			// SceneManager.sceneLoaded += OnSceneLoaded;
		}
		
		void Start ()
		{
		}
	
		// Update is called once per frame
		/*void Update () {
		
		}*/
	}
}
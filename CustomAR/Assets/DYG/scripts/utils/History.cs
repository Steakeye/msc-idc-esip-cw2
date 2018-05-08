using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class History : MonoBehaviour {

	private static List<string> history = new List<string>();
	
	// Use this for initialization
	void Start ()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		history.Add("history-entry");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GoBack()
	{
		
	}
}

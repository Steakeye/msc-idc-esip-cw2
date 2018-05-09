using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class History : MonoBehaviour {

	private static Stack<string> history = new Stack<string>();
	
	// Use this for initialization
	void Start ()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;

		// Don't allow repeat entries into history
		if (history.Count == 0 || history.Peek() != currentSceneName)
		{
			history.Push(currentSceneName);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GoBack()
	{
		// Let's make sure we cna actually go back tosomething
		if (history.Count > 1) 
		{
			history.Pop();
			SceneManager.LoadScene(history.Peek());
		}
	}
}

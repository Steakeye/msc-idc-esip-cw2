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
			populateButtons();
		}
	
		void Start ()
		{
			populateButtons();
		}
	
		// Update is called once per frame
		/*void Update () {
		
		}*/

		private void populateButtons()
		{
			Button[] viewButtons = GetComponentsInChildren<Button>();
		}
	}
}
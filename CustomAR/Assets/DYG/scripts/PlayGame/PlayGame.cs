using System.Collections;
using System.Collections.Generic;
using DYG.utils;
using UnityEngine;

namespace DYG
{
	public class PlayGame : MonoBehaviour {

		// Use this for initialization
		void Awake () {
			AR.initVuforia();
			AR.initVuforiaARCam();
		}
	
		// Update is called once per frame
		void Update () {
		
		}
	}
}
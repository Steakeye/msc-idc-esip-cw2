using System;
using System.Collections;
using System.Collections.Generic;
using DYG.utils;
using UnityEngine;
using UnityEngine.UI;

namespace DYG
{
	public class PlayGame : MonoBehaviour
	{
		public TextMesh ReadyText;
		
		// Use this for initialization
		void Awake () {
			AR.initVuforia();
			AR.initVuforiaARCam();
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public void PlaneInScene() {
			if (planeNeverInScene)
			{
				planeNeverInScene = false;
				StartCoroutine(hideReadyMessage());
			}
		}

		IEnumerator hideReadyMessage()
		{
			yield return new WaitForSeconds(1f);

			float originalAlpha = ReadyText.color.a;
			
			for (; originalAlpha >= 0; originalAlpha -= 0.1f)
			{
				Color currentColor = ReadyText.color;

				originalAlpha = (float)Math.Round(originalAlpha, 1);
				currentColor.a = originalAlpha;
				Debug.Log("FadeOut: " + originalAlpha);
				ReadyText.color = currentColor;
				yield return null;
			}
            
			ReadyText.gameObject.SetActive(false);
		}

		private bool planeNeverInScene = true;
	}
}
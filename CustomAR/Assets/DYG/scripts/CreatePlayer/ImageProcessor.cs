using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DYG
{
	public class ImageProcessor : MonoBehaviour {

		public Material material;

		void Start()
		{
			RawImage img = GetComponent<RawImage>();
			img.color = Color.white;
		}
	
		public void ApplyThreshold() {
			RawImage img = GetComponent<RawImage>();

			img.material = material;
		}

		public void RemoveThreshold() {
			RawImage img = GetComponent<RawImage>();

			ResetThreshold();
	
			img.material = null;
		}

		public void ResetThreshold()
		{
			material.SetFloat("_ThresholdPoint", midPoint);
		}
	
		public void AdjustThreshold(float changeToVal) {
			RawImage img = GetComponent<RawImage>();
		
			img.material.SetFloat("_ThresholdPoint", changeToVal);
		}

		private const float midPoint = 0.5f;
	}
}
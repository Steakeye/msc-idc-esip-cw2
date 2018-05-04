using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour {

	public Material material;

	public void ApplyThreshold() {
		RawImage img = GetComponent<RawImage>();

		img.material = material;
	}

	public void RemoveThreshold() {
		Debug.Log("TODO: RemoveThreshold!");

		RawImage img = GetComponent<RawImage>();
	
		img.material = new Material(new Shader());
	}
	
	public void AdjustThreshold(float changeToVal) {
		Debug.Log("TODO: AdjustThreshold!");

		RawImage img = GetComponent<RawImage>();
		
		img.material.SetFloat("_ThresholdPoint", changeToVal);
	}

}

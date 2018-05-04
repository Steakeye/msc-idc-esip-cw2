using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	
		img.material = new Material(new Shader());
	}
	
	public void AdjustThreshold(float changeToVal) {
		RawImage img = GetComponent<RawImage>();
		
		img.material.SetFloat("_ThresholdPoint", changeToVal);
	}

}

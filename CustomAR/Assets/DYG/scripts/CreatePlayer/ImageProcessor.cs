using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour {

	public Material material;
   
	void Start() 
	{
		//
	}

	public void ApplyThreshold() {
		Debug.Log("TODO: ApplyThreshold!");

		RawImage img = GetComponent<RawImage>();

		img.material = material;
	}

	void OnGUI()
	{
		//Graphics.Blit(source, destination, material);
		//This get's called
		Debug.Log("OnGUI!");
	}

	void OnRenderObject()
	{
		//Graphics.Blit(source, destination, material);
		//This get's called
		Debug.Log("OnRenderObject!");
	}
}

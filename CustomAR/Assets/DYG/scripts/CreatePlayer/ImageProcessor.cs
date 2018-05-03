using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessor : MonoBehaviour {

	public Material material;
   
	void Start() 
	{
		if (!SystemInfo.supportsImageEffects || null == material || 
		    null == material.shader || !material.shader.isSupported)
		{
			enabled = false;
		}	
	}

	public void ApplyThreshold() {
		Debug.Log("TODO: ApplyThreshold!");
	}

	void OnBecameVisible()
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnBecameVisible!");
	}

	void OnGUI()
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnGUI!");
	}

	void OnPostRender()
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnPostRender!");
	}

	void OnPreRender()
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnPreRender!");
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnRenderImage!");
	}
	void OnRenderObject()
	{
		//Graphics.Blit(source, destination, material);
		Debug.Log("OnRenderObject!");
	}
}

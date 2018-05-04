using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;

public class CreatePlayer : MonoBehaviour
{
	public GameObject CaptureButtonGO;
	public GameObject ProcessButtonGO;
	public GameObject RawImgGO;
	public Slider ProcessSlider;
	public ImageProcessor ProcessImage;

	// Use this for initialization
	void Start ()
	{
		Debug.Log("Start called.");
		initRawImg();
		initCam();
		initCamTexture();
	}
	
	public void OnCaptureClick()
	{
		Debug.Log("On Capture Click!");

		if (captureRequested)
		{
			resetImage();
			hideProcessUI();
		}
		else
		{
			captureImage();
			showProcessButton();
		}
		
		toggleCaptureButtonText();
		captureRequested = !captureRequested;
	}
	
	public void OnProcessClick()
	{
		Debug.Log("On Process Click!");

		if (processRequested)
		{
			saveImage(); 
		}
		else
		{
			processImage();
			showProcessSlider();
		}
		toggleProcessButtonText();
		processRequested = !processRequested;
	}
	
	public void OnSliderMove()
	{
		//Debug.Log("On OnSliderMove!");

		ProcessImage.AdjustThreshold(ProcessSlider.value);
	}

	private void resetImage()
	{
		Debug.Log("Reset Image!");
		webCamTexture.Play();
	}

	private void initRawImg()
	{
		rawImg = RawImgGO.GetComponent<RawImage>();
	}

	private void initCamTexture()
	{
		rawImg.texture = webCamTexture;
	}

	private void initCam()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		webCamTexture = new WebCamTexture(deviceName, Screen.width, Screen.height, 24);
		webCamTexture.Play();		
	}

	private void captureImage()
	{
		webCamTexture.Pause();
	}
	
	private void processImage()
	{
		//Debug.Log("ProcessImage!");
		
		ProcessImage.ApplyThreshold();
		showProcessSlider(true);
	}

	private void saveImage()
	{
		int width = webCamTexture.width;
		int height = webCamTexture.height;
		Texture2D sourceTex = new Texture2D(width, height);
		Texture2D outTex = new Texture2D(width, height);
		RenderTexture renderTex = new RenderTexture(width, height, 0);
		Color[] pixels = webCamTexture.GetPixels();
		// Set webcam data to texture into the texture
		sourceTex.SetPixels(pixels);
		sourceTex.Apply();
		
		Graphics.Blit(sourceTex, renderTex, rawImg.material);

		RenderTexture.active = renderTex;
		
		outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

		//texture to PNG data
		byte[] bytes = outTex.EncodeToPNG();
		Destroy(sourceTex);
		Destroy(outTex);
		
		File.WriteAllBytes(Application.dataPath + "/test.png", bytes);
	}

	private void hideProcessUI()
	{
		showProcessButton(false);
		showProcessSlider(false);
		toggleProcessButtonText();
		
		if (processRequested)
		{
			toggleProcessButtonText();
		}
		
		processRequested = false;
	}
	
	private void toggleCaptureButtonText()
	{
		toggleButtonText(CaptureButtonGO, buttonTextCapture, buttonTextRetry, captureRequested);
	}
	
	private void toggleProcessButtonText()
	{
		toggleButtonText(ProcessButtonGO, buttonTextProcess, buttonTextSave, processRequested);
	}
	
	private void toggleButtonText(GameObject buttonGO, string textOne, string textTwo, bool toggle)
	{
		Debug.Log("Toggle Text!");

		Text txt = buttonGO.GetComponentInChildren<Text>();
		
		if (toggle)
		{
			txt.text = textOne;
		}
		else
		{
			txt.text = textTwo;
		}
	}

	private void showProcessButton(bool show = true)
	{
		ProcessButtonGO.SetActive(show);
	}
	
	private void showProcessSlider(bool show = true)
	{
		GameObject processSlider = ProcessButtonGO.GetComponentInChildren<GameObject>();
		processSlider.SetActive(show);
	}
	
	private string buttonTextCapture = "Capture";
	private string buttonTextRetry = "Retry";
	private string buttonTextProcess = "Process";
	private string buttonTextSave = "Save";
	private bool captureRequested = false;
	private bool processRequested = false;
	private WebCamTexture webCamTexture;
	private string deviceName;
	private RawImage rawImg;

}

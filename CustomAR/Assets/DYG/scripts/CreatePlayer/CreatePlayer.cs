using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CreatePlayer : MonoBehaviour
{
	public GameObject CaptureButtonGO;
	public GameObject ProcessButtonGO;
	public GameObject RawImgGO;
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
			ResetImage(); 
			showProcessButton(false);
		}
		else
		{
			captureImage();
			showProcessButton(true);
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
		}
		toggleProcessButtonText();
		processRequested = !processRequested;
	}

	public void ResetImage()
	{
		Debug.Log("Reset Image!");
		webCamTexture.Play();
	}

	private void initRawImg()
	{
		rawImg = RawImgGO.GetComponent<RawImage>();
		Renderer rdr = rawImg.GetComponent<Renderer>();
	}

	private void initCamTexture()
	{
		//rawImg.material.mainTexture = webCamTexture;
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
		Debug.Log("ProcessImage!");
		
		ProcessImage.ApplyThreshold();
	}

	private void saveImage()
	{
		Texture2D tex = new Texture2D(webCamTexture.width, webCamTexture.height);
		Color[] pixels = webCamTexture.GetPixels();
		// Set webcam data to texture into the texture
		tex.SetPixels(pixels);
		tex.Apply();
		
		//texture to PNG data
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);
		
		File.WriteAllBytes(Application.dataPath + "/test.png", bytes);
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
		//Button buttonToChange = buttonGO.GetComponent<Button>();
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

	private void showProcessButton(bool show)
	{
		ProcessButtonGO.SetActive(show);
	}
	
	private string buttonTextCapture = "Capture";
	private string buttonTextRetry = "Retry";
	private string buttonTextProcess = "Process";
	private string buttonTextSave = "Save";
	private bool captureRequested = false;
	private bool processRequested = false;
	private WebCamTexture webCamTexture;
	private string deviceName;
	//private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	private RawImage rawImg;

}

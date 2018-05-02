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
	public GameObject RawImgGO;


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

		if (buttonHasCaptured)
		{
			ResetImage(); 
		}
		else
		{
			CaptureImage();
		}
	}

	public void ResetImage()
	{
		Debug.Log("Reset Image!");
	}

	private void initRawImg()
	{
		rawImg = RawImgGO.GetComponent<RawImage>();
		Renderer rdr = rawImg.GetComponent<Renderer>();
	}

	private void initCamTexture()
	{
		rawImg.material.mainTexture = webCamTexture;
	}

	private void initCam()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		webCamTexture = new WebCamTexture(deviceName, Screen.width, Screen.height, 24);
		webCamTexture.Play();		
	}

	private void CaptureImage()
	{
		webCamTexture.Pause();

		SaveImage();
	}

	private void SaveImage()
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
	
	private void ToggleButtonText(GameObject buttonGO, string textOne, string textTwo, out bool toggle)
	{
		Button buttonToChange = buttonGO.GetComponent<Button>();
		Debug.Log("Toggle Text!");

		Text txt = buttonToChange.GetComponent<Text>();
		
		if (toggle)
		{
			txt.text = textTwo;
		}
		else
		{
			txt.text = textOne;
		}

		toggle = !toggle;
	}
	
	private string buttonTextCapture = "Capture";
	private string buttonTextRetry = "Retry";
	private bool buttonHasCaptured = false;
	private bool captureRequested = false;
	private WebCamTexture webCamTexture;
	private string deviceName;
	//private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	private RawImage rawImg;

}

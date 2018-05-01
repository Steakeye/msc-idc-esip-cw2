using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CameraCapture : MonoBehaviour
{
	public GameObject CaptureButton;

	private void Awake()
	{
		Debug.Log("Awake called.");
		Renderer rdr = GetComponent<Renderer>();
	}

	// Use this for initialization
	void Start ()
	{
		initCam();
	}

	// Update is called once per frame
	void Update () {
		if (captureRequested)
		{
			Debug.Log("We want to capture image!");
			captureRequested = false;
			//StartCoroutine(CaptureImage());
			CaptureImage();
		}
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
			//CaptureImage();
			captureRequested = true;
		}
	}
	
	public IEnumerator CaptureImageCoroutine()
	{
		Debug.Log("Capturing Image!");
		// We should only read the screen buffer after rendering is complete
		yield return frameEnd;

		CaptureImage();
	}
	
	public void ResetImage()
	{
		Debug.Log("Reset Image!");
	}

	private void initCam()
	{
		Renderer rdr = null;
		bool noError = true;	
		
		try
		{
			rdr = GetComponent<Renderer>();
		}
		catch (Exception e)
		{
			noError = false;
		}
		finally
		{

			if (noError && rdr != null)
			{
				WebCamDevice[] devices = WebCamTexture.devices;
				deviceName = devices[0].name;
				webCamTexture = new WebCamTexture(deviceName, Screen.width, Screen.width, 12);

				rdr.material.mainTexture = webCamTexture;
				webCamTexture.Play();
			}
		}
	}

	private void CaptureImage()
	{
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		//Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		Texture2D tex = new Texture2D(width, height);

		Color[] pixels = webCamTexture.GetPixels();
		// Read screen contents into the texture
		tex.SetPixels(pixels);
		tex.Apply();

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);

		File.WriteAllBytes(Application.dataPath + "/test.png", bytes);
	}
	
	private void ToogleText()
	{
		Button captureButton = CaptureButton.GetComponent<Button>();
		Debug.Log("Toogle Text!");

		Text txt = captureButton.GetComponent<Text>();
		
		if (buttonHasCaptured)
		{
			txt.text = buttonTextCapture;
		}
		else
		{
			txt.text = buttonTextRetry;
		}

		buttonHasCaptured = !buttonHasCaptured;
	}

	private static string RESOURCE_PATH = "resources/";
	
	private string buttonTextCapture = "Capture";
	private string buttonTextRetry = "Retry";
	private bool buttonHasCaptured = false;
	private bool captureRequested = false;
	private WebCamTexture webCamTexture;
	private string deviceName;
	private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

}

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CreatePlayer : MonoBehaviour
{
	public GameObject CaptureButton;
	
	// Use this for initialization
	void Start ()
	{
		initCam();
	}

	// Update is called once per frame
	/*void Update () {
		
	}*/

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
	
	public IEnumerator CaptureImage()
	{
		Debug.Log("Capturing Image!");
		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);

		File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
	}
	
	public void ResetImage()
	{
		Debug.Log("Reset Image!");
	}

	private void initCam()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		webCamTexture = new WebCamTexture(deviceName, Screen.width, Screen.width, 12);
		
		webCamTexture = new WebCamTexture();
		GetComponent<Renderer>().material.mainTexture = webCamTexture;
		webCamTexture.Play();
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
	private WebCamTexture webCamTexture;
	private string deviceName;
}

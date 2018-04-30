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
	void Start () {
		webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webCamTexture;
        webCamTexture.Play();
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
	
	public void CaptureImage()
	{
		Debug.Log("Capturing Image!");
		// NOTE - you almost certainly have to do this here:

		//yield return new WaitForEndOfFrame(); 

		// it's a rare case where the Unity doco is pretty clear,
		// http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
		// be sure to scroll down to the SECOND long example on that doco page 

		Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
		photo.SetPixels(webCamTexture.GetPixels());
		photo.Apply();

		//Encode to a PNG
		byte[] bytes = photo.EncodeToPNG();
		//Write out the PNG. 
		//Folder.CameraRoll
		File.WriteAllBytes(Application.dataPath + "/photo.png", bytes);	
	}
	
	public void ResetImage()
	{
		Debug.Log("Reset Image!");
	}
	
	protected void ToogleText()
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
}

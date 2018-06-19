using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Vuforia;
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;
using Image = Vuforia.Image;

namespace DYG
{
	using utils;
	
	public class CreatePlayer : MonoBehaviour
	{
		public GameObject CaptureButtonGO;
		public GameObject ProcessButtonGO;
		public GameObject RawImgGO;
		public RawImage FinalImage;
		public Material FlipMaterial;
		public Material CutoutMaterial;
		public Slider ProcessSlider;
		public ImageProcessor ProcessImage;
		public Data DataLayer;

		#if UNITY_EDITOR
		public Material GrayScaleFixMaterial;
		#endif
		
		private void Awake()
		{
			Debug.Log("CreatePlayer.Awake called");
			DataLayer = Data.Instance;
			//XRSettings.enabled = false;
			TrackerManager.Instance.GetStateManager().ReassociateTrackables();
			currentCam = CameraDevice.Instance;
			setUDTPixelFormat();
		}

		// Use this for initialization
		void Start ()
		{
			Debug.Log("CreatePlayer.Start called.");
			initRawImg();
		}

		private void OnDestroy()
		{

			//webCamTexture.Stop();

		}

		public void OnCaptureClick()
		{
			Debug.Log("On Capture Click!");

			if (captureRequested)
			{
				resetImage();
				resetProcessUI();
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
				resetProcessUI();
				showProcessedImage();
			}
			else
			{
				processImage();
				showProcessSlider();
				toggleProcessButtonText();
				processRequested = !processRequested;
			}
		}
	
		public void OnSliderMove()
		{
			//Debug.Log("On OnSliderMove!");

			ProcessImage.AdjustThreshold(ProcessSlider.value);
		}

		private void resetImage()
		{
			// Debug.Log("Reset Image!");
			ProcessImage.RemoveThreshold();
			
			//rawImg.color = Color.white;
			rawImg.color = Color.clear;
			//rawImg.texture = webCamTexture;				
			
			FinalImage.gameObject.SetActive(false);
			
			//webCamTexture.Play();
			currentCam.Start();
		}

		private void initRawImg()
		{
			rawImg = RawImgGO.GetComponent<RawImage>();
			rawImg.color = Color.clear;
		}

		private void stopCurrentCam()
		{
			CameraDevice currentCam = CameraDevice.Instance;

			if (currentCam.IsActive())
			{
				currentCam.Stop();
			}
		}

		private void captureImage()
		{
			currentCam.Stop();
			Image udtImage = currentCam.GetCameraImage(udtPixelFormat);

			Texture2D udtTex = new Texture2D(0, 0);
			udtImage.CopyToTexture(udtTex);
			
			resolveDimensions(udtTex);

			#if UNITY_EDITOR
			udtTex = fixGreyscaleTexture(udtTex);
			#endif
		
			rawImg.color = Color.white;
			
			rawImg.texture = flipTexture(udtTex);
		}

		private void resolveDimensions(Texture2D originalTex)
		{
			int screenW = Screen.width;
			int screenH = Screen.height;
			float screenAspectRatio = (float)screenW / (float)screenH;

			int texW = originalTex.width;
			int texH = originalTex.height;
			float texAspectRatio = (float)texW / (float)texH;

			float ratioModifier = screenAspectRatio / texAspectRatio;

			int nextXPos = 0;
			int nextYPos = 0;

			Action doCrop = () =>
			{
				Color[] croppedPixels = originalTex.GetPixels(nextXPos, nextYPos, texW, texH); 
				originalTex.Resize(texW, texH);
				originalTex.SetPixels(croppedPixels);
				originalTex.Apply();
			};
			
			if (ratioModifier < 1)
			{
				//Width needs cropping
				int oldW = texW;
				texW = (int)(texW / ratioModifier);
				nextXPos = (oldW - texW) / 2;
				doCrop();
			}
			else if (ratioModifier > 1)
			{
				//Height needs cropping
				int oldH = texH;
				texH = (int)(texH / ratioModifier);
				nextYPos = (oldH - texH) / 2;
				doCrop();
			}
		}

#if UNITY_EDITOR
		private Texture2D fixGreyscaleTexture(Texture2D originalTex)
		{
			int width = originalTex.width;
			int height = originalTex.height;

			RenderTexture renderTex = new RenderTexture(width, height, 0);
			Texture2D outTex = new Texture2D(width, height);

			GrayScaleFixMaterial.SetTexture("_MainTex", originalTex);
			
			//Apply material (and child shader) to the texture to mimic webcam effect
			Graphics.Blit(originalTex, renderTex, GrayScaleFixMaterial);
		
			//Set this texture as the active texture in order to set this data onto another texture
			RenderTexture.active = renderTex;
		
			outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
		
			outTex.Apply();

			return outTex;
		}
		#endif 

		private Texture2D flipTexture(Texture2D texToflip)
		{
			int width = texToflip.width;
			int height = texToflip.height;

			RenderTexture renderTex = new RenderTexture(width, height, 0);
			Texture2D outTex = new Texture2D(width, height);

			FlipMaterial.SetTexture("_MainTex", texToflip);
			FlipMaterial.SetInt("_FlipY", 1);
			
			//Apply material (and child shader) to the texture to mimic webcam effect
			Graphics.Blit(texToflip, renderTex, FlipMaterial);
		
			//Set this texture as the active texture in order to set this data onto another texture
			RenderTexture.active = renderTex;
		
			outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
		
			outTex.Apply();

			return outTex;
		}

		private void processImage()
		{
			//Debug.Log("ProcessImage!");
		
			ProcessImage.ApplyThreshold();
			showProcessSlider(true);
		}

		private void saveImage()
		{
			//Texture2D camTexture = getTextureFromCam(); 
			Texture2D capturedTexture = (Texture2D)rawImg.texture; 
			
			Texture2D processedTexture = makeProcessedTexture(capturedTexture);

			Texture2D alphaAddedTexture = cutoutCamTextureWithProcessed(capturedTexture, processedTexture);

			Destroy(processedTexture);
		
			Texture2D croppedTexture = createCroppedTexture(alphaAddedTexture);

			Destroy(alphaAddedTexture);
		
			DataLayer.PlayerTexture = croppedTexture;
		}

		private void showProcessedImage()
		{
			Texture2D displayTex = DataLayer.PlayerTexture;

			if (displayTex != null)
			{
				ProcessImage.RemoveThreshold();
				rawImg.color = Color.gray;
				rawImg.texture = null;

				FinalImage.texture = displayTex;
				FinalImage.rectTransform.sizeDelta = new Vector2(displayTex.width, displayTex.height);
				FinalImage.gameObject.SetActive(true);
			}
		}
		
		private Texture2D cutoutCamTextureWithProcessed(Texture2D camTex, Texture2D cutoutTex)
		{
			int width = camTex.width;
			int height = camTex.height;

			RenderTexture renderTex = new RenderTexture(width, height, 0);
			Texture2D outTex = new Texture2D(width, height);

			CutoutMaterial.SetTexture("_MainTex", camTex);
			CutoutMaterial.SetTexture("_CutoutTex", cutoutTex);
			
			//Apply material (and child shader) to the texture to mimic webcam effect
			Graphics.Blit(camTex, renderTex, CutoutMaterial);
		
			//Set this texture as the active texture in order to set this data onto another texture
			RenderTexture.active = renderTex;
		
			outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
		
			outTex.Apply();

			return outTex;
		}

		private Texture2D createCroppedTexture(Texture2D tex)
		{
			TextureExtension.Point[] cropCoords = tex.FindCropCoordinates(Color.clear);

			int cropWidth = (int) cropCoords[1].x - cropCoords[0].x;
			int cropHeight = (int) cropCoords[1].y - cropCoords[0].y;
		
			Texture2D cropTex = new Texture2D(cropWidth, cropHeight);

			Color[] croppedPixels = tex.GetPixels((int) cropCoords[0].x, (int)cropCoords[0].y, cropWidth, cropHeight);
		
			cropTex.SetPixels(croppedPixels);
		
			cropTex.Apply();

			return cropTex;
		}


		private Texture2D makeProcessedTexture(Texture2D sourceTex)
		{
			const int borderMargin = 10;

			int width = sourceTex.width;
			int height = sourceTex.height;
		
			Texture2D outTex = makeThresholdTexture(sourceTex);

			Destroy(sourceTex);
		
			//Try to cut out the image from all corners (expensive but thorough
			//Top left
			outTex.FloodFillArea(borderMargin, borderMargin, Color.clear);
			//Top right
			outTex.FloodFillArea(width - borderMargin, borderMargin, Color.clear);
			//Botton left
			outTex.FloodFillArea(borderMargin, height - borderMargin, Color.clear);
			//Botton right
			outTex.FloodFillArea(width - borderMargin, height - borderMargin, Color.clear);
		
			outTex.Apply();
		
			return outTex;
		}

		private Texture2D makeThresholdTexture(Texture2D original)
		{
			int width = original.width;
			int height = original.height;

			RenderTexture renderTex = new RenderTexture(width, height, 0);
			Texture2D outTex = new Texture2D(width, height);

			//Apply material (and child shader) to the texture to mimic webcam effect
			Graphics.Blit(original, renderTex, rawImg.material);
		
			//Set this texture as the active texture in order to set this data onto another texture
			RenderTexture.active = renderTex;
		
			outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

			outTex.Apply();
		
			return outTex;
		}
	
		private void resetProcessUI()
		{
			showProcessButton(false);
			showProcessSlider(false);
			
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
			//Debug.Log("Toggle Text!");

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
			ProcessSlider.gameObject.SetActive(show);

			if (!show)
			{
				ProcessSlider.value = thresholdStart;
			}
		}

		        
		private void setUDTPixelFormat()
		{
			#if UNITY_EDITOR
			udtPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;        //Need Grayscale for Editor
			#else
            udtPixelFormat = Image.PIXEL_FORMAT.RGB888;               //Need RGB888 for mobile
            #endif      
			
			bool camFormatSet = currentCam.SetFrameFormat(udtPixelFormat, true);
		}
		
		private Image.PIXEL_FORMAT udtPixelFormat;
		private CameraDevice currentCam;
		private string buttonTextCapture = "Capture";
		private string buttonTextRetry = "Retry";
		private string buttonTextProcess = "Process";
		private string buttonTextSave = "Save";
		private bool captureRequested = false;
		private bool processRequested = false;
		private string deviceName;
		private RawImage rawImg;
		private const float thresholdStart = 0.5f;
	}
}
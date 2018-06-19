using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

namespace DYG
{
	using utils;

	public class SetupButtons : MonoBehaviour
	{

		private const string PLAYER_BUTTON_NAME = "SetPlayerButton";
		private const string LEFT_BUTTON_NAME = "SetLeftButton";
		private const string RIGHT_BUTTON_NAME = "SetRightButton";
		private const string PLAYER_BUTTON_TEXT_MESSAGE_UPDATE = "Update Player";
		private const string LEFT_BUTTON_TEXT_MESSAGE_UPDATE = "Update Left Button";
		private const string RIGHT_BUTTON_TEXT_MESSAGE_UPDATE = "Update Right Button";
		private const string PLAYER_IMAGE_NAME = "PlayerImage";
		private const string LEFT_BUTTON_IMAGE_NAME = "LeftButtonImage";
		private const string RIGHT_BUTTON_IMAGE_NAME = "RightButtonImage";
		private const int BUTTON_IMAGE_MARGIN = 5;

		void Awake()
		{
			populateButtons();
			checkAllDataPresent();
		}

		public void GoToButtonScene(string direction)
		{
			LoadArgs.SetArgs(buttonSceneName, new Dictionary<string, string>()
			{
				{"direction", direction}
			});

			SceneManager.LoadScene(buttonSceneName);
		}

		public void GoToPlayerScene() {
			GoToScene(playerSceneName);
		}
	
		private void GoToScene(string sceneName, Dictionary<string, string> args = null)
		{
			if (args != null)
			{
				LoadArgs.SetArgs(sceneName, args);
			}
			
			SceneManager.LoadScene(sceneName);
		}

		private void populateButtons()
		{
			Button[] viewButtons = GetComponentsInChildren<Button>();

			Texture2D texture = null;
			string imageName = null, updatedTextValue = null;
			bool flipOnY = false;
			UDTData? udtData;
			Data dataCache = Data.Instance;
			
			Action<Texture2D, string, string> assignValues = (Texture2D tex, string imgName, string updatedTextVal) =>
			{
				texture = tex;
				imageName = imgName;
				updatedTextValue = updatedTextVal;
			};
			
			Action<UDTData?, string, string> assignValuesFromData = (data, imgName, updatedTextVal) =>
			{
				if (data.HasValue)
				{
					assignValues(((UDTData)data).Texture, imgName, updatedTextVal);
				}
			};

			foreach (Button viewButton in viewButtons)
			{				
				switch (viewButton.name)
				{
					case PLAYER_BUTTON_NAME:
					{
						assignValues(dataCache.PlayerTexture, PLAYER_IMAGE_NAME, PLAYER_BUTTON_TEXT_MESSAGE_UPDATE);
						flipOnY = false;
						break;
					}
					case LEFT_BUTTON_NAME:
					{
						assignValuesFromData(dataCache.UDTLeft, LEFT_BUTTON_IMAGE_NAME, LEFT_BUTTON_TEXT_MESSAGE_UPDATE);
						flipOnY = true;
						break;
					}
					case RIGHT_BUTTON_NAME:
					{
						assignValuesFromData(dataCache.UDTRight, RIGHT_BUTTON_IMAGE_NAME, RIGHT_BUTTON_TEXT_MESSAGE_UPDATE);
						flipOnY = true;
						break;
					}
				}

				if (texture != null)
				{					
					updateButton(viewButton, texture, imageName, updatedTextValue);

					if (flipOnY)
					{
						RawImage buttonImage = viewButton.GetComponentInChildren<RawImage>();

						if (buttonImage)
						{
							#if UNITY_EDITOR
							Material alphaToRGBMat = (Material)Resources.Load("AlphaToRGB", typeof(Material));
							buttonImage.material = alphaToRGBMat;
							#endif
							buttonImage.uvRect = new Rect(0, 0, 1, -1);		
						}
					}
				}
				
				texture = null;
				imageName = null;
				updatedTextValue = null;
				udtData = null;
			}
		}

		private void checkAllDataPresent()
		{
			Data dataInstance = Data.Instance;
			bool allDataPresent = (dataInstance.PlayerTexture != null &&
			                       dataInstance.UDTLeft.HasValue &&
			                       dataInstance.UDTRight.HasValue);

			if (allDataPresent)
			{
				transform.root.BroadcastMessage("AllDataPresent");
			}
		}
		
		private void updateButton(Button button, Texture2D texture, string imageName, string updatedTextValue)
		{
			if (texture != null)
			{
				populateButtonImage(button, texture, imageName);
				updateButtonText(button, updatedTextValue);
			}
		}

		private void populateButtonImage(Button button, Texture2D texture, string imageName)
		{
			RawImage buttonImage = createRawImageForButton(texture, imageName);
			
			buttonImage.gameObject.layer = button.gameObject.layer; //5

			setImageSizeAndPos(button, buttonImage);
		}

		private void setImageSizeAndPos(Button button, RawImage image)
		{
			image.transform.position = button.transform.position;

			RectTransform buttonRT = button.GetComponent<RectTransform>();
			RectTransform imageRT = image.GetComponent<RectTransform>();

			imageRT.SetParent(button.transform);

			Texture playerTex = image.texture; 
			int imgW = playerTex.width;
			int imgH = playerTex.height;
			Rect buttonRect = buttonRT.rect;
			float maxImageW = buttonRect.width - BUTTON_IMAGE_MARGIN * 2; 
			float maxImageH = (buttonRect.height - BUTTON_IMAGE_MARGIN * 2) / 2; 

			//float scaleAmount = 0f;
			
			int imgDimension = 0;
			float maxDimension = 0;
			
			AspectRatio aspectRatio = Dimensions.GetAspectRatio(imgW, imgH);

			switch (aspectRatio)
			{
				case AspectRatio.Portrait:
				case AspectRatio.Square:
				{
					//We know that we want to scale by height
					imgDimension = imgH;
					maxDimension = maxImageH;
				}
					break;
				case AspectRatio.LandScape:
				{
					//We need to detgermine whether we can scale by width
					//or still scale by height.
					// If AR of the original texture is greater than button image area then scale by width
					if (Dimensions.GetAspectRatioFloat(imgW, imgH) > Dimensions.GetAspectRatioFloat(maxImageW, maxImageH))
					{
						imgDimension = imgW;
						maxDimension = maxImageW;
					}
					else
					{
						imgDimension = imgH;
						maxDimension = maxImageH;
					}
				}
					break;
			}

			float scaleAmount = (float)imgDimension/maxDimension;
			
			int scaledImgW = (int)(imgW / scaleAmount);
			int scaledImgH = (int)(imgH / scaleAmount);
			
			imageRT.sizeDelta = new Vector2(scaledImgW, scaledImgH);
		}

		private RawImage createRawImageForButton(Texture2D texture, string name)
		{
			GameObject imageGO = new GameObject(name, typeof(RawImage));

			RawImage image = imageGO.GetComponentInChildren<RawImage>();
				
			image.texture = texture;

			return image;
		}
		
		private void updateButtonText(Button button, string textValue)
		{
			Text buttonText = button.GetComponentInChildren<Text>();

			repositionButtonText(buttonText);
			updateButtonTextMessage(buttonText, textValue);
		}
		
		private void updateButtonTextMessage(Text buttonText, string textValue)
		{
			buttonText.text = textValue;
		}
		
		private void repositionButtonText(Text buttonText)
		{
			//buttonText.alignment = TextAnchor.UpperCenter;
			Vector3 oldTextPos = buttonText.transform.position;
			float updatedTextHeight = buttonText.rectTransform.rect.height / 2;
			buttonText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, updatedTextHeight, updatedTextHeight);
			buttonText.rectTransform.anchoredPosition = new Vector2(0, -updatedTextHeight/2);
			//buttonText.transform.position = new Vector3(oldTextPos.x, updatedTextHeight, oldTextPos.z);
			//buttonText.transform.SetAsLastSibling();
		}

		private string playerSceneName = "CreatePlayer";
		private string buttonSceneName = "CreateButton";
	}
}
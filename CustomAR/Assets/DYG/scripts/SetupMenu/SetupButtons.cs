using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
		private const int PLAYER_IMAGE_MARGIN = 10;

		void Awake()
		{
			populateButtons();
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
	
		public void GoToScene(string sceneName, Dictionary<string, string> args = null)
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
			UDTData? udtData;
			
			Action<Texture2D, string, string> assignValues = (Texture2D tex, string imgName, string updatedTextVal) =>
			{
				texture = tex;
				imageName = imgName;
				updatedTextValue = updatedTextVal;
			};
			
			Action<UDTData?, string, string> assignValuesFromData = (UDTData? data, string imgName, string updatedTextVal) =>
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
						assignValues(Data.Instance.PlayerTexture, PLAYER_IMAGE_NAME, PLAYER_BUTTON_TEXT_MESSAGE_UPDATE);
						break;
					}
					case LEFT_BUTTON_NAME:
					{
						assignValuesFromData(Data.Instance.UDTLeft, LEFT_BUTTON_IMAGE_NAME, LEFT_BUTTON_TEXT_MESSAGE_UPDATE);
						break;
					}
					case RIGHT_BUTTON_NAME:
					{
						assignValuesFromData(Data.Instance.UDTRight, RIGHT_BUTTON_IMAGE_NAME, RIGHT_BUTTON_TEXT_MESSAGE_UPDATE);
						break;
					}
				}

				if (texture != null)
				{
					updateButton(viewButton, texture, imageName, updatedTextValue);
				}
				
				texture = null;
				imageName = null;
				updatedTextValue = null;
				udtData = null;
			}
		}

		private void populatePlayerButtonImage(Button playerButton)
		{
			populateButtonImage(playerButton, Data.Instance.PlayerTexture, PLAYER_IMAGE_NAME); 
			updateButtonText(playerButton, PLAYER_BUTTON_TEXT_MESSAGE_UPDATE);
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
			RawImage playerImage = createRawImageForButton(texture, imageName);
			
			playerImage.gameObject.layer = button.gameObject.layer; //5

			setImageSizeAndPos(button, playerImage);
		}

		private void setImageSizeAndPos(Button button, RawImage image)
		{
			image.transform.position = button.transform.position;

			RectTransform playerButtonRT = button.GetComponent<RectTransform>();
			RectTransform playerImageRT = image.GetComponent<RectTransform>();

			playerImageRT.SetParent(button.transform);

			Texture playerTex = image.texture; 
			int playerImgW = playerTex.width;
			int playerImgH = playerTex.height;
			Rect buttonRect = playerButtonRT.rect;
			float maxImageW = buttonRect.width - PLAYER_IMAGE_MARGIN * 2; 
			float maxImageH = (buttonRect.height - PLAYER_IMAGE_MARGIN * 2) / 2; 

			float scaleAmount = 0f;
			
			int imgDimension = 0;
			float maxDimension = 0;
			
			AspectRatio aspectRatio = Dimensions.GetAspectRatio(playerImgW, playerImgH);

			switch (aspectRatio)
			{
				case AspectRatio.Portrait:
				case AspectRatio.Square:
				{
					//We know that we want to scale by height
					imgDimension = playerImgH;
					maxDimension = maxImageH;
				}
					break;
				case AspectRatio.LandScape:
				{
					//We need to detgermine whether we can scale by width
					//or still scale by height.
					// If AR of the original texture is greater than button image area then scale by width
					if (Dimensions.GetAspectRatioFloat(playerImgW, playerImgH) > Dimensions.GetAspectRatioFloat(maxImageW, maxImageH))
					{
						imgDimension = playerImgW;
						maxDimension = maxImageW;
					}
					else
					{
						imgDimension = playerImgH;
						maxDimension = maxImageH;
					}
				}
					break;
			}

			scaleAmount = (float)imgDimension/maxDimension;
			
			int scaledImgW = (int)(playerImgW / scaleAmount);
			int scaledImgH = (int)(playerImgH / scaleAmount);
			
			playerImageRT.sizeDelta = new Vector2(scaledImgW, scaledImgH);
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
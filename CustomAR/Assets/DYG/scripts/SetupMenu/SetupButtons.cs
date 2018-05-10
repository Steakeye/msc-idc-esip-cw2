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
		private const string PLAYER_IMAGE_NAME = "PlayerImage"; 
		private const int PLAYER_IMAGE_MARGIN = 10; 
		
		void Awake ()
		{
			populateButtons();
		}
		
		public void goToScene(string sceneName)
		{
			Scene foundScene = SceneManager.GetSceneByName(sceneName);

			if (foundScene.IsValid())
			{
				SceneManager.LoadScene(sceneName);
			}
		}
		
		private void populateButtons()
		{
			Button[] viewButtons = GetComponentsInChildren<Button>();
			
			//viewButtons.Select()

			foreach (Button viewButton in viewButtons)
			{
				switch (viewButton.name)
				{
					case PLAYER_BUTTON_NAME:
					{
						populatePlayerButtonImage(viewButton);
						break;
					}
				}
			}
		}

		private void populatePlayerButtonImage(Button playerButton)
		{
			Texture2D playerYexture = Data.Instance.PlayerTexture; 
			
			if (playerYexture != null)
			{
				RawImage playerImage = createPlayerImage(playerYexture);
				
				playerImage.gameObject.layer = playerButton.gameObject.layer; //5

				setPlayerImageSizeAndPos(playerButton, playerImage);
				repositionPlayerButtonText(playerButton);
			}
		}

		private void setPlayerImageSizeAndPos(Button playerButton, RawImage playerImage)
		{
			playerImage.transform.position = playerButton.transform.position;

			RectTransform playerButtonRT = playerButton.GetComponent<RectTransform>();
			RectTransform playerImageRT = playerImage.GetComponent<RectTransform>();

			playerImageRT.SetParent(playerButton.transform);

			Texture playerTex = playerImage.texture; 
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

		private RawImage createPlayerImage(Texture2D playerYexture)
		{
			GameObject playerImageGO = new GameObject(PLAYER_IMAGE_NAME, typeof(RawImage));

			RawImage playerImage = playerImageGO.GetComponentInChildren<RawImage>();
				
			playerImage.texture = playerYexture;

			return playerImage;
		}

		private void repositionPlayerButtonText(Button playerButton)
		{
			Text buttonText = playerButton.GetComponentInChildren<Text>();

		}

	}
}
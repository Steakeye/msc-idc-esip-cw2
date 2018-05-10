using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DYG
{
	public class SetupButtons : MonoBehaviour
	{

		private const string PLAYER_BUTTON_NAME = "SetPlayerButton"; 
		private const string PLAYER_IMAGE_NAME = "PlayerImage"; 
		
		void Awake ()
		{
			populateButtons();
		}

		/*void OnEnable()
		{
			Debug.Log("OnEnable called");
			// SceneManager.sceneLoaded += OnSceneLoaded;
			populateButtons();
		}*/

		/*void Start ()
		{
			populateButtons();
		}*/
		
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
				GameObject playerImageGO = new GameObject(PLAYER_IMAGE_NAME, typeof(RawImage));

				RawImage playerImage = playerImageGO.GetComponentInChildren<RawImage>();

				//RawImage playerImage = playerButton.gameObject.AddComponent<RawImage>();
				/*RawImage playerImage = playerButton.GetComponentInChildren<RawImage>();
				RawImage playerImage2 = playerButton.gameObject.GetComponentInChildren<RawImage>();*/
				
				playerImage.texture = playerYexture;
				playerImage.transform.position = playerButton.transform.position;
				playerImageGO.SetActive(true);
				playerImageGO.layer = 5;
					
				RectTransform playerImageRT = playerImage.GetComponent<RectTransform>();

				playerImageRT.SetParent(playerButton.transform);
				playerImageRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0,10);

			}
		} 

	}
}
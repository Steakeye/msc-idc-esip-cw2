using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DYG.plane;
using DYG.udt;
using DYG.utils;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using Vuforia;

namespace DYG
{
	public class TrackableStateHandler : ITrackableEventHandler
	{
		public TrackableStateHandler(TrackableBehaviour tb, Action<string, TrackableBehaviour.Status> occlusionAction)
		{
			trackableBehaviour = tb;
		}
		
		public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
		{
			Debug.Log("OnTrackableStateChanged");
		}

		private TrackableBehaviour trackableBehaviour;
	}

	public class PlayGame : MonoBehaviour
	{
		public GameObject GamePlaneGO;
		public SpriteRenderer GamePlayerRenderer;
		public GameObject GamePlayerQuad;
		public TextMesh ReadyText;
		public PlaneManager PlaneManager;
		
		// Use this for initialization
		void Awake () {
			TrackerManager.Instance.GetStateManager().ReassociateTrackables();
			
			udtEventHandler = UDTEventHandler.Instance;
			
			/*AR.initVuforia();
			AR.initVuforiaARCam();*/
		}

		void Start()
		{
			PlaneManager.OnPlaneInScene += PlaneInScene;
			loadGameAssets();
		}
		
		// Update is called once per frame
		void Update ()
		{
			movePlayer();
		}

		public void PlaneInScene() {
			if (planeNeverInScene)
			{
				planeNeverInScene = false;
				StartCoroutine(hideReadyMessage());
				
				//loadGameAssets();
			}
		}
		
		private IEnumerator hideReadyMessage()
		{
			yield return new WaitForSeconds(1.5f);

			float originalAlpha = ReadyText.color.a;
			
			for (; originalAlpha >= 0; originalAlpha -= 0.05f)
			{
				Color currentColor = ReadyText.color;

				originalAlpha = (float)Math.Round(originalAlpha, 2);
				currentColor.a = originalAlpha;
				Debug.Log(("FadeOut: " + originalAlpha.ToString()));
				ReadyText.color = currentColor;
				yield return null;
			}
            
			ReadyText.gameObject.SetActive(false);
			
			startGame();
		}

		private void loadGameAssets()
		{
			Debug.Log("loadGameAssets called!");
			addPlayerTextureToSprite();
			setupButtons();
		}

		private void startGame()
		{
			Debug.Log("startGame called!");
		}

		private void addPlayerTextureToSprite()
		{
			Debug.Log("addPlayerTextureToSprite called!");

			//float scale = 1;
			
			Texture2D playerTex = Data.Instance.PlayerTexture;
			/*Rect rec = new Rect(0, 0, playerTex.width/scale, playerTex.height/scale);
			Sprite playerSprite = Sprite.Create(playerTex, rec, Vector2.zero, scale);*/

			Renderer quadRenderer = GamePlayerQuad.GetComponent<Renderer>();
			quadRenderer.material.mainTexture = playerTex;
			quadRenderer.material.shader = Shader.Find("Sprites/Default");
		}

		private void setupButtons()
		{
			//udtEventHandler = GetComponent<UDTEventHandler>();
			udtEventHandler = UDTEventHandler.Instance;
			
			Data data = Data.Instance;

			UDTData leftUDTData = data.UDTLeft.GetValueOrDefault();
			UDTData rightUDTData = data.UDTRight.GetValueOrDefault();

			TrackableBehaviour[] trackableBehaviours = {
				leftUDTData.TrackableBehaviour,
				rightUDTData.TrackableBehaviour
			};

			ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

			if (!objectTracker.IsActive)
			{
				objectTracker.Start();
			};
			
			if (trackableBehaviours.Any())
			{
				Action<string, TrackableBehaviour.Status> buttonAction = (trackerName, status) =>
				{
					if (trackerName.Contains("Left"))
					{
						if (status == TrackableBehaviour.Status.TRACKED || status == TrackableBehaviour.Status.NOT_FOUND)
						{
							leftButtonUp = true;
						}
						else
						{
							leftButtonUp = false;
						}
					}
					else if (trackerName.Contains("Right"))
					{
						if (status == TrackableBehaviour.Status.TRACKED || status == TrackableBehaviour.Status.NOT_FOUND)
						{
							rightButtonUp = true;
						}
						else
						{
							rightButtonUp = false;
						}
					}
				};
				//dataSetTrackableBehaviours.Where()
				IEnumerable<TrackableBehaviour> validTrackableBehaviours = trackableBehaviours.Where(ds => ds != null);
				TrackableBehaviour[] validArr = validTrackableBehaviours.ToArray();

				foreach (TrackableBehaviour tb in validArr)
				{
					tb.RegisterTrackableEventHandler(new TrackableStateHandler(tb, buttonAction)); 					
				}
			}
		}

		private void movePlayer()
		{
			if (planeNeverInScene || leftButtonUp && rightButtonUp || (!leftButtonUp && !rightButtonUp))
			{
				return;
			}

			Rect playerRect = GamePlayerQuad.GetComponent<Rect>();

			Vector2 rectPos = playerRect.position;
			
			if (!leftButtonUp)
			{
				rectPos.x += .5f;
			}
			if (!rightButtonUp)
			{
				rectPos.x -= .5f;
			}

			playerRect.position = rectPos;
		}

		private UDTEventHandler udtEventHandler;

		bool leftButtonUp = true;
		bool rightButtonUp = true;
		private bool planeNeverInScene = true;
	}
}
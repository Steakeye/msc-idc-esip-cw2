using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DYG.plane;
using DYG.udt;
using DYG.utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using Vuforia;

namespace DYG
{
	using TrackableAction = Action<string, TrackableBehaviour.Status, TrackableBehaviour.Status>;
	
	public class TrackableStateHandler : ITrackableEventHandler
	{
		public TrackableStateHandler(TrackableBehaviour tb, TrackableAction occlusionAction)
		{
			trackableBehaviour = tb;
			action = occlusionAction;
		}
		
		public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
		{
			//Debug.Log("TrackableStateHandler.OnTrackableStateChanged");
			action(trackableBehaviour.TrackableName, previousStatus, newStatus);
		}

		private TrackableBehaviour trackableBehaviour;
		private TrackableAction action;
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
				//Debug.Log(("FadeOut: " + originalAlpha));
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
				TrackableAction buttonAction = (trackerName, oldStatus, newStatus) =>
				{
					Nullable<bool> nextUpVal = null;
					
					if (newStatus == TrackableBehaviour.Status.TRACKED)
					{
						nextUpVal = true;
					}
					else if (newStatus == TrackableBehaviour.Status.NOT_FOUND && oldStatus == TrackableBehaviour.Status.TRACKED)
					{
						nextUpVal = false;
					}

					if (nextUpVal.HasValue)
					{
						bool up = nextUpVal.Value;
						if (trackerName.Contains("Left"))
						{
							leftButtonUp = up;
						}
						else if (trackerName.Contains("Right"))
						{
							rightButtonUp = up;
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

			Transform playerTransform = GamePlayerQuad.transform;

			Vector3 rectPos = playerTransform.localPosition;
			
			if (!leftButtonUp)
			{
				rectPos.x -= .05f;
			}
			if (!rightButtonUp)
			{
				rectPos.x += .05f;
			}

			playerTransform.localPosition = rectPos;
		}

		private UDTEventHandler udtEventHandler;

		bool leftButtonUp = true;
		bool rightButtonUp = true;
		private bool planeNeverInScene = true;
	}
}
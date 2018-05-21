using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DYG.plane;
using DYG.udt;
using DYG.utils;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

namespace DYG
{
	public class PlayGame : MonoBehaviour
	{
		public GameObject GamePlaneGO;
		public SpriteRenderer GamePlayerRenderer;
		public GameObject GamePlayerQuad;
		public TextMesh ReadyText;
		public PlaneManager PlaneManager;
		
		// Use this for initialization
		void Awake () {
			AR.initVuforia();
			AR.initVuforiaARCam();
		}

		void Start()
		{
			PlaneManager.OnPlaneInScene += PlaneInScene;
			loadGameAssets();
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void PlaneInScene() {
			if (planeNeverInScene)
			{
				planeNeverInScene = false;
				StartCoroutine(hideReadyMessage());
				
				//loadGameAssets();
			}
		}

		private static BindingFlags ReflectionFlags = BindingFlags.Instance
		                                   | BindingFlags.GetProperty
		                                   | BindingFlags.SetProperty
		                                   | BindingFlags.GetField
		                                   | BindingFlags.SetField
		                                   | BindingFlags.NonPublic;
		
		private IEnumerator hideReadyMessage()
		{
			yield return new WaitForSeconds(1.5f);

			float originalAlpha = ReadyText.color.a;
			
			for (; originalAlpha >= 0; originalAlpha -= 0.05f)
			{
				Color currentColor = ReadyText.color;

				originalAlpha = (float)Math.Round(originalAlpha, 2);
				currentColor.a = originalAlpha;
				Debug.Log("FadeOut: " + originalAlpha);
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
			udtEventHandler = GetComponent<UDTEventHandler>();
			udtEventHandler.InitObjectTracker();
			
			Data data = Data.Instance;

			Type trackableType = typeof(Trackable);
			FieldInfo[] fields = trackableType.GetFields(ReflectionFlags);
			PropertyInfo[] properties = trackableType.GetProperties(ReflectionFlags);
			MemberInfo[] members = trackableType.GetMembers(ReflectionFlags);
			
			FieldInfo dataSetFieldInfo = fields.FirstOrDefault(feildInfo => feildInfo.Name == "DataSet");

			//DataSetTrackableBehaviour leftUDTB = data.UDTLeft.Value.TrackableBehaviour;
			//DataSetTrackableBehaviour rightUDTB = data.UDTRight.Value.TrackableBehaviour;
			UDTData leftUDTData = data.UDTLeft.GetValueOrDefault();
			UDTData rightUDTData = data.UDTRight.GetValueOrDefault();

			DataSetTrackableBehaviour[] dataSetTrackableBehaviours = {
				leftUDTData.TrackableBehaviour,
				rightUDTData.TrackableBehaviour
			};
			
			if (dataSetTrackableBehaviours.Any())
			{
				//dataSetTrackableBehaviours.Where()
				IEnumerable<DataSetTrackableBehaviour> dataSetTrackableBehavioursEnumerable = dataSetTrackableBehaviours.Where(dstb => dstb != null);
				Trackable[] trackables = dataSetTrackableBehavioursEnumerable.Select(dstb =>
				{
					
					Trackable trackable = dstb.Trackable;

					DataSet td = (DataSet)dataSetFieldInfo.GetValue(trackable);
					
					return trackable;
				}).ToArray();
			}
		}

		private UDTEventHandler udtEventHandler;

		private bool planeNeverInScene = true;
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using DYG.plane;
using DYG.utils;
using UnityEngine;
using UnityEngine.UI;

namespace DYG
{
	public class PlayGame : MonoBehaviour
	{
		public GameObject GamePlaneGO;
		public SpriteRenderer GamePlayerRenderer;
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
		}
		// Update is called once per frame
		void Update () {
		
		}

		public void PlaneInScene() {
			if (planeNeverInScene)
			{
				planeNeverInScene = false;
				StartCoroutine(hideReadyMessage());
				
				loadGameAssets();
			}
		}

		IEnumerator hideReadyMessage()
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
		}

		private void startGame()
		{
			Debug.Log("startGame called!");
		}

		private void addPlayerTextureToSprite()
		{
			Debug.Log("addPlayerTextureToSprite called!");

			/*Texture2D playerTex = Data.Instance.PlayerTexture;
			Rect rec = new Rect(0, 0, playerTex.width, playerTex.height);
			Sprite playerSprite = Sprite.Create(playerTex, rec, Vector2.zero, 1);*/
			//Sprite playerSprite = new Sprite();

			//playerSprite.
			//playerSprite.tra
			
			//GamePlayerRenderer.sprite = playerSprite;
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			block.AddTexture("_MainTex", Data.Instance.PlayerTexture);
			GamePlayerRenderer.SetPropertyBlock(block);
		}

		private bool planeNeverInScene = true;
	}
}
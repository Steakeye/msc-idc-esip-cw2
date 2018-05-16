using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vuforia;
using Object = UnityEngine.Object;

/*
 * Inspired by http://naplandgames.com/blog/2016/11/27/saving-data-in-unity-3d-serialization-for-beginners/
 */

namespace DYG.utils
{
	public struct UDTData
	{
		public Texture2D Texture;
		public DataSetTrackableBehaviour TrackableBehaviour;
	}

	public class Data : MonoBehaviour {

		private static Data _instance = null;
		private static Object threadSafer = new Object();

		private const string PLAYER_H_KEY = "playerH";
		private const string PLAYER_W_KEY = "playerW";
		
		private static Data findLocalInstanceOrSceneInstance()
		{
			return _instance ?? FindObjectOfType<Data>(); 
		}
		
		public static Data Instance
		{
			get
			{
				lock (threadSafer)
				{
					// Check if the instance of this class doesn't exist either as a member or in the scene
					if (findLocalInstanceOrSceneInstance() == null)
					{
						//Create anew instance if one doesn't exist
						GameObject go = new GameObject(typeof(Data).ToString());
						_instance = go.AddComponent<Data>();
					}

					return _instance;
				}
			}
		}
	
		// Use this for one-time only initialization
		void Awake() 
		{
			lock (threadSafer)
			{
				if (playerTexPath == null)
				{
					playerTexPath = Application.persistentDataPath + "/player.png";
				}

				Data existingInstance = findLocalInstanceOrSceneInstance();
				if (existingInstance == null)
				{
					_instance = this;
				} else if (existingInstance != this)
				{
					//gameObject.AddComponent(this);
					Destroy(this);
				}
				else
				{
					_instance = existingInstance;
					DontDestroyOnLoad(gameObject);

					loadSavedData();
				}
			}
		}
	
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public Texture2D PlayerTexture
		{
			get { return playerTex; }
			set
			{
				if (playerTex == null || playerTex.imageContentsHash != value.imageContentsHash)
				{
					playerTexUpdated = true;
				}
				playerTex = value; 
				
			}
		}

		public UDTData? UDTLeft
		{
			get { return udtLeft; }
			set
			{
				if (udtLeft == null || (udtLeft != null && value == null) || ((UDTData)udtLeft).Texture.imageContentsHash != ((UDTData)value).Texture.imageContentsHash)
				{
					udtLeftTexUpdated = true;
				}
				udtLeft = value; 
				
			}
		}
		
		public UDTData? UDTRight
		{
			get { return udtRight; }
			set
			{
				if (udtRight == null || (udtRight != null && value == null) || ((UDTData)udtLeft).Texture.imageContentsHash != ((UDTData)value).Texture.imageContentsHash)
				{
					udtRightTexUpdated = true;
				}
				udtRight = value; 
				
			}
		}

		public void SaveData()
		{
			savePlayerIfPresent();
		}

		private void loadSavedData()
		{
			loadPlayerIfPresent();
		}
	
		private void loadPlayerIfPresent()
		{
			if (File.Exists(playerTexPath))
			{
				byte[] bytes = File.ReadAllBytes(playerTexPath);
				
				PlayerTexture = new Texture2D(0, 0);
				
				PlayerTexture.LoadImage(bytes);
			}
		}
	
		private void savePlayerIfPresent()
		{
			if (playerTex != null && playerTexUpdated)
			{
				byte[] bytes = playerTex.EncodeToPNG();

				File.WriteAllBytes(playerTexPath, bytes);
			}
		}

		private Texture2D playerTex = null;
		private UDTData? udtLeft;
		private UDTData? udtRight;
		private bool playerTexUpdated = false;
		private bool udtLeftTexUpdated = false;
		private bool udtRightTexUpdated = false;
		private string playerTexPath = null;
	}
}
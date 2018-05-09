using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * Inspired by http://naplandgames.com/blog/2016/11/27/saving-data-in-unity-3d-serialization-for-beginners/
 */

namespace DYG
{
	public class Data : MonoBehaviour {

		private static Data _instance = null;

		private const string PLAYER_H_KEY = "playerH";
		private const string PLAYER_W_KEY = "playerW";
		
		public static Data Instance
		{
			get
			{
				// If the instance of this class doesn't exist
				if (_instance == null)
				{
					// Check the scene for a Game Object with this class
					_instance = FindObjectOfType<Data>();

					// If none is found in the scene then create a new Game Object
					// and add this class to it.
					if (_instance == null)
					{
						GameObject go = new GameObject(typeof(Data).ToString());
						_instance = go.AddComponent<Data>();
					}
				}

				return _instance;
			}
		}
	
		// Use this for one-time only initialization
		void Awake() {
			if (Instance != this)
			{
				Destroy(this);
			}
			else
			{
				DontDestroyOnLoad(gameObject);

				loadSavedData();
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
			get; set;
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
			//PlayerPrefs.
			if (File.Exists(playerTexPath))
			{
				byte[] bytes = File.ReadAllBytes(playerTexPath);
				
				int texW = PlayerPrefs.GetInt(PLAYER_W_KEY);
				int texH = PlayerPrefs.GetInt(PLAYER_H_KEY);
				
				//PlayerTexture = new Texture2D(texW, texH);
				PlayerTexture = new Texture2D(0, 0);
				
				PlayerTexture.LoadImage(bytes);
			}
		}
	
		private void savePlayerIfPresent()
		{
			if (PlayerTexture != null)
			{
				PlayerPrefs.SetInt(PLAYER_W_KEY, PlayerTexture.width);
				PlayerPrefs.SetInt(PLAYER_H_KEY, PlayerTexture.height);

				byte[] bytes = PlayerTexture.EncodeToPNG();

				File.WriteAllBytes(playerTexPath, bytes);
			}
		}

		//private Texture2D playerTex = null;
		private string playerTexPath = Application.persistentDataPath + "/player.png";
}
}
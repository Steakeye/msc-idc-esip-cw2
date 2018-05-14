using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DYG.utils
{
	using ArgsHash = Dictionary<string, string>;

	public static class LoadArgs {

		private static Dictionary<string, ArgsHash> sceneArgsHash = new Dictionary<string, ArgsHash>();
	
		public static void setArgs(string sceneName, ArgsHash args)
		{
			sceneArgsHash.Add(sceneName, args);
		}

		public static ArgsHash getArgs(string sceneName)
		{
			//sceneArgsHash.
			ArgsHash sceneArgs = null;
			
			//bool gotArgs = sceneArgsHash.TryGetValue(sceneName, out sceneArgs);
			bool gotArgs = sceneArgsHash.TryGetValue(sceneName, out sceneArgs);

			// Remove them so they're not used again accidentally
			if (gotArgs)
			{
				sceneArgsHash.Remove(sceneName);
			}

			return sceneArgs;
		}
	}
}
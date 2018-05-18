using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DYG.utils
{
	public static class GO {
		public static T[] findAllElements<T>() where T : Object {
			return Resources.FindObjectsOfTypeAll<T>();
		}
	}
}
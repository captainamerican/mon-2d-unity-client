using System.Collections;
using UnityEngine;

namespace World {

	public class Forest_01_Entrance_Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		IEnumerator Start() {
			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}
	}
}
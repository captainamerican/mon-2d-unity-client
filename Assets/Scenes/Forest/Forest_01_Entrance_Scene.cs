using System.Collections;

using UnityEngine;

namespace World {

	public class Forest_01_Entrance_Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		IEnumerator Start() {
			Player.transform.position = Engine.NextScenePosition;
			Engine.NextScenePosition = Vector3.zero;

			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}
	}
}
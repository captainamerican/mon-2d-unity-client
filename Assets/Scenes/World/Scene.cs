using System.Collections;

using UnityEngine;

namespace World {

	public class Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		IEnumerator Start() {
			NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
				Engine.NextScene = null;
			}

			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}
	}
}
using System.Collections;

using UnityEngine;

namespace Village {
	public class Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		IEnumerator Start() {
			NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
			}

			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}

		public void OpenPotionShop() {
			Debug.Log("Open Potion Shop");
		}

		public void OpenTrainer() {
			Debug.Log("Open Trainer");
		}

		public void OpenStorage() {
			Debug.Log("Open Storage");
		}
	}
}
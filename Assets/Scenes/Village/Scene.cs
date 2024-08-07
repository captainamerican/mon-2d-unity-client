using System.Collections;

using UnityEngine;

namespace Village {
	public class Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		GameObject Player;

		public const string Name = "Village";

		static public Vector3 Location_Main {
			get {
				return new Vector3(0, 0, 0);
			}
		}

		static public Vector3 Location_Tree {
			get {
				return new Vector3(0, 46, 0);
			}
		}

		IEnumerator Start() {
			NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
				Engine.NextScene = null;
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
using System;
using System.Collections;

using UnityEngine;

namespace Village {
	public class Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		Player Player;

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
			Engine.Mode = EngineMode.Menu;
			Player.Stop();

			StartCoroutine(
				Crafting.Scene.Load(
					() => StartCoroutine(
						ReturnFromStore(Crafting.Scene.Unload)
					)
				)
			);
		}

		public void OpenTrainer() {
			Debug.Log("Open Trainer");
		}

		public void OpenStorage() {
			Debug.Log("Open Storage");
		}

		IEnumerator ReturnFromStore(Func<IEnumerator> callback) {
			yield return callback();
			Engine.Mode = EngineMode.PlayerControl;
		}
	}
}
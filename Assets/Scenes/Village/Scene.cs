using System;
using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Village {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		public const string Name = "Village";

		// -------------------------------------------------------------------------

		[SerializeField] Engine Engine;
		[SerializeField] Player Player;

		// -------------------------------------------------------------------------

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

		// -------------------------------------------------------------------------

		IEnumerator Start() {
			Game.NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
			}
			Engine.NextScene = null;

			//
			Engine.Profile.MapId = Game.MapId.Village;
			Engine.Profile.CurrentLocation = Player.transform.position;

			//
			yield return Dialogue.Scene.Load();
			yield return Menu.Scene.Load();
			yield return Loader.Scene.Clear();
			Engine.SetMode(EngineMode.PlayerControl);
		}

		// -------------------------------------------------------------------------

		public void OpenPotionShop() {
			Engine.Mode = EngineMode.Store;
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
			Engine.Mode = EngineMode.Store;
			Player.Stop();

			StartCoroutine(
				Trainer.Scene.Load(
					() => StartCoroutine(
						ReturnFromStore(Trainer.Scene.Unload)
					)
				)
			);
		}

		public void OpenStorage() {
			Engine.Mode = EngineMode.Store;
			Player.Stop();

			StartCoroutine(
				CreatureManager.Scene.Load(
					() => StartCoroutine(
						ReturnFromStore(CreatureManager.Scene.Unload)
					)
				)
			);
		}

		// -------------------------------------------------------------------------

		IEnumerator ReturnFromStore(Func<IEnumerator> callback) {
			yield return callback();

			//
			Engine.Mode = EngineMode.PlayerControl;
		}

		// -------------------------------------------------------------------------

	}
}
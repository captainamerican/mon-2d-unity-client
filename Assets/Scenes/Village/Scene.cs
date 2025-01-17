using System;
using System.Collections;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Village {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		public const string Name = "Village";

		// -------------------------------------------------------------------------

		public static Vector3 Location_Main {
			get {
				return new Vector3(0, 0, 0);
			}
		}

		public static Vector3 Location_Tree {
			get {
				return new Vector3(0, 46, 0);
			}
		}

		// -------------------------------------------------------------------------

		[SerializeField] Engine Engine;
		[SerializeField] Player Player;

		// -------------------------------------------------------------------------

		IEnumerator Start() {
			Game.NextScene nextScene = Engine.NextScene;
			if (nextScene != null) {
				Player.transform.position = nextScene.Destination;
				Player.SetFacing(nextScene.PlayerDirection);
				Player.Stop();
			}
			Engine.NextScene = null;

			//
			Engine.Profile.SceneName = Name;
			Engine.Profile.MapId = Game.MapId.Village;
			Engine.Profile.CurrentLocation = Player.transform.position;

			//
			Engine.Profile.Magic = Engine.Profile.MagicTotal;
			Engine.Profile.Creatures.ForEach(creature => creature.Heal());

			//
			if (
				Engine.Profile.StoryPoints.Has(Game.StoryPointId.ToldAboutPocketTeleporter) &&
				!Engine.Profile.StoryPoints.Has(Game.StoryPointId.UsedPocketTeleporter)
			) {
				Engine.Profile.StoryPoints.Add(Game.StoryPointId.UsedPocketTeleporter);
			}

			//
			yield return Dialogue.Scene.Load();
			yield return Menu.Scene.Load();

			if (nextScene.ExecuteOnLoad != null) {
				GameObject go = GameObject.Find($"/Cutscenes/{nextScene.ExecuteOnLoad}");
				Cutscene cutscene = go.GetComponent<Cutscene>();

				_ = StartCoroutine(cutscene.Playing());
			} else {
				yield return Loader.Scene.Clear();
				Engine.Mode = EngineMode.PlayerControl;
			}
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
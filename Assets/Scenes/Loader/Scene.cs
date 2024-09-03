using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Loader {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "Loader";
		static Scene Self;

		static public void Load(Game.NextScene nextScene) {
			Database.Engine.NextScene = nextScene;
			SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------

		[SerializeField] Engine Engine;
		[SerializeField] CanvasGroup CanvasGroup;

		// -------------------------------------------------------------------------

		void Awake() {
			Self = this;
		}

		IEnumerator Start() {
			yield return Do.ForReal(0.25f, ratio => CanvasGroup.alpha = Mathf.Lerp(0, 1, ratio));
			yield return Dialogue.Scene.Unload();
			yield return Menu.Scene.Unload();

			//
			List<UnityEngine.SceneManagement.Scene> scenesToRemove = new();

			for (int i = 0; i < SceneManager.sceneCount; i++) {
				UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
				if (scene.name == Name) {
					continue;
				}

				scenesToRemove.Add(scene);
			}

			//
			for (int i = 0; i < scenesToRemove.Count; i++) {
				UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
				AsyncOperation removeScene = SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				yield return Wait.Until(() => removeScene.isDone);
			}

			// just in case
			Engine.Mode = EngineMode.None;
			Time.timeScale = 1;

			if (Engine.NextScene?.SaveFile != null) {
				Engine.Profile = Engine.NextScene.SaveFile;
			}

			//
			AsyncOperation clearResources = Resources.UnloadUnusedAssets();
			yield return Wait.Until(() => clearResources.isDone);
			yield return Wait.ForReal(0.25f);

			//
			SceneManager.LoadSceneAsync(Engine?.NextScene?.Name ?? StartScreen.Scene.Name, LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------

		static public IEnumerator Clear() {
			if (Self != null) {
				yield return Self.RemoveSelf();
			}
		}

		protected IEnumerator RemoveSelf() {
			if (SceneManager.GetSceneByName(Name).name != null) {
				yield return Do.For(0.25f, ratio => CanvasGroup.alpha = Mathf.Lerp(1, 0, ratio));

				AsyncOperation removeSelf = SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				yield return Wait.Until(() => {
					return removeSelf?.isDone ?? true;
				});
			}
		}

		// -------------------------------------------------------------------------

	}
}
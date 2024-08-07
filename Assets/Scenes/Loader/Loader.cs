using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader {

	public class Scene : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		CanvasGroup CanvasGroup;

		static Scene Self;

		void Awake() {
			Self = this;
		}

		IEnumerator Start() {
			yield return Do.For(0.25f, ratio => CanvasGroup.alpha = Mathf.Lerp(0, 1, ratio));

			//
			List<UnityEngine.SceneManagement.Scene> scenesToRemove = new();

			for (int i = 0; i < SceneManager.sceneCount; i++) {
				UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
				if (scene.name == "Loader") {
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

			//
			AsyncOperation clearResources = Resources.UnloadUnusedAssets();
			yield return Wait.Until(() => clearResources.isDone);
			yield return Wait.For(0.25f);

			SceneManager.LoadSceneAsync(Engine.NextScene.Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Clear() {
			if (Self != null) {
				yield return Self.RemoveSelf();
			}
		}

		protected IEnumerator RemoveSelf() {
			if (SceneManager.GetSceneByName("Loader").name != null) {
				yield return Do.For(0.25f, ratio => CanvasGroup.alpha = Mathf.Lerp(1, 0, ratio));

				AsyncOperation removeSelf = SceneManager.UnloadSceneAsync("Loader", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				yield return Wait.Until(() => {
					return removeSelf?.isDone ?? true;
				});
			}
		}
	}
}
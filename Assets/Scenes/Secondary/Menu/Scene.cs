using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Menu {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "Dialogue";

		static Scene Self;

		static public IEnumerator Load() {
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			if (Self != null) {
				yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			}
		}

		static public IEnumerator Display() {
			Debug.Assert(Self != null, "Menu scene wasn't loaded!");

			//
			return Self.Show();
		}

		// -------------------------------------------------------------------------

		[SerializeField] Canvas Canvas;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] CompendiumMenu CompendiumMenu;

		// -------------------------------------------------------------------------

		void Awake() {
			Self = this;
		}

		IEnumerator Start() {
			InitialMenu.gameObject.SetActive(false);
			CompendiumMenu.gameObject.SetActive(false);

			//
			yield return Wait.ForReal(0.5f);
			yield return Show();
			Debug.Log("Done");
		}

		// -------------------------------------------------------------------------

		public IEnumerator Show() {
			Canvas.gameObject.SetActive(true);

			InitialMenu.gameObject.SetActive(true);
			InitialMenu.Configure(() => Canvas.gameObject.SetActive(false));

			//
			return Wait.Until(() => !Canvas.isActiveAndEnabled);
		}

		// -------------------------------------------------------------------------

	}
}
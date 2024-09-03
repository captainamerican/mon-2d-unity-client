using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace StartScreen {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "StartScreen";

		// -------------------------------------------------------------------------

		[SerializeField] Engine Engine;
		[SerializeField] Button StartButton;

		// -------------------------------------------------------------------------

		IEnumerator Start() {
			Engine.NextScene = null;
			yield return Loader.Scene.Clear();

			Game.Focus.This(StartButton);
		}


		// -------------------------------------------------------------------------

		public void StartGame() {
			Engine.NextScene = new Game.NextScene {
				Name = Village.Scene.Name,
				Destination = Village.Scene.Location_Main
			};

			//
			SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------
	}
}
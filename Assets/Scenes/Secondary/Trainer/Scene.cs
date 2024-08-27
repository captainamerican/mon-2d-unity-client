using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "Trainer";

		static Action OnDone = () => Debug.Log("Close Menu");

		// -------------------------------------------------------------------------

		[SerializeField] List<GameObject> MenusToDisableOnLoad;

		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		static public IEnumerator Load(Action onDone) {
			OnDone = onDone;

			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}

		// ------------------------------------------------------------------------- 

		void Start() {
			MenusToDisableOnLoad.ForEach(menu => menu.SetActive(false));

			InitialMenu.gameObject.SetActive(true);
			InitialMenu.Configure(OnDone);
		}

		// -------------------------------------------------------------------------

	}
}
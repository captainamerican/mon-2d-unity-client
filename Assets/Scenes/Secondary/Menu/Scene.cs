using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------

namespace Menu {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static public string Name = "Menu";

		static Scene Self;

		static public IEnumerator Load() {
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			if (Self != null) {
				yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			}

			yield break;
		}

		// -------------------------------------------------------------------------

		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] Canvas Canvas;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] CompendiumMenu CompendiumMenu;
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] InventoryMenu InventoryMenu;
		[SerializeField] OptionsMenu OptionsMenu;
		[SerializeField] StatusMenu StatusMenu;
		[SerializeField] WorldMapMenu WorldMapMenu;
		[SerializeField] SaveLoadMenu SaveLoadMenu;

		[Header("World Map")]
		[SerializeField] GameObject WorldMapContent;

		// -------------------------------------------------------------------------

		InputAction Menu;

		// -------------------------------------------------------------------------

		void Awake() {
			Self = this;

			//
			OnDestroy();

			//
			Menu = PlayerInput.currentActionMap.FindAction("Menu");
			Menu.performed += OpenMenu;

			//
			InitialMenu.gameObject.SetActive(false);
			CompendiumMenu.gameObject.SetActive(false);
			CreaturesMenu.gameObject.SetActive(false);
			InventoryMenu.gameObject.SetActive(false);
			OptionsMenu.gameObject.SetActive(false);
			StatusMenu.gameObject.SetActive(false);
			WorldMapMenu.gameObject.SetActive(false);
			SaveLoadMenu.gameObject.SetActive(false);

			WorldMapContent.SetActive(false);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			if (Menu != null) {
				Menu.performed -= OpenMenu;
			}
		}

		// -------------------------------------------------------------------------

		IEnumerator Show() {
			Canvas.gameObject.SetActive(true);

			InitialMenu.gameObject.SetActive(true);
			InitialMenu.Configure(() => Canvas.gameObject.SetActive(false));

			//
			return Wait.Until(() => !Canvas.isActiveAndEnabled);
		}

		void OpenMenu(InputAction.CallbackContext _) {
			if (!Engine.PlayerHasControl()) {
				return;
			}

			//
			StartCoroutine(ShowingMenu());
		}

		IEnumerator ShowingMenu() {
			Engine.Mode = EngineMode.Menu;
			Time.timeScale = 0;
			yield return Show();
			Engine.Mode = EngineMode.PlayerControl;
			Time.timeScale = 1;
		}

		// -------------------------------------------------------------------------

	}
}
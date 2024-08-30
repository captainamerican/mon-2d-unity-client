using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------

namespace Menu {
	public class WorldMapMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject WorldMapContent;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		// --------------------------------------------------------------------------

		void OnEnable() {
			RemoveInputCallbacks();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;

			WorldMapContent.SetActive(true);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			InitialMenu.gameObject.SetActive(true);

			//
			WorldMapContent.SetActive(false);
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------  

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		// -------------------------------------------------------------------------
	}
}
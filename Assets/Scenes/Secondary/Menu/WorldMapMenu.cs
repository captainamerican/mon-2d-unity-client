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

		float cameraSize;
		Vector3 cameraPosition;

		// --------------------------------------------------------------------------

		void OnEnable() {
			RemoveInputCallbacks();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;

			WorldMapContent.SetActive(true);

			if (Camera.main.transform.parent.CompareTag("Player")) {
				cameraSize = Camera.main.orthographicSize;
				Camera.main.orthographicSize = 32;

				cameraPosition = Camera.main.transform.parent.position;
				Camera.main.transform.parent.position = Vector3.zero;
			}
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			if (Camera.main.transform.parent.CompareTag("Player")) {
				Camera.main.transform.parent.position = cameraPosition;
				Camera.main.orthographicSize = cameraSize;
			}

			//
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
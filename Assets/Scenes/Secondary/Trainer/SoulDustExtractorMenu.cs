using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class SoulDustExtractorMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		public enum Phase {
			Normal,
			SubModal
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Quantity Modal")]
		[SerializeField] GameObject QuantityModal;
		[SerializeField] TextMeshProUGUI MinLabel;
		[SerializeField] TextMeshProUGUI MaxLabel;
		[SerializeField] RectTransform QuantityTransform;
		[SerializeField] RectTransform QuantityThumb;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		Phase phase;
		int selectedButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			selectedButtonIndex = 0;
			phase = Phase.Normal;

			//
			QuantityModal.SetActive(false);

			//
			ConfigureButtons();
			ConfigureCancel();
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			switch (phase) {
				case Phase.Normal:
					GoBack();
					break;
			}
		}

		void GoBack() {
			InitialMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		// -----------------------------------------------------------------------

		void ConfigureCancel() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureButtons() {
		}

		// -----------------------------------------------------------------------

	}
}
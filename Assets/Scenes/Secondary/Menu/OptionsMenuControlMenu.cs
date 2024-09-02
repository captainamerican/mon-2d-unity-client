using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class OptionsMenuControlMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject Dialog;
		[SerializeField] Button ConfirmButton;

		[Header("Exit")]
		[SerializeField] OptionsMenu OptionsMenu;
		[SerializeField] Button EditControlsButton;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		// -------------------------------------------------------------------------

		void Start() {
			Dialog.SetActive(false);
		}

		// -------------------------------------------------------------------------

		public void Show() {
			OptionsMenu.SubmodalHasControl();
			ConfigureCancel();

			//
			Dialog.SetActive(true);
			Game.Focus.This(ConfirmButton);
		}

		public void ConfirmChanges() {
			Close();
		}

		// -------------------------------------------------------------------------

		void OnCancel(InputAction.CallbackContext _) {
			Close();
		}

		void Close() {
			ResetCancel();
			OptionsMenu.RestoreNormal();

			//
			Game.Focus.This(EditControlsButton);
			Dialog.SetActive(false);
		}


		void ConfigureCancel() {
			ResetCancel();

			//
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnCancel;
		}

		void ResetCancel() {
			if (Cancel != null) {
				Cancel.performed -= OnCancel;
			}

			Cancel = null;
		}

		// -------------------------------------------------------------------------

	}
}
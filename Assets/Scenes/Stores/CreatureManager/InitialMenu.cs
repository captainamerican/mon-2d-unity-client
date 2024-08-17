using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class InitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] List<Button> Buttons;

		[Header("Menus")]
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		Action onBack;

		int selectedButtonIndex = 0;

		// -------------------------------------------------------------------------

		private void OnEnable() {
			ConfigureCancel();
			ConfigureButtons();
			FocusPreviouslySelectedButton();
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= OnGoBack;
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}

		// -------------------------------------------------------------------------

		public void Configure(Action onBack) {
			this.onBack = onBack;
		}

		public void GoToCreaturesMenu() {
			CreaturesMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		public void GoToCreatorMenu() {
			EditInitialMenu.Configure(null);
			EditInitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		void GoBack() {
			selectedButtonIndex = 0;

			//
			onBack();
		}

		// -------------------------------------------------------------------------

		void ConfigureCancel() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureButtons() {
			for (int i = 0; i < Buttons.Count; i++) {
				int j = i;
				Buttons[i]
					.GetComponent<InformationButton>()
					.Configure(() => selectedButtonIndex = j);
			}
		}

		void FocusPreviouslySelectedButton() {
			var button = Buttons[selectedButtonIndex];

			button.Select();
			button.OnSelect(null);
		}

		// -------------------------------------------------------------------------

	}
}
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class InitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] List<Button> Buttons;
		[SerializeField] List<InformationButton> InformationButtons;

		[Header("Menus")]
		[SerializeField] CompendiumMenu CompendiumMenu;
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] InventoryMenu InventoryMenu;
		[SerializeField] OptionsMenu OptionsMenu;
		[SerializeField] StatusMenu StatusMenu;
		[SerializeField] WorldMapMenu WorldMapMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		Action goBack;

		int currentButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancel();
			ConfigureOptions();
			SelectCurrentOption();
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
			GoBack();
		}

		void GoBack() {
			goBack?.Invoke();
		}

		// -------------------------------------------------------------------------

		public void Configure(Action goBack) {
			this.goBack = goBack;
		}

		void ConfigureCancel() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureOptions() {
			Do.ForEach(InformationButtons, ConfigureButton);
		}

		void ConfigureButton(InformationButton informationButton, int index) {
			informationButton.Configure(() => OnButtonHighlighted(index));
		}

		// -------------------------------------------------------------------------

		void OnButtonHighlighted(int index) {
			currentButtonIndex = index;
		}

		void SelectCurrentOption() {
			Game.Btn.Select(Buttons[currentButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void OptionSelected(int index) {
			switch (index) {
				case 0:
					CompendiumMenu.gameObject.SetActive(true);
					break;

				case 1:
					CreaturesMenu.gameObject.SetActive(true);
					break;

				case 2:
					InventoryMenu.gameObject.SetActive(true);
					break;

				case 3:
					WorldMapMenu.gameObject.SetActive(true);
					break;

				case 4:
					StatusMenu.gameObject.SetActive(true);
					break;

				case 5:
					OptionsMenu.gameObject.SetActive(true);
					break;

				default:
					return;
			}

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

	}
}
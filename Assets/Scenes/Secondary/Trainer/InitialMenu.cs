using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class InitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] TextMeshProUGUI Description;

		[SerializeField] List<Button> Buttons;
		[SerializeField] List<InformationButton> InformationButtons;
		[SerializeField] List<string> Descriptions;

		[Header("Menus")]
		[SerializeField] SoulDustExtractorMenu SoulDustExtractorMenu;
		[SerializeField] BodyPartReclaimationMenu BodyPartReclaimationMenu;
		[SerializeField] BodyPartQualityRestorationMenu BodyPartQualityRestorationMenu;

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
			Description.text = Descriptions[index];
		}

		void SelectCurrentOption() {
			Game.Btn.Select(Buttons[currentButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void OptionSelected(int index) {
			switch (index) {
				case 0:
					break;

				case 1:
					SoulDustExtractorMenu.gameObject.SetActive(true);
					break;

				case 2:
					BodyPartQualityRestorationMenu.gameObject.SetActive(true);
					break;

				case 3:
					BodyPartReclaimationMenu.gameObject.SetActive(true);
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
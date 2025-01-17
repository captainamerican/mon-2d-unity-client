using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class InitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] List<Button> Buttons;

		[Header("Menus")]
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] EditInitialMenu EditInitialMenu;
		[SerializeField] EditPartyMenu EditPartyMenu;

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
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}

		// -------------------------------------------------------------------------

		public void Configure(Action onBack) {
			this.onBack = onBack;

			CreaturesMenu.gameObject.SetActive(false);
			EditInitialMenu.gameObject.SetActive(false);
			EditPartyMenu.gameObject.SetActive(false);
		}

		public void GoToCreaturesMenu() {
			CreaturesMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		public void GoToCreatorMenu() {
			EditInitialMenu.Configure(new EditingCreature {
				IsNew = true,
				Creature = new() {
					Id = Game.Id.Generate(),
					Name = "",
				},
				Original = new(),
				AvailableHead = new(Engine.Profile.BodyPartStorage.Head),
				AvailableTorso = new(Engine.Profile.BodyPartStorage.Torso),
				AvailableTail = new(Engine.Profile.BodyPartStorage.Tail),
				AvailableAppendage = new(Engine.Profile.BodyPartStorage.Appendage)
			});
			EditInitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		public void GoToEditPartyMenu() {
			EditPartyMenu.gameObject.SetActive(true);

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
			Game.Focus.This(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

	}
}
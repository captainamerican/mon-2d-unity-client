using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CreatureManager {
	public class StartMenu : MonoBehaviour {
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] List<Button> Buttons;
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] EditStart EditStartMenu;

		InputAction Cancel;

		Action onBack;

		private void OnEnable() {
			ConfigureCancel();

			//
			for (int i = 0; i < Buttons.Count; i++) {
				int up = i == 0 ? Buttons.Count - 1 : i - 1;
				int down = i == Buttons.Count - 1 ? 0 : i + 1;

				Button button = Buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = Buttons[up];
				navigation.selectOnDown = Buttons[down];

				button.navigation = navigation;
			}

			//
			Buttons[0].Select();
			Buttons[0].OnSelect(null);
		}

		private void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= OnGoBack;
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}

		public void Configure(Action onBack) {
			this.onBack = onBack;
		}

		void GoBack() {
			onBack();
		}

		public void GoToCreaturesMenu() {
			CreaturesMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		public void GoToCreatorMenu() {
			EditStartMenu.Configure(null);
			EditStartMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}


		void ConfigureCancel() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}
	}

}
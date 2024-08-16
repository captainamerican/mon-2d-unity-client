using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CreatureManager {
	public class EditSkillsMenu : MonoBehaviour {
		[SerializeField] Engine Engine;

		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] EditStart EditStartMenu;

		Game.ConstructedCreature creature;

		InputAction Cancel;

		void OnEnable() {
			ConfigureCancelAction();

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

		void GoBack() {
			EditStartMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		void ConfigureCancelAction() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		public void Configure(Game.ConstructedCreature creature) {
			this.creature = creature;
		}


	}
}
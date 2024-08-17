using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CreatureManager {
	public class EditSkillsMenu : MonoBehaviour {
		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

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
			EditInitialMenu.gameObject.SetActive(true);

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
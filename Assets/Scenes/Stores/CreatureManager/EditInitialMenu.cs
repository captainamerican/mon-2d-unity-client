using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class EditInitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] EditPartsMenu EditPartsMenu;
		[SerializeField] EditSkillsMenu EditSkillsMenu;
		[SerializeField] List<Button> Buttons;

		[SerializeField] TextMeshProUGUI Description;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] EditNameMenu EditNameMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		ConstructedCreature creature;

		bool goBackToStart;
		int selectedButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

			//
			Description.text = creature?.Name == null
				? "Construct new creature"
				: $"Edit {creature?.Name}";

			//
			Buttons[selectedButtonIndex].Select();
			Buttons[selectedButtonIndex].OnSelect(null);
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

		void GoBack() {
			selectedButtonIndex = 0;

			//
			(
				goBackToStart
					? InitialMenu.gameObject
					: CreaturesMenu.gameObject
			).SetActive(true);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		void ConfigureCancelAction() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		public void Configure(ConstructedCreature creature) {
			goBackToStart = creature == null;
			this.creature = creature ?? new ConstructedCreature();
		}

		public void OpenEditPartsMenu() {
			selectedButtonIndex = 0;

			//
			EditPartsMenu.gameObject.SetActive(true);
			EditPartsMenu.Configure(creature);

			//
			gameObject.SetActive(false);
		}

		public void OpenEditSkillsMenu() {
			selectedButtonIndex = 1;

			//
			EditSkillsMenu.gameObject.SetActive(true);
			EditSkillsMenu.Configure(creature);

			//
			gameObject.SetActive(false);
		}

		public void OpenEditNameMenu() {
			selectedButtonIndex = 2;

			//
			EditNameMenu.gameObject.SetActive(true);
			EditNameMenu.Configure(creature);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

	}
}
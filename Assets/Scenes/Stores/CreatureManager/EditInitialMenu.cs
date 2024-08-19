using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		[SerializeField] Button DeleteButton;
		[SerializeField] Button ConfirmButton;

		[SerializeField] TextMeshProUGUI Description;

		[Header("Confirm Deletion Modal")]
		[SerializeField] GameObject ConfirmDeletionModal;
		[SerializeField] Button AcceptDeletionButton;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] EditNameMenu EditNameMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		ConstructedCreature creature;

		bool isNewCreature;
		int selectedButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

			//
			ConfirmDeletionModal.SetActive(false);

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
				isNewCreature
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

		void ConfigureButtons() {
			DeleteButton.gameObject.SetActive(!isNewCreature);
			ConfirmButton.gameObject.SetActive(isNewCreature);

			var buttons = Buttons.Where(button => button.gameObject.activeSelf).ToList();

			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				//
				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;
			}
		}

		// -------------------------------------------------------------------------

		public void Configure(ConstructedCreature creature) {
			isNewCreature = creature == null;
			this.creature = creature ?? new ConstructedCreature {
				Id = Engine.GenerateRandomId(),
				Name = "New Creature"
			};

			//
			Description.text = $"Edit {this.creature.Name}";

			//
			ConfigureButtons();
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

		public void DeconstructCreature() {
			ConfirmDeletionModal.SetActive(true);

			//
			AcceptDeletionButton.Select();
			AcceptDeletionButton.OnSelect(null);
		}

		public void ConfirmDeletion() {
			Engine.Profile.Creatures.RemoveAll(c => c.Id == creature.Id);
			Engine.Profile.Party.Creatures.RemoveAll(c => c.Id == creature.Id);

			//
			GoBack();
		}

		public void CancelDeletion() {
			ConfirmDeletionModal.SetActive(false);

			//
			Buttons[3].Select();
			Buttons[3].OnSelect(null);
		}

		public void ConfirmNewCreature() {
			Engine.Profile.Creatures.Add(creature);
		}

		// -------------------------------------------------------------------------

	}
}
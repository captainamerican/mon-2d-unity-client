using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class EditInitialMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum FocusPhase {
			Normal,
			DeleteModal,
			CancelModal,
			IncompleteModal,
			CantDeleteModal
		}

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

		[Header("Confirm Deletion Modal")]
		[SerializeField] GameObject DeleteModal;
		[SerializeField] Button CancelDeletionButton;

		[Header("Abandon Chances Modal")]
		[SerializeField] GameObject CancelModal;
		[SerializeField] Button CancelChangesButton;

		[Header("Incomplete Modal")]
		[SerializeField] GameObject IncompleteModal;
		[SerializeField] Button OkayIncompleteButton;

		[Header("Cant Delete Last Modal")]
		[SerializeField] GameObject CantDeleteLastModal;
		[SerializeField] Button OkayCantDeleteLastButton;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] EditNameMenu EditNameMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		EditingCreature editing;

		FocusPhase phase;

		bool isNewCreature;
		int selectedButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

			//
			DeleteModal.SetActive(false);
			CancelModal.SetActive(false);

			//
			Game.Button.Select(Buttons[selectedButtonIndex]);

			//
			phase = FocusPhase.Normal;
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= HandleCancelAction;
		}

		void HandleCancelAction(InputAction.CallbackContext ctx) {
			switch (phase) {
				case FocusPhase.Normal:
					CancelChanges();
					break;

				case FocusPhase.DeleteModal:
					CancelDeleteCreature();
					break;

				case FocusPhase.CancelModal:
					CancelCancelChanges();
					break;
			}
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
				Cancel.performed -= HandleCancelAction;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += HandleCancelAction;
		}

		void ConfigureButtons() {
			for (int i = 0; i < Buttons.Count; i++) {
				int up = i == 0 ? Buttons.Count - 1 : i - 1;
				int down = i == Buttons.Count - 1 ? 0 : i + 1;

				//
				Button button = Buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = Buttons[up];
				navigation.selectOnDown = Buttons[down];

				button.navigation = navigation;

				//
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => selectedButtonIndex = j);
			}
		}

		// -------------------------------------------------------------------------

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;

			//
			Description.text = $"Edit '{this.editing.Creature.Name}'";

			//
			ConfigureButtons();
		}

		public void OpenEditPartsMenu() {
			EditPartsMenu.gameObject.SetActive(true);
			EditPartsMenu.Configure(editing);

			//
			gameObject.SetActive(false);
		}

		public void OpenEditSkillsMenu() {
			EditSkillsMenu.gameObject.SetActive(true);
			EditSkillsMenu.Configure(editing);

			//
			gameObject.SetActive(false);
		}

		public void OpenEditNameMenu() {
			EditNameMenu.gameObject.SetActive(true);
			EditNameMenu.Configure(editing);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		public void DeleteCreature() {
			if (!editing.IsNew && Engine.Profile.Creatures.Count < 2) {
				CantDeleteLastModal.SetActive(true);
				Game.Button.Select(OkayCantDeleteLastButton);
				return;
			}

			//
			DeleteModal.SetActive(true);
			Game.Button.Select(CancelDeletionButton);
		}

		public void ConfirmDeleteCreature() {
			Engine.Profile.Creatures.RemoveAll(c => c.Id == editing.Creature.Id);
			Engine.Profile.Party.Remove(editing.Creature.Id);

			//
			GoBack();
		}

		public void CancelDeleteCreature() {
			CantDeleteLastModal.SetActive(false);
			Game.Button.Select(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void CancelChanges() {
			if (!editing.Changed) {
				GoBack();
				return;
			}

			CancelModal.SetActive(true);
			Game.Button.Select(CancelChangesButton);
		}

		public void ConfirmCancelChanges() {
			GoBack();
		}

		public void CancelCancelChanges() {
			CancelModal.SetActive(false);
			Game.Button.Select(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void CantDeleteLastCreature() {
			CantDeleteLastModal.SetActive(true);
			Game.Button.Select(OkayCantDeleteLastButton);
		}

		public void OkayCantDeleteLast() {
			CantDeleteLastModal.SetActive(false);
			Game.Button.Select(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void CreatureIncomplete() {
			IncompleteModal.SetActive(true);
			Game.Button.Select(OkayCantDeleteLastButton);
		}

		public void OkayIncompleteModal() {
			IncompleteModal.SetActive(false);
			Game.Button.Select(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

		public void SaveChanges() {
			Engine.Profile.Creatures.RemoveAll(c => c.Id == editing.Creature.Id);
			Engine.Profile.Creatures.Add(editing.Creature);

			//
			GoBack();
		}

		// -------------------------------------------------------------------------

	}
}
using System.Collections.Generic;
using System.Linq;

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
			SubModal
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

		[Header("Missing Head Modal")]
		[SerializeField] GameObject MissingHeadModal;
		[SerializeField] Button MissingHeadButton;

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
		int selectedButtonIndex;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

			//
			DeleteModal.SetActive(false);
			CancelModal.SetActive(false);

			//
			Game.Btn.Select(Buttons[selectedButtonIndex]);

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

				case FocusPhase.SubModal:
					CloseModals();
					break;
			}
		}

		// -------------------------------------------------------------------------

		void GoBack() {
			CloseModals();

			//
			selectedButtonIndex = 0;
			gameObject.SetActive(false);

			//
			(
				editing.IsNew
					? InitialMenu.gameObject
					: CreaturesMenu.gameObject
			).SetActive(true);
		}

		// -------------------------------------------------------------------------

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;

			//
			string name = editing.Creature.Name.Trim();
			Description.text = $"Edit “{(name == "" ? "New Creature" : name)}”";

			//
			ConfigureButtons();
		}

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

			//
			Buttons[0].GetComponentInChildren<TextMeshProUGUI>().text =
				(!editing.Creature.HasAllRequiredBodyParts)
					? "Body Parts ⚠"
					: "Body Parts";
			Buttons[1].GetComponentInChildren<TextMeshProUGUI>().text =
				(editing.Creature.MissingHead || !editing.Creature.HasAtLeastOneSkill)
					? "Skills ⚠"
					: "Skills";
			Buttons[2].GetComponentInChildren<TextMeshProUGUI>().text =
				(!editing.Creature.HasSetName)
					? "Name ⚠"
					: "Name";
			Buttons[4].GetComponentInChildren<TextMeshProUGUI>().text =
				(editing.Creature.IsComplete)
					? "Complete"
					: "Complete ⚠";
		}

		// -------------------------------------------------------------------------

		public void OpenEditPartsMenu() {
			EditPartsMenu.gameObject.SetActive(true);
			EditPartsMenu.Configure(editing);

			//
			gameObject.SetActive(false);
		}

		public void OpenEditSkillsMenu() {
			if (editing.Creature.MissingHead) {
				MissingHead();
				return;
			}

			//
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

		public void SaveChanges() {
			if (!editing.Creature.IsComplete) {
				CreatureIncomplete();
				return;
			}

			//
			Engine.Profile.Creatures.RemoveAll(c => c.Id == editing.Creature.Id);
			Engine.Profile.Creatures.Add(editing.Creature);
			Engine.Profile.Storage.Head = editing.AvailableHead;
			Engine.Profile.Storage.Torso = editing.AvailableTorso;
			Engine.Profile.Storage.Tail = editing.AvailableTail;
			Engine.Profile.Storage.Appendage = editing.AvailableAppendage;

			//
			GoBack();
		}

		// -------------------------------------------------------------------------

		public void DeleteCreature() {
			if (
				!editing.IsNew &&
				Engine.Profile.Creatures.Count < 2
			) {
				CantDeleteLastCreature();
				return;
			}

			//
			ShowModal(DeleteModal, CancelDeletionButton);
		}

		public void OnDeleteCreature(int action) {
			CloseModals();

			//
			if (action < 1) {
				return;
			}

			Engine.Profile.Storage.Head.Add(editing.Creature.Head);
			Engine.Profile.Storage.Torso.Add(editing.Creature.Torso);
			Engine.Profile.Storage.Tail.Add(editing.Creature.Tail);
			editing.Creature.Appendages.ForEach(
				Engine.Profile.Storage.Appendage.Add
			);

			//
			Engine.Profile.Creatures.RemoveAll(c => c.Id == editing.Creature.Id);
			Engine.Profile.Party.Remove(editing.Creature.Id);

			//
			GoBack();
		}

		// -------------------------------------------------------------------------

		public void CancelChanges() {
			if (!editing.Changed) {
				GoBack();
				return;
			}

			//
			ShowModal(CancelModal, CancelChangesButton);
		}

		public void OnCancelChanges(int action) {
			CloseModals();

			//
			if (action < 1) {
				return;
			}

			//
			GoBack();
		}

		// -------------------------------------------------------------------------

		void MissingHead() {
			ShowModal(MissingHeadModal, MissingHeadButton);
		}

		public void OnMissingHead() {
			CloseModals();
		}

		// -------------------------------------------------------------------------

		void CantDeleteLastCreature() {
			ShowModal(CantDeleteLastModal, OkayCantDeleteLastButton);
		}

		public void OnCantDeleteLastCreature() {
			CloseModals();
		}

		// -------------------------------------------------------------------------

		void CreatureIncomplete() {
			ShowModal(IncompleteModal, OkayIncompleteButton);
		}

		public void OnCreatureIncomplete() {
			CloseModals();
		}

		// -------------------------------------------------------------------------

		void ShowModal(GameObject modal, Button focus) {
			phase = FocusPhase.SubModal;

			//
			modal.SetActive(true);
			Game.Btn.Select(focus);
		}

		void CloseModals() {
			phase = FocusPhase.Normal;

			//
			IncompleteModal.SetActive(false);
			MissingHeadModal.SetActive(false);
			CantDeleteLastModal.SetActive(false);
			CancelModal.SetActive(false);
			DeleteModal.SetActive(false);

			//
			Game.Btn.Select(Buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------

	}
}
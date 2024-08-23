using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class EditNameMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] GameObject AlphabetParent;
		[SerializeField] GameObject NumbersParent;
		[SerializeField] List<Button> AlphabetButtons;
		[SerializeField] List<Button> NumbersButtons;

		[SerializeField] TextMeshProUGUI NameLabel;
		[SerializeField] TextMeshProUGUI CapsButtonLabel;
		[SerializeField] TextMeshProUGUI ShiftButtonLabel;

		[SerializeField] GameObject Cursor;

		[Header("Menu")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		enum ShiftMode {
			None,
			Shift,
			Capslock
		}

		enum Mode {
			Alphabet,
			NumbersAndSymbols
		}

		EditingCreature editing;

		InputAction Cancel;

		ShiftMode Shift = ShiftMode.Shift;
		Mode WhichMode = Mode.Alphabet;
		string newName;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

			//
			Shift = ShiftMode.Shift;
			WhichMode = Mode.Alphabet;

			// 
			ConfigureCancelAction();
			UpdateShiftButtonLabel();
			UpdateCapsButtonLabel();

			//
			Game.Btn.Select(AlphabetButtons[0]);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= HandleCancelAction;
		}

		void HandleCancelAction(InputAction.CallbackContext _) {
			GoBack();
		}

		// -------------------------------------------------------------------------

		void GoBack() {
			EditInitialMenu.gameObject.SetActive(true);
			EditInitialMenu.Configure(editing);

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

		// -------------------------------------------------------------------------

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;
			newName = editingCreature.Creature.Name;

			//
			UpdateName();
			UpdateCursor();
		}

		public void ConfirmRename() {
			editing.Creature.Name = newName.Trim();

			//
			GoBack();
		}

		public void ToggleCapslock() {
			Shift = Shift == ShiftMode.Capslock
				? ShiftMode.None
				: ShiftMode.Capslock;

			UpdateCapsButtonLabel();
			UpdateShiftButtonLabel();
			UpdateButtons();
		}

		public void ToggleShift() {
			Shift = Shift == ShiftMode.Shift
				? ShiftMode.None
				: ShiftMode.Shift;

			UpdateShiftButtonLabel();
			UpdateCapsButtonLabel();
			UpdateButtons();
		}

		public void ToggleMode() {
			WhichMode = WhichMode == Mode.Alphabet
				? Mode.NumbersAndSymbols
				: Mode.Alphabet;
		}

		public void Backspace() {
			newName = newName.Length > 0 ? newName[0..^1] : "";

			//
			UpdateName();
			UpdateCursor();
		}

		public void OnKeyTyped(string value) {
			if (value == " " && newName.Length > 0 && newName[^1] == ' ') {
				return;
			}

			//
			string final = Shift != ShiftMode.None ? value.ToUpper() : value;
			if (Shift == ShiftMode.Shift) {
				Shift = ShiftMode.None;

				UpdateShiftButtonLabel();
				UpdateCapsButtonLabel();
				UpdateButtons();
			}

			//
			if (newName.Length < 16) {
				newName += final;
				newName = newName.TrimStart();
			}

			//
			UpdateName();
			UpdateCursor();
		}

		void UpdateName() {
			NameLabel.text = newName;
		}

		void UpdateShiftButtonLabel() {
			ShiftButtonLabel.color = Shift == ShiftMode.Shift
				? Color.black
				: new Color(0, 0, 0, 0.5f);
		}

		void UpdateCapsButtonLabel() {
			CapsButtonLabel.color = Shift == ShiftMode.Capslock
				? Color.black
				: new Color(0, 0, 0, 0.5f);
		}

		void UpdateCursor() {
			Cursor.SetActive(newName.Length < 16);
		}

		void UpdateButtons() {
			AlphabetButtons.ForEach(button => {
				var label = button.GetComponentInChildren<TextMeshProUGUI>();

				label.text = Shift > ShiftMode.None
					? label.text.ToUpper()
					: label.text.ToLower();
			});
		}

		// -------------------------------------------------------------------------

	}
}
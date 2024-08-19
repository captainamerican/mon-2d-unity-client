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
		[SerializeField] TextMeshProUGUI ModeLabel;
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

		Game.ConstructedCreature creature;

		ShiftMode Shift = ShiftMode.Shift;
		Mode WhichMode = Mode.Alphabet;
		string newName;

		// -------------------------------------------------------------------------

		void OnEnable() {
			Shift = ShiftMode.Shift;
			WhichMode = Mode.Alphabet;

			//
			SetMode();
			ConfigureCancelAction();
			UpdateShiftButtonLabel();
			UpdateCapsButtonLabel();

			//
			AlphabetButtons[0].Select();
			AlphabetButtons[0].OnSelect(null);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}

		// -------------------------------------------------------------------------

		void GoBack() {
			EditInitialMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		void ConfigureCancelAction() {
		}

		// -------------------------------------------------------------------------

		public void Configure(Game.ConstructedCreature creature) {
			this.creature = creature;
			newName = creature.Name;

			//
			UpdateName();
			UpdateCursor();
		}

		public void ConfirmRename() {
			creature.Name = newName.Trim();

			//
			GoBack();
		}

		public void CancelRename() {
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

			//
			SetMode();
		}

		public void Backspace() {
			newName = newName.Length > 0 ? newName[0..^1] : "";

			//
			UpdateName();
			UpdateCursor();
		}

		void SetMode() {
			AlphabetParent.SetActive(WhichMode == Mode.Alphabet);
			NumbersParent.SetActive(WhichMode == Mode.NumbersAndSymbols);

			ModeLabel.text = WhichMode == Mode.Alphabet ? "Symbols" : "Alphabet";
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
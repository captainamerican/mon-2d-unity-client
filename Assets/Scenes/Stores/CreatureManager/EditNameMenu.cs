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

		InputAction Cancel;

		ShiftMode Shift = ShiftMode.Shift;
		Mode WhichMode = Mode.Alphabet;
		string newName;

		// -------------------------------------------------------------------------

		void OnEnable() {
			Shift = ShiftMode.Shift;
			WhichMode = Mode.Alphabet;

			//
			ConfigureCancelAction();
			SetMode();

			//
			AlphabetButtons[0].Select();
			AlphabetButtons[0].OnSelect(null);
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
			EditInitialMenu.gameObject.SetActive(true);

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

		// -------------------------------------------------------------------------

		public void Configure(Game.ConstructedCreature creature) {
			this.creature = creature;
			newName = creature.Name;

			//
			UpdateName();
		}

		public void ToggleCapslock() {
			Shift = Shift == ShiftMode.Capslock
				? ShiftMode.None
				: ShiftMode.Capslock;
		}

		public void ToggleShift() {
			Shift = Shift == ShiftMode.Shift
				? ShiftMode.None
				: ShiftMode.Shift;
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
		}

		void SetMode() {
			AlphabetParent.SetActive(WhichMode == Mode.Alphabet);
			NumbersParent.SetActive(WhichMode == Mode.NumbersAndSymbols);

			ModeLabel.text = WhichMode == Mode.Alphabet ? "Symbols" : "Alphabet";
		}

		public void OnKeyTyped(string value) {
			string final = Shift != ShiftMode.None ? value.ToUpper() : value;
			if (Shift == ShiftMode.Shift) {
				Shift = ShiftMode.None;
			}

			//
			if (newName.Length < 16) {
				newName += final;
			}

			//
			UpdateName();
		}

		void UpdateName() {
			NameLabel.text = $"Name: {newName}";
		}
	}
}
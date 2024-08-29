using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class OptionsMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum Phase {
			Normal,
			SubModal,
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject FirstSelected;
		[SerializeField] GameObject ReturnToStart;

		[SerializeField] Slider DialogueSpeed;
		[SerializeField] Slider MusicVolume;
		[SerializeField] Slider SFXVolume;

		[SerializeField] TMP_Dropdown BattleAnimations;
		[SerializeField] TMP_Dropdown CheatMenu;
		[SerializeField] TMP_Dropdown Speedrunning;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;

		// --------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;

			//
			ConfigureInput();
			ConfigureControls();

			//
			EventSystem.current.SetSelectedGameObject(FirstSelected);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			switch (phase) {
				case Phase.Normal:
					GoBack();
					break;

				case Phase.SubModal:
					GoBackToNormal();
					break;
			}
		}

		void GoBack() {
			InitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		void GoBackToNormal() {
			phase = Phase.Normal;

			//
			EventSystem.current.SetSelectedGameObject(ReturnToStart);
		}

		// ------------------------------------------------------------------------- 

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureControls() {
			DialogueSpeed.value = Engine.Profile.Options.DialogueSpeed;
			MusicVolume.value = Engine.Profile.Options.MusicVolume;
			SFXVolume.value = Engine.Profile.Options.SFXVolume;
			BattleAnimations.value = Engine.Profile.Options.BattleAnimations ? 0 : 1;
			CheatMenu.value = Engine.Profile.Options.CheatMenu ? 0 : 1;
			Speedrunning.value = Engine.Profile.Options.Speedrunning ? 0 : 1;
		}

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		// -------------------------------------------------------------------------

		// -------------------------------------------------------------------------
	}
}
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

		[Header("Confirm Exit Dialog")]
		[SerializeField] GameObject ConfirmExitDialog;
		[SerializeField] Button CancelConfirm;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;

		// --------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			ConfirmExitDialog.SetActive(false);

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
			ConfirmExitDialog.SetActive(false);
			EventSystem.current.SetSelectedGameObject(ReturnToStart);
		}

		// ------------------------------------------------------------------------- 

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureControls() {
			DialogueSpeed.value = 1 - Engine.Profile.Options.DialogueSpeed;
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

		public void OnDialogueSpeedChanged(float newValue) {
			Engine.Profile.Options.DialogueSpeed = Mathf.Clamp01(1 - newValue);
		}

		public void MusicVolumeChanged(float newValue) {
			Engine.Profile.Options.MusicVolume = Mathf.Clamp01(newValue);
		}

		public void SFXVolumeChanged(float newValue) {
			Engine.Profile.Options.SFXVolume = Mathf.Clamp01(newValue);
		}

		public void BattleAnimationsChanged(int index) {
			Engine.Profile.Options.BattleAnimations = index == 0;
		}

		public void CheatMenuChanged(int index) {
			Engine.Profile.Options.CheatMenu = index == 0;
		}

		public void SpeedrunningChanged(int index) {
			Engine.Profile.Options.Speedrunning = index == 0;
		}

		public void ConfirmExitToStart() {
			phase = Phase.SubModal;

			ConfirmExitDialog.SetActive(true);
			Game.Focus.This(CancelConfirm);
		}

		public void OnExitToStart(int action) {
			if (action < 1) {
				GoBackToNormal();
				return;
			}

			//
			Engine.NextScene = new NextScene { Name = StartScreen.Scene.Name };
			SceneManager.LoadSceneAsync(Loader.Scene.Name, LoadSceneMode.Additive);
		}

		// -------------------------------------------------------------------------

		public void SubmodalHasControl() {
			phase = Phase.SubModal;

			Cancel.performed -= OnGoBack;
		}

		public void RestoreNormal() {
			phase = Phase.Normal;

			// 
			RemoveInputCallbacks();
			Cancel.performed += OnGoBack;
		}

		// -------------------------------------------------------------------------
	}
}
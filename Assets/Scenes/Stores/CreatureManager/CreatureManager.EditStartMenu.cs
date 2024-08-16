using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CreatureManager {
	public class EditStart : MonoBehaviour {
		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;

		[SerializeField] StartMenu StartMenu;
		[SerializeField] CreaturesMenu CreaturesMenu;
		[SerializeField] List<Button> Buttons;

		[SerializeField] TextMeshProUGUI Description;

		InputAction Cancel;

		bool goBackToStart;
		ConstructedCreature creature;

		void OnEnable() {
			ConfigureCancelAction();

			//
			Description.text = creature?.Name == null ? "Construct new creature" : $"Edit {creature?.Name}";

			//
			Buttons[0].Select();
			Buttons[0].OnSelect(null);
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
			if (goBackToStart) {
				StartMenu.gameObject.SetActive(true);
			} else {
				CreaturesMenu.gameObject.SetActive(true);
			}

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

		public void Configure(ConstructedCreature creature) {
			goBackToStart = creature == null;
			this.creature = creature ?? new ConstructedCreature();
		}

		public void OpenEditPartsMenu() {
		}

		public void OpenEditSkillsMenu() {
		}

		public void OpenEditNameMenu() {
		}
	}
}
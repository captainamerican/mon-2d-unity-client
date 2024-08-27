using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class SparringPitMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		public enum Phase {
			Normal,
			Selecting
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] Item SoulDust;

		[Header("Local")]
		[SerializeField] ScrollView ScrollView;
		[SerializeField] Transform ListParent;
		[SerializeField] GameObject ListTemplate;
		[SerializeField] Button RemoveButton;

		[SerializeField] List<Button> Installed;
		[SerializeField] List<TextMeshProUGUI> Warnings;
		[SerializeField] Button JuiceButton;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;
		int selectedButtonIndex;
		int selectedSlotIndex;

		readonly List<Button> slots = new();
		readonly List<Button> buttons = new();

		// -------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			selectedButtonIndex = 0;
			selectedSlotIndex = 0;

			//
			ConfigureAvailableSlots();
			ConfigureButtons();
			ConfigureNavigation();
			ConfigureInput();
			ScrollView.UpdateVisibleButtonRange(buttons, 0);

			// 
			Game.Btn.Select(slots[0]);
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

				case Phase.Selecting:
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
			Game.Btn.Select(slots[selectedSlotIndex]);

			//
			phase = Phase.Normal;
		}

		// -------------------------------------------------------------------------

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void ConfigureAvailableSlots() {
			slots.Clear();

			slots.Add(Installed[5]);
		}

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureButtons() {
			foreach (var button in buttons) {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			}

			//
			buttons.Clear();
			buttons.Add(RemoveButton);

			//
			List<Game.BodyPartEntryBase> items = new();
			Engine.Profile.Storage.Head.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Torso.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Tail.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Appendage.ForEach(entry => items.Add(entry));

			items
				.Where(entry => {
					if (entry is Game.HeadBodyPartEntry head) {
						return head.Experience >= head.BodyPart.ExperienceToLevel;
					} else if (entry is Game.TorsoBodyPartEntry torso) {
						return torso.Experience >= torso.BodyPart.ExperienceToLevel;
					} else if (entry is Game.TailBodyPartEntry tail) {
						return tail.Experience >= tail.BodyPart.ExperienceToLevel;
					} else if (entry is Game.AppendageBodyPartEntry appendage) {
						return appendage.Experience >= appendage.BodyPart.ExperienceToLevel;
					}

					return false;
				})
				.OrderByDescending(entry => entry.Experience)
				.ThenBy(entry => {
					if (entry is Game.HeadBodyPartEntry head) {
						return head.BodyPart.Name;
					} else if (entry is Game.TorsoBodyPartEntry torso) {
						return torso.BodyPart.Name;
					} else if (entry is Game.TailBodyPartEntry tail) {
						return tail.BodyPart.Name;
					} else if (entry is Game.AppendageBodyPartEntry appendage) {
						return appendage.BodyPart.Name;
					}

					return "???";
				})
				.ToList()
				.ForEach(ConfigureButton);
		}

		void ConfigureButton(Game.BodyPartEntryBase entry) {
			int i = buttons.Count;
			int j = i;

			//
			GameObject buttonGO = Instantiate(ListTemplate, ListParent);
			buttonGO.SetActive(true);

			if (entry is Game.HeadBodyPartEntry head) {
				buttonGO.name = head.BodyPart.Name;
			} else if (entry is Game.TorsoBodyPartEntry torso) {
				buttonGO.name = torso.BodyPart.Name;
			} else if (entry is Game.TailBodyPartEntry tail) {
				buttonGO.name = tail.BodyPart.Name;
			} else if (entry is Game.AppendageBodyPartEntry appendage) {
				buttonGO.name = appendage.BodyPart.Name;
			}

			//
			Button button = buttonGO.GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(OnButtonSelected);

			//
			buttonGO.GetComponent<BodyPartButton>()
				.Configure(entry);

			//
			buttons.Add(button);
		}

		void ConfigureNavigation() {
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;

				//
				int j = i;
				button.GetComponent<InformationButton>()
				.Configure(() => {
					selectedButtonIndex = j;
					ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);
				});
			}
		}

		// -------------------------------------------------------------------------

		void OnButtonSelected() {
		}

		// -------------------------------------------------------------------------

	}
}
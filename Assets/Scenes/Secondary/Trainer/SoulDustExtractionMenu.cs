using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class SoulDustExtractorMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		public enum Phase {
			Normal,
			SubModal
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

		[Header("Information")]
		[SerializeField] TextMeshProUGUI CurrentLabel;
		[SerializeField] TextMeshProUGUI ExtractLabel;
		[SerializeField] TextMeshProUGUI TotalLabel;


		[Header("Quantity Modal")]
		[SerializeField] GameObject QuantityModal;
		[SerializeField] TextMeshProUGUI ItemLabel;
		[SerializeField] TextMeshProUGUI ResultLabel;
		[SerializeField] Button QuantityCancelButton;

		[Header("Warning Modal")]
		[SerializeField] GameObject WarningModal;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;
		int selectedButtonIndex;

		readonly List<Button> buttons = new();
		readonly List<Game.BodyPartEntry> bodyPartEntries = new();

		// -------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			selectedButtonIndex = 0;

			//
			QuantityModal.SetActive(false);
			WarningModal.SetActive(false);

			//
			ConfigureButtons();
			ConfigureNavigation();
			ConfigureInput();
			UpdateCurrentLabel();
			ScrollView.UpdateVisibleButtonRange(buttons, 0);

			//
			if (buttons.Count < 1) {
				WarningModal.SetActive(true);
			} else {
				Game.Focus.This(buttons[0]);
			}
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
					GoBackToList();
					break;
			}
		}

		void GoBack() {
			InitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		void GoBackToList() {
			QuantityModal.SetActive(false);
			Game.Focus.This(buttons[selectedButtonIndex]);

			//
			phase = Phase.Normal;
		}

		// -------------------------------------------------------------------------

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
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
			bodyPartEntries.Clear();

			//
			List<Game.BodyPartEntry> items = new();
			Engine.Profile.BodyPartStorage.Head.ForEach(entry => items.Add(entry));
			Engine.Profile.BodyPartStorage.Torso.ForEach(entry => items.Add(entry));
			Engine.Profile.BodyPartStorage.Tail.ForEach(entry => items.Add(entry));
			Engine.Profile.BodyPartStorage.Appendage.ForEach(entry => items.Add(entry));

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

		void ConfigureButton(Game.BodyPartEntry entry) {
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
			bodyPartEntries.Add(entry);
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

					//
					int current = Engine.Profile.Inventory.GetItemQuantity(SoulDust);
					int extract = bodyPartEntries[j].Experience;
					int total = Mathf.Clamp(current + extract, 0, 999999);

					ExtractLabel.text = $"{extract:n0}";
					TotalLabel.text = $"{total:n0}";
				});
			}
		}

		// -------------------------------------------------------------------------

		void OnButtonSelected() {


			var entry = bodyPartEntries[selectedButtonIndex];

			UpdateItemLabel(entry);
			ResultLabel.text = $"Extract will result in {entry.Experience:N0} Soul Dust.";

			//
			QuantityModal.SetActive(true);
			Game.Focus.This(QuantityCancelButton);

			//
			phase = Phase.SubModal;
		}

		void UpdateItemLabel(Game.BodyPartEntry entry) {
			if (entry is Game.HeadBodyPartEntry entry1) {
				ItemLabel.text = entry1.BodyPart.Name;
			} else if (entry is Game.TorsoBodyPartEntry entry2) {
				ItemLabel.text = entry2.BodyPart.Name;
			} else if (entry is Game.TailBodyPartEntry entry3) {
				ItemLabel.text = entry3.BodyPart.Name;
			} else if (entry is Game.AppendageBodyPartEntry entry4) {
				ItemLabel.text = entry4.BodyPart.Name;
			}
		}

		public void ExtractSoulDust(int action) {
			if (action < 1) {
				GoBackToList();
				return;
			}

			//
			var entry = bodyPartEntries[selectedButtonIndex];

			Engine.Profile.Inventory.AdjustItem(SoulDust, entry.Experience);
			Engine.Profile.BodyPartStorage.AddToReclaimable(entry);
			Engine.Profile.BodyPartStorage.Remove(entry);

			//
			QuantityModal.SetActive(false);

			//
			var button = buttons[selectedButtonIndex];
			button.gameObject.SetActive(false);
			Destroy(button.gameObject);

			//
			buttons.RemoveAt(selectedButtonIndex);
			bodyPartEntries.RemoveAt(selectedButtonIndex);

			//
			ConfigureNavigation();
			UpdateCurrentLabel();

			//
			if (buttons.Count < 1) {
				WarningModal.SetActive(true);
				ScrollView.UpdateVisibleButtonRange(buttons, 0);
			} else {
				selectedButtonIndex = Mathf.Clamp(selectedButtonIndex, 0, buttons.Count - 1);
				Game.Focus.This(buttons[selectedButtonIndex]);
				ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);
			}

			//
			phase = Phase.Normal;
		}

		void UpdateCurrentLabel() {
			CurrentLabel.text = $"{Engine.Profile.Inventory.GetItemQuantity(SoulDust):N0}";
			ExtractLabel.text = "0";
			TotalLabel.text = CurrentLabel.text;
		}

		// -------------------------------------------------------------------------

	}
}
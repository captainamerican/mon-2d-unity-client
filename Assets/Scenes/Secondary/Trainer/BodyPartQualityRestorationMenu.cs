using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Trainer {
	public class BodyPartQualityRestorationMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		public enum Phase {
			Normal,
			SubModal,
			QuantityModal
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

		[Header("Quantity Modal")]
		[SerializeField] GameObject QuantityModal;
		[SerializeField] TextMeshProUGUI ItemLabel;
		[SerializeField] TextMeshProUGUI CostLabel;
		[SerializeField] TextMeshProUGUI LeftOverLabel;
		[SerializeField] TextMeshProUGUI RatioLabel;
		[SerializeField] RectTransform RatioThumb;
		[SerializeField] InformationButton ConfirmButton;
		[SerializeField] InformationButton CancelButton;

		[Header("Warning Modal")]
		[SerializeField] GameObject WarningModal;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;
		InputAction CategoryLeft;
		InputAction CategoryRight;

		Phase phase;
		int selectedButtonIndex;

		readonly List<Button> buttons = new();
		readonly List<Game.BodyPartEntryBase> bodyPartEntries = new();

		float durationUntilNextTrigger;

		int qualityLevelsToRestore = 0;
		int maxQualityLevelsToRestore = 0;

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
				Game.Btn.Select(buttons[0]);
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
				case Phase.QuantityModal:
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
			Game.Btn.Select(buttons[selectedButtonIndex]);

			//
			phase = Phase.Normal;
		}
		private void Update() {
			if (phase == Phase.QuantityModal) {
				bool left = CategoryLeft.IsPressed();
				bool right = CategoryRight.IsPressed();
				if (left || right) {
					durationUntilNextTrigger -= Time.unscaledDeltaTime;
					if (durationUntilNextTrigger < 0) {
						durationUntilNextTrigger = 0.1f;

						if (left) {
							DecreaseQuantity();
						}

						if (right) {
							IncreaseQuantity();
						}
					}
				}

			}
		}

		void DecreaseQuantity() {
			qualityLevelsToRestore -= 1;
			if (qualityLevelsToRestore < 1) {
				qualityLevelsToRestore = maxQualityLevelsToRestore;
			}

			//
			UpdateCostAndRemainingLabels();
			UpdateRatioThumb();
		}

		void IncreaseQuantity() {
			qualityLevelsToRestore += 1;
			if (qualityLevelsToRestore > maxQualityLevelsToRestore) {
				qualityLevelsToRestore = 1;
			}

			//
			UpdateCostAndRemainingLabels();
			UpdateRatioThumb();
		}

		void UpdateRatioThumb() {
			var parent = RatioThumb.parent.GetComponent<RectTransform>();

			float parentWidth = Mathf.Ceil(parent.rect.width);
			float rawButtonWidth = maxQualityLevelsToRestore > 0 ? parentWidth / (maxQualityLevelsToRestore + 1) : parentWidth;
			float buttonWidth = Mathf.Round(Mathf.Clamp(rawButtonWidth, 10f, parentWidth));
			float track = parentWidth - buttonWidth;
			float offset = maxQualityLevelsToRestore > 0 ? Mathf.Ceil(track * ((float) (qualityLevelsToRestore - 1) / ((float) (maxQualityLevelsToRestore)))) : 0;

			RatioThumb.anchoredPosition = new Vector2(offset, 0);
			RatioThumb.sizeDelta = new Vector2(buttonWidth, 4);

			//
			RatioLabel.text = $"{100 - maxQualityLevelsToRestore + qualityLevelsToRestore}";
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
			CategoryLeft = PlayerInput.currentActionMap.FindAction("CategoryLeft");
			CategoryRight = PlayerInput.currentActionMap.FindAction("CategoryRight");

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
			List<Game.BodyPartEntryBase> items = new();
			Engine.Profile.Storage.Head.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Torso.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Tail.ForEach(entry => items.Add(entry));
			Engine.Profile.Storage.Appendage.ForEach(entry => items.Add(entry));

			items
				.Where(entry => entry.Quality < 1)
				.OrderBy(entry => entry.Quality)
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

			//
			RatioThumb.GetComponent<InformationButton>()
				.Configure(() => phase = Phase.QuantityModal);
			ConfirmButton.Configure(() => phase = Phase.SubModal);
			CancelButton.Configure(() => phase = Phase.SubModal);
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
				});
			}
		}

		// -------------------------------------------------------------------------

		void OnButtonSelected() {
			var entry = bodyPartEntries[selectedButtonIndex];

			//
			qualityLevelsToRestore = 1;
			maxQualityLevelsToRestore = 100 - Mathf.RoundToInt(entry.Quality * 100);

			UpdateItemLabel(entry);
			UpdateCostAndRemainingLabels();
			UpdateRatioThumb();

			//
			QuantityModal.SetActive(true);
			Game.Btn.Select(RatioThumb.GetComponent<Button>());

			//
			phase = Phase.QuantityModal;
		}

		void UpdateItemLabel(Game.BodyPartEntryBase entry) {
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

		public void RestoreQuality(int action) {
			if (action < 1) {
				GoBackToList();
				return;
			}

			//
			var entry = bodyPartEntries[selectedButtonIndex];
			entry.Quality = Mathf.Clamp01(entry.Quality + 0.01f * qualityLevelsToRestore);

			int cost = qualityLevelsToRestore * 10;
			Engine.Profile.Inventory.AdjustItem(SoulDust, -cost);

			//
			QuantityModal.SetActive(false);

			//
			var button = buttons[selectedButtonIndex];
			button.GetComponent<BodyPartButton>()
				.Configure(entry);

			//
			if (entry.Quality >= 1) {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);

				//
				buttons.RemoveAt(selectedButtonIndex);
				bodyPartEntries.RemoveAt(selectedButtonIndex);
			}

			//
			ConfigureNavigation();
			UpdateCurrentLabel();
			UpdateCostAndRemainingLabels();

			//
			if (buttons.Count < 1) {
				WarningModal.SetActive(true);
				ScrollView.UpdateVisibleButtonRange(buttons, 0);
			} else {
				selectedButtonIndex = Mathf.Clamp(selectedButtonIndex, 0, buttons.Count - 1);

				Game.Btn.Select(buttons[selectedButtonIndex]);
				ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);
			}

			//
			phase = Phase.Normal;
		}

		void UpdateCurrentLabel() {
			CurrentLabel.text = $"{Engine.Profile.Inventory.GetItemQuantity(SoulDust):N0}";
		}

		void UpdateCostAndRemainingLabels() {
			int cost = qualityLevelsToRestore * 10;
			int remainder = Engine.Profile.Inventory.GetItemQuantity(SoulDust) - cost;

			CostLabel.text = $"{cost:n0}";
			LeftOverLabel.text = $"{remainder:n0}";
		}

		public void FocusConfirmButton() {
			Game.Btn.Select(ConfirmButton.GetComponent<Button>());
		}

		// -------------------------------------------------------------------------

	}
}
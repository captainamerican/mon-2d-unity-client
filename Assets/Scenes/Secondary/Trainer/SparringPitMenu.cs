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
		[SerializeField] GameObject ItemTemplate;
		[SerializeField] Button RemoveButton;

		[SerializeField] List<Button> Installed;
		[SerializeField] List<GameObject> Warnings;
		[SerializeField] Button JuiceButton;
		[SerializeField] Button JuiceItemButton;

		[SerializeField] GameObject EnhancerInformation;
		[SerializeField] GameObject AppendageInformation;

		[SerializeField] List<Item> KeyItems;

		[Header("Item Information")]
		[SerializeField] GameObject ItemInformation;
		[SerializeField] TextMeshProUGUI BoostLabel;
		[SerializeField] TextMeshProUGUI DownsideLabel;
		[SerializeField] TextMeshProUGUI DescriptionLabel;
		[SerializeField] TextMeshProUGUI FlavorLabel;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;
		int selectedSlotIndex;
		int selectedButtonIndex;

		readonly List<Button> slots = new();
		readonly List<Button> buttons = new();

		// -------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			selectedSlotIndex = 0;
			selectedButtonIndex = 0;

			//
			AppendageInformation.SetActive(false);
			EnhancerInformation.SetActive(false);
			ItemInformation.SetActive(false);

			//
			ConfigureAvailableSlots();
			ConfigureInstalledNavigation();
			ConfigureButtons();
			ConfigureListNavigation();
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
			ItemInformation.SetActive(false);
			EnhancerInformation.SetActive(false);
			AppendageInformation.SetActive(false);

			//
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
			bool hasMannequinPartA = Engine.Profile.Inventory.HasItem(KeyItems[0]);
			bool hasMannequinPartB = Engine.Profile.Inventory.HasItem(KeyItems[1]);
			bool hasMannequinPartC = Engine.Profile.Inventory.HasItem(KeyItems[2]);
			bool hasMannequinPartD = Engine.Profile.Inventory.HasItem(KeyItems[3]);

			//
			Installed[0].gameObject.SetActive(hasMannequinPartA);
			Warnings[0].SetActive(!hasMannequinPartA);

			Installed[1].gameObject.SetActive(hasMannequinPartB);
			Warnings[1].SetActive(!hasMannequinPartB);

			Installed[2].gameObject.SetActive(hasMannequinPartC);
			Warnings[2].SetActive(!hasMannequinPartC);

			Installed[5].gameObject.SetActive(hasMannequinPartD);
			Installed[6].gameObject.SetActive(hasMannequinPartD);
			Warnings[3].SetActive(!hasMannequinPartD);
			Warnings[4].SetActive(!hasMannequinPartD);

			//
			if (hasMannequinPartA) {
				Installed[0].GetComponent<BodyPartButton>()
					.Configure(Engine.Profile.SparringPit.Head);
			}

			if (hasMannequinPartB) {
				Installed[1].GetComponent<BodyPartButton>()
					.Configure(Engine.Profile.SparringPit.Torso);
			}

			if (hasMannequinPartC) {
				Installed[2].GetComponent<BodyPartButton>()
					.Configure(Engine.Profile.SparringPit.Tail);
			}

			Installed[3].GetComponent<BodyPartButton>()
					.Configure(Engine.Profile.SparringPit.GetAppendage(0), "Appendage");
			Installed[4].GetComponent<BodyPartButton>()
					.Configure(Engine.Profile.SparringPit.GetAppendage(1), "Appendage");

			if (hasMannequinPartD) {
				Installed[5].GetComponent<BodyPartButton>()
						.Configure(Engine.Profile.SparringPit.GetAppendage(2), "Appendage");
				Installed[6].GetComponent<BodyPartButton>()
						.Configure(Engine.Profile.SparringPit.GetAppendage(3), "Appendage");
			}

			//
			slots.Clear();

			//
			Installed.ForEach(button => {
				if (!button.gameObject.activeSelf) {
					return;
				}

				//
				int i = slots.Count;
				button.GetComponent<InformationButton>()
					.Configure(() => {
						selectedSlotIndex = i;

						//
						ConfigureButtons();
						ConfigureListNavigation();

						//
						ItemInformation.SetActive(false);
						EnhancerInformation.SetActive(false);
						AppendageInformation.SetActive(true);
					});
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(SlotButtonSelected);

				//
				slots.Add(button);
			});

			//
			JuiceItemButton.gameObject.SetActive(false);
			JuiceButton.gameObject.SetActive(false);

			if (Engine.Profile.SparringPit.Enhancer?.Item != null) {
				JuiceItemButton.GetComponent<InformationButton>()
					.Configure(JuiceItemButtonHighlighted);
				JuiceItemButton.onClick.RemoveAllListeners();
				JuiceItemButton.onClick.AddListener(JuiceButtonSelected);
				JuiceItemButton.gameObject.SetActive(true);

				JuiceItemButton.GetComponent<ItemButton>()
					.Configure(Engine.Profile.SparringPit.Enhancer);

				//
				slots.Add(JuiceItemButton);
			} else {
				JuiceButton.GetComponent<InformationButton>()
					.Configure(JuiceButtonHighlighted);
				JuiceButton.onClick.RemoveAllListeners();
				JuiceButton.onClick.AddListener(JuiceButtonSelected);
				JuiceButton.gameObject.SetActive(true);

				//
				slots.Add(JuiceButton);
			}
		}

		void JuiceItemButtonHighlighted() {
			selectedSlotIndex = slots.Count - 1;

			//
			ItemInformation.SetActive(true);
			ConfigureItemInformation(Engine.Profile.SparringPit.Enhancer);

			EnhancerInformation.SetActive(false);
			AppendageInformation.SetActive(false);

			//
			ConfigureButtons();
			ConfigureListNavigation();
		}

		void JuiceButtonSelected() {
			ItemInformation.SetActive(false);
			EnhancerInformation.SetActive(false);
			AppendageInformation.SetActive(false);

			//
			phase = Phase.Selecting;
			Game.Btn.Select(buttons[0]);
		}

		void SlotButtonSelected() {
			phase = Phase.Selecting;
			Game.Btn.Select(buttons[0]);
		}

		void JuiceButtonHighlighted() {
			selectedSlotIndex = slots.Count - 1;

			EnhancerInformation.SetActive(true);
			AppendageInformation.SetActive(false);

			//
			ConfigureButtons();
			ConfigureListNavigation();
		}

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureButtons() {
			buttons.Remove(RemoveButton);

			foreach (var button in buttons) {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			}

			buttons.Clear();

			//
			RemoveButton.GetComponent<InformationButton>()
				.Configure(() => {
					selectedButtonIndex = 0;
					ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);

					//
					ItemInformation.SetActive(false);
				});
			RemoveButton.onClick.RemoveAllListeners();
			RemoveButton.onClick.AddListener(OnRemoveButtonSelected);
			buttons.Add(RemoveButton);

			//
			var slotButton = slots[selectedSlotIndex];
			int slotType = slotButton.GetComponent<SparringPitItemSlot>().SlotType;

			if (slotType < 5) {
				List<Game.BodyPartEntryBase> items = new();

				switch (slotType) {
					case 1:
						Engine.Profile.Storage.Head.ForEach(entry => items.Add(entry));
						break;
					case 2:
						Engine.Profile.Storage.Torso.ForEach(entry => items.Add(entry));
						break;
					case 3:
						Engine.Profile.Storage.Tail.ForEach(entry => items.Add(entry));
						break;
					case 4:
						Engine.Profile.Storage.Appendage.ForEach(entry => items.Add(entry));
						break;
				}

				items
					.OrderBy(entry => entry.Experience)
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
					.ForEach(ConfigureBodyPartButton);
			} else {
				Engine.Profile.Inventory.All
					.Where(
						entry => entry.Amount > 0 &&
						entry?.Item?.Type == Game.ItemType.TrainingItem
					)
					.OrderBy(entry => entry?.Item?.Name ?? "???")
					.ToList()
					.ForEach(ConfigureEnhancerButton);
			}
		}

		void ConfigureEnhancerButton(Game.InventoryEntry entry) {
			int i = buttons.Count;
			int j = i;

			//
			GameObject buttonGO = Instantiate(ItemTemplate, ListParent);
			buttonGO.SetActive(true);

			//
			Button button = buttonGO.GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => OnButtonSelected(entry));

			// 
			button.GetComponent<InformationButton>()
			.Configure(() => {
				selectedSlotIndex = j;
				ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);

				//
				ItemInformation.SetActive(true);
				ConfigureItemInformation(entry);
			});

			// 
			button.GetComponent<ItemButton>()
				.Configure(entry);

			//
			buttons.Add(button);
		}

		void ConfigureItemInformation(Game.InventoryEntry entry) {
			Game.Effect boost = entry.Item.Effects[0];
			Game.Effect downside = entry.Item.Effects[1];

			//
			BoostLabel.text = $"+{boost.Strength}%";
			DownsideLabel.text = $"{downside.Strength}%";
			DescriptionLabel.text = entry.Item.Description;
			FlavorLabel.text = entry.Item.FlavorText;
		}

		void ConfigureBodyPartButton(Game.BodyPartEntryBase entry) {
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
			button.onClick.AddListener(() => OnButtonSelected(entry));

			//
			buttonGO.GetComponent<BodyPartButton>()
				.Configure(entry);

			// 
			button.GetComponent<InformationButton>()
			.Configure(() => {
				selectedButtonIndex = j;
				ScrollView.UpdateVisibleButtonRange(buttons, selectedButtonIndex);
			});

			//
			buttons.Add(button);
		}

		void ConfigureListNavigation() {
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;
			}
		}

		void ConfigureInstalledNavigation() {
			for (int i = 0; i < slots.Count; i++) {
				int up = i == 0 ? slots.Count - 1 : i - 1;
				int down = i == slots.Count - 1 ? 0 : i + 1;

				Button button = slots[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = slots[up];
				navigation.selectOnDown = slots[down];

				button.navigation = navigation;
			}
		}

		// -------------------------------------------------------------------------

		void OnButtonSelected(Game.InventoryEntry entry) {
			Engine.Profile.SparringPit.Enhancer = entry;

			//
			ConfigureAvailableSlots();
			ConfigureInstalledNavigation();
			ConfigureButtons();
			ConfigureListNavigation();
			GoBackToNormal();

			//
			Game.Btn.Select(slots.Last());
		}

		void OnButtonSelected(Game.BodyPartEntryBase entry) {
			if (entry is Game.AppendageBodyPartEntry appendage) {
				var slot = slots[selectedSlotIndex];
				var itemSlot = slot.GetComponent<SparringPitItemSlot>();
				int slotIndex = itemSlot.Slot - 3;
				Engine.Profile.SparringPit.SetAppendage(appendage, slotIndex);
			} else {
				Engine.Profile.SparringPit.Set(entry);
			}

			//
			Engine.Profile.Storage.Remove(entry);

			//
			ConfigureAvailableSlots();
			ConfigureInstalledNavigation();
			ConfigureButtons();
			ConfigureListNavigation();
			GoBackToNormal();
		}

		void OnRemoveButtonSelected() {
			Button slot = slots[selectedSlotIndex];
			var itemSlot = slot.GetComponent<SparringPitItemSlot>();
			int slotType = itemSlot.SlotType;

			switch (slotType) {
				case 1:
					var head = Engine.Profile.SparringPit.Head;
					Engine.Profile.SparringPit.Head = null;

					if (head?.BodyPart != null) {
						Engine.Profile.Storage.Add(head);
					}
					break;
				case 2:
					var torso = Engine.Profile.SparringPit.Torso;
					Engine.Profile.SparringPit.Torso = null;

					if (torso?.BodyPart != null) {
						Engine.Profile.Storage.Add(torso);
					}
					break;
				case 3:
					var tail = Engine.Profile.SparringPit.Tail;
					Engine.Profile.SparringPit.Torso = null;

					if (tail?.BodyPart != null) {
						Engine.Profile.Storage.Add(tail);
					}
					break;
				case 4:
					int slotIndex = itemSlot.Slot - 3;
					var appendage = Engine.Profile.SparringPit.GetAppendage(slotIndex);
					Engine.Profile.SparringPit.SetAppendage(null, slotIndex);

					if (appendage?.BodyPart != null) {
						Engine.Profile.Storage.Add(appendage);
					}
					break;

				case 5:
					Engine.Profile.SparringPit.Enhancer = null;
					break;
			}

			//
			ConfigureAvailableSlots();
			ConfigureInstalledNavigation();
			ConfigureButtons();
			ConfigureListNavigation();
			GoBackToNormal();
		}

		// -------------------------------------------------------------------------

	}
}
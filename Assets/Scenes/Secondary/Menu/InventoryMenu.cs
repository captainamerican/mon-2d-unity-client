using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class InventoryMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] Transform ListParent;
		[SerializeField] Button ListTemplate;

		[SerializeField] GameObject ItemInformation;
		[SerializeField] TextMeshProUGUI ItemDescription;
		[SerializeField] TextMeshProUGUI ItemFlavor;

		[SerializeField] TextMeshProUGUI CategoryLabel;
		[SerializeField] List<InformationButton> CategoryButtons;

		[SerializeField] ScrollView ScrollView;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// ------------------------------------------------------------------------- 

		InputAction Cancel;

		List<Button> buttons = new();
		List<Game.InventoryEntry> inventoryEntries = new();

		int categoryIndex;

		readonly List<Game.ItemType> categoryOrder = new() {
			Game.ItemType.Consumable,
			Game.ItemType.Reusable,
			Game.ItemType.CraftingMaterial,
			Game.ItemType.TrainingItem,
			Game.ItemType.BodyPart,
			Game.ItemType.KeyItem,
		};
		readonly List<string> categoryLabels = new() {
			"Consumables",
			"Reusable",
			"Crafting Material",
			"Training Items",
			"Body Parts",
			"Key Items"
		};

		// --------------------------------------------------------------------------

		void OnEnable() {
			categoryIndex = 0;

			ConfigureInput();
			ConfigureList();

			for (int i = 0; i < CategoryButtons.Count; i++) {
				int j = i;

				var informationButton = CategoryButtons[i];
				informationButton.Configure(() => {
					ItemInformation.SetActive(false);

					//
					categoryIndex = j;

					ConfigureList();

					//
					ScrollView.UpdateVisibleButtonRange(buttons, 0);
				});
			}

			//
			ItemInformation.SetActive(false);
			ScrollView.UpdateVisibleButtonRange(buttons, 0);
			if (buttons.Count > 0) {
				Game.Focus.This(buttons[0]);
			} else {
				Game.Focus.This(CategoryButtons[categoryIndex].GetComponent<Button>());
			}
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			InitialMenu.gameObject.SetActive(true);

			// 
			gameObject.SetActive(false);
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
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureList() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});

			buttons.Clear();
			inventoryEntries.Clear();

			//
			Game.ItemType category = categoryOrder[categoryIndex];

			Engine.Profile.Inventory.All
				.Where(entry =>
					entry.Item.Type == category &&
					entry.Amount > 0
				)
				.OrderBy(entry => entry.Item.Name)
				.ToList()
				.ForEach(entry => {
					var button = Instantiate(ListTemplate, ListParent);
					button.gameObject.SetActive(true);

					button.GetComponent<ItemButton>()
						.Configure(entry);

					int i = buttons.Count;
					button.GetComponent<InformationButton>()
						.Configure(() => {
							ItemInformation.SetActive(true);
							ItemDescription.text = entry.Item.Description;
							ItemFlavor.text = entry.Item.FlavorText;

							//
							ScrollView.UpdateVisibleButtonRange(buttons, i);
						});

					//
					buttons.Add(button);
					inventoryEntries.Add(entry);
				});

			//
			Button categoryButton = CategoryButtons[categoryIndex].GetComponent<Button>();
			Navigation categoryNavigation = categoryButton.navigation;

			categoryNavigation.selectOnUp = (buttons.Count > 0) ? buttons.Last() : null;
			categoryNavigation.selectOnDown = (buttons.Count > 0) ? buttons[0] : null;
			categoryButton.navigation = categoryNavigation;

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i - 1;
				int down = i + 1;

				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = up < 0
					? categoryButton
					: buttons[up];
				navigation.selectOnDown = down >= buttons.Count
					? categoryButton
					: buttons[down];

				button.navigation = navigation;
			}

			//
			CategoryLabel.text = categoryLabels[categoryIndex];
		}

		// -------------------------------------------------------------------------
	}
}
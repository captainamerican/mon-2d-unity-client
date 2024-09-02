using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Crafting {
	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		public const string Name = "Crafting";

		static public IEnumerator Load(Action onDone) {
			OnDone = onDone;

			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] EventSystem DebugEventSystem;
		[SerializeField] Camera DebugCamera;
		[SerializeField] Canvas Canvas;
		[SerializeField] Transform ItemParent;
		[SerializeField] GameObject ItemTemplate;

		[SerializeField] TextMeshProUGUI ItemDescription;
		[SerializeField] TextMeshProUGUI ItemFlavorText;

		[SerializeField] TextMeshProUGUI Ingredients;
		[SerializeField] TextMeshProUGUI CanHasLabel;

		[SerializeField] ScrollView ScrollView;

		[Header("Quantity Modal")]
		[SerializeField] GameObject QuantityModal;
		[SerializeField] TextMeshProUGUI QuantityMaximum;
		[SerializeField] TextMeshProUGUI QuantityLabel;
		[SerializeField] Slider Quantity;
		[SerializeField] Button QuantityCancelButton;

		// -------------------------------------------------------------------------

		public enum Phase {
			Base,
			Modal
		}

		readonly Dictionary<Item, int> iventoryItemCount = new();
		readonly Dictionary<Item, bool> canBeCrafted = new();
		readonly Dictionary<Item, bool> hasEquipmentToCraft = new();
		readonly Dictionary<Item, int> timesCanBeCrated = new();
		readonly List<Button> buttons = new();

		int currentButtonIndex = 0;
		Item selectedItem;

		Phase phase;
		InputAction Cancel;

		static Action OnDone = () => Debug.Log("Close");

		// -------------------------------------------------------------------------

		void Awake() {
			RemoveInput();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");

			// 
			Configure();
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= CloseMenu;
		}

		void RemoveInput() {
			if (Cancel != null) {
				Cancel.performed -= CloseMenu;
			}
		}

		// -------------------------------------------------------------------------

		void CloseMenu(InputAction.CallbackContext ctx) {
			switch (phase) {
				case Phase.Base:
					RemoveInput();
					RemoveNavigation();

					buttons.Clear();
					canBeCrafted.Clear();
					hasEquipmentToCraft.Clear();
					iventoryItemCount.Clear();

					//
					OnDone?.Invoke();
					break;

				case Phase.Modal:
					CloseQuantityModal();
					break;

			}
		}

		void Configure() {
			phase = Phase.Base;

			//
			QuantityModal.SetActive(false);

			//
			RebuildDictionaries();

			Engine
				.Items
				.Where(item => HasEquipmentToCraft(item) && item.Recipe.Count > 0)
				.OrderBy(item => item.SortName)
				.ToList()
				.ForEach(item => {
					GameObject buttonGO = Instantiate(ItemTemplate, ItemParent);
					buttonGO.name = item.Name;
					buttonGO.SetActive(true);

					// update text
					TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
					label.text = item.Name;

					if (!canBeCrafted[item]) {
						Color color = label.color;
						color.a = 0.5f;

						label.color = color;
					}

					// configure button
					int buttonIndex = buttons.Count;
					Button button = buttonGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button
						.onClick
						.AddListener(() => OnItemSelected(item));
					button
					.GetComponent<InformationButton>()
					.Configure(() => OnItemHovered(item, buttonIndex));

					//
					buttons.Add(button);
				});

			// navigation
			AddNavigation();

			// selected first button
			Game.Focus.This(buttons[currentButtonIndex]);

			//
			Cancel.performed += CloseMenu;
		}

		void RebuildDictionaries() {
			canBeCrafted.Clear();
			hasEquipmentToCraft.Clear();
			iventoryItemCount.Clear();

			//
			Engine.Items.ForEach(item => {
				iventoryItemCount[item] = 0;
				canBeCrafted[item] = false;
			});
			Engine.Profile.Inventory.All.ForEach(entry => {
				iventoryItemCount[entry.Item] += entry.Amount;
			});

			//
			int recipeMaxIngredients = 1 + Engine.CraftingEquipment.Sum(
				item => iventoryItemCount[item] > 0 ? 1 : 0
			);

			Engine.Items.ForEach(item => {
				List<Game.RecipeIngredient> ingredients = item.Recipe;
				if (ingredients.Count < 1) {
					return;
				}

				int totalIngredients = 0;
				Dictionary<Item, int> needed = new();
				ingredients.ForEach(ingredient => {
					if (!needed.ContainsKey(ingredient.Item)) {
						needed[ingredient.Item] = 0;
					}

					needed[ingredient.Item] += ingredient.Quantity;
					totalIngredients += ingredient.Quantity;
				});

				//
				canBeCrafted[item] =
					totalIngredients > 0 &&
					needed.All(
						ingredient =>
							iventoryItemCount.ContainsKey(ingredient.Key) &&
							iventoryItemCount[ingredient.Key] >= ingredient.Value
					) &&
					iventoryItemCount[item] < 99;
				hasEquipmentToCraft[item] = ingredients.Count <= recipeMaxIngredients;

				//
				int minimumCraftable = 999;
				ingredients.ForEach(ingredient => {
					int has = iventoryItemCount.ContainsKey(ingredient.Item) ? iventoryItemCount[ingredient.Item] : 0;
					int craft = ingredient.Quantity > 0
						? (int) Mathf.FloorToInt((float) has / (float) ingredient.Quantity)
						: 0;

					if (craft < minimumCraftable) {
						minimumCraftable = craft;
					}
				});

				timesCanBeCrated[item] = Mathf.Clamp(minimumCraftable, 0, 99 - iventoryItemCount[item]);
			});
		}

		bool HasEquipmentToCraft(Item item) {
			return hasEquipmentToCraft.ContainsKey(item) && hasEquipmentToCraft[item];
		}

		void OnItemHovered(
			Item item,
			int index
		) {
			currentButtonIndex = index;

			//
			ItemDescription.text = item.Description;
			ItemFlavorText.text = item.FlavorText;

			//
			List<Game.RecipeIngredient> ingredients = item.Recipe
				.OrderBy(ingredient => ingredient.Item.Name)
				.ToList();
			Ingredients.text = string.Join("\n", Do.Times(4, i => {
				if (i >= ingredients.Count) {
					return " ";
				}

				//
				var ingredient = ingredients[i];

				//
				return $"{ingredient.Quantity}x {ingredient.Item.Name}";
			}));

			//
			CanHasLabel.text = $"{timesCanBeCrated[item]:D2}/{iventoryItemCount[item]:D2}";

			//
			ScrollView.UpdateVisibleButtonRange(buttons, index);
		}

		void OnItemSelected(Item item) {
			if (!canBeCrafted[item]) {
				return;
			}

			// 
			StartCoroutine(ShowQuantityModal(item));
		}

		IEnumerator ShowQuantityModal(Item item) {
			yield return new WaitForEndOfFrame();

			//
			phase = Phase.Modal;

			// 
			QuantityModal.SetActive(true);

			//
			selectedItem = item;

			QuantityMaximum.text = $"{timesCanBeCrated[item]:D2}";
			Quantity.value = 1;
			Quantity.maxValue = timesCanBeCrated[item];

			QuantityLabel.text = "01";

			//
			EventSystem.current.SetSelectedGameObject(Quantity.gameObject);
		}

		public void CraftItem(int action) {
			CloseQuantityModal();
			if (action < 0) {
				return;
			}

			//
			int quantity = Mathf.FloorToInt(Quantity.value);
			selectedItem.Recipe.ForEach(ingredient =>
				Engine.Profile.Inventory
				.AdjustItem(
					ingredient.Item,
					-(ingredient.Quantity * quantity)
				)
			);

			Engine.Profile.Inventory
				.AdjustItem(selectedItem, quantity);

			//
			StartCoroutine(Reset());
		}

		IEnumerator Reset() {
			yield return new WaitForEndOfFrame();

			// 
			RemoveNavigation();

			buttons.ForEach(button => Destroy(button.gameObject));
			buttons.Clear();

			// 
			Configure();
		}

		public void OnQuantityChanged(float quantity) {
			QuantityLabel.text = $"{quantity:d2}";
		}

		void CloseQuantityModal() {
			phase = Phase.Base;

			//
			QuantityModal.SetActive(false);

			//
			buttons[currentButtonIndex].Select();
			buttons[currentButtonIndex].OnSelect(null);
			buttons[currentButtonIndex].GetComponent<InformationButton>().OnSelect(null);
		}

		void AddNavigation() {
			for (int i = 0; i < buttons.Count; i++) {
				int up = i <= 0 ? buttons.Count - 1 : i - 1;
				int down = i >= buttons.Count - 1 ? 0 : i + 1;

				//
				Navigation navigation = buttons[i].navigation;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				//
				buttons[i].navigation = navigation;
			}
		}

		void RemoveNavigation() {
			buttons.ForEach(button => {
				Navigation navigation = button.navigation;
				navigation.selectOnUp = null;
				navigation.selectOnDown = null;

				//
				button.navigation = navigation;

				button.onClick.RemoveAllListeners();
			});
		}
	}
}
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

namespace Crafting {
	public class Scene : MonoBehaviour {
		public const string Name = "Crafting";

		public enum Phase {
			Base,
			Modal
		}

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

		[SerializeField] List<TextMeshProUGUI> Ingredients;
		[SerializeField] TextMeshProUGUI CanHasLabel;

		[SerializeField] RectTransform ScrollbarThumb;

		[Header("Quantity Modal")]
		[SerializeField] GameObject QuantityModal;
		[SerializeField] TextMeshProUGUI QuantityMinimum;
		[SerializeField] TextMeshProUGUI QuantityMaximum;
		[SerializeField] TextMeshProUGUI QuantityLabel;
		[SerializeField] RectTransform QuantityThumb;

		readonly Dictionary<Item, int> iventoryItemCount = new();
		readonly Dictionary<Item, bool> canBeCrafted = new();
		readonly Dictionary<Item, bool> hasEquipmentToCraft = new();
		readonly Dictionary<Item, int> timesCanBeCrated = new();
		readonly List<Button> buttons = new();

		int visibleButtonRangeMin = 0;
		int visibleButtonRangeMax = 8;
		int currentButtonIndex = 0;

		const int quantityRatio = 37;
		const int quantityWidth = 8;
		int quantityMin = 0;
		int quantityMax = 0;
		int selectedQuantity = 0;
		Item selectedItem;

		float durationUntilNextTrigger;

		Phase phase;
		InputAction CategoryLeft;
		InputAction CategoryRight;
		InputAction Submit;
		InputAction Cancel;

		static Action OnDone;

		static public IEnumerator Load(Action onDone) {
			OnDone = onDone;

			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}

		private void Awake() {
			if (EventSystem.current == null) {
				DebugEventSystem.enabled = true;
			}

			if (Camera.main == null) {
				DebugCamera.enabled = true;
			}
		}

		void Start() {
			Submit = PlayerInput.currentActionMap.FindAction("Submit");
			CategoryLeft = PlayerInput.currentActionMap.FindAction("CategoryLeft");
			CategoryRight = PlayerInput.currentActionMap.FindAction("CategoryRight");
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");

			// 
			Configure();
		}

		private void Update() {
			if (phase == Phase.Modal) {
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

		private void OnDestroy() {
			Submit.performed -= CraftItem;
			Cancel.performed -= CloseMenu;
		}

		void CloseMenu(InputAction.CallbackContext ctx) {
			Cancel.performed -= CloseMenu;

			RemoveNavigation();

			buttons.Clear();
			canBeCrafted.Clear();
			hasEquipmentToCraft.Clear();
			iventoryItemCount.Clear();

			//
			OnDone?.Invoke();
		}


		void Configure() {
			phase = Phase.Base;

			//
			QuantityModal.SetActive(false);

			//
			RebuildDictionaries();

			Engine
				.AllItems
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
			buttons[currentButtonIndex].Select();
			buttons[currentButtonIndex].OnSelect(null);
			buttons[currentButtonIndex].GetComponent<InformationButton>().OnSelect(null);

			//
			Cancel.performed += CloseMenu;

			Canvas.worldCamera = Camera.main;
		}

		void RebuildDictionaries() {
			canBeCrafted.Clear();
			hasEquipmentToCraft.Clear();
			iventoryItemCount.Clear();

			//
			Engine.AllItems.ForEach(item => {
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

			Engine.AllItems.ForEach(item => {
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
			Do.Times(4, i => {
				var label = Ingredients[i];
				if (i > ingredients.Count - 1) {
					label.text = "";
					return "";
				}

				//
				var ingredient = ingredients[i];
				label.text = $"{ingredient.Quantity}x {ingredient.Item.Name}";

				//
				return "";
			});

			//
			CanHasLabel.text = $"{timesCanBeCrated[item]:D2}/{iventoryItemCount[item]:D2}";

			//
			UpdateVisibleButtonRange(index);
		}

		void OnItemSelected(Item item) {
			if (!canBeCrafted[item]) {
				return;
			}

			//
			EventSystem.current.SetSelectedGameObject(null);
			StartCoroutine(ShowQuantityModal(item));
		}

		IEnumerator ShowQuantityModal(Item item) {
			yield return new WaitForEndOfFrame();

			//
			phase = Phase.Modal;

			//
			Submit.performed += CraftItem;

			Cancel.performed -= CloseMenu;
			Cancel.performed += CloseQuantityModal;

			// 
			QuantityModal.SetActive(true);

			QuantityMinimum.text = $"01";
			QuantityMaximum.text = $"{timesCanBeCrated[item]:D2}";

			//
			selectedItem = item;
			selectedQuantity = 1;

			quantityMin = 1;
			quantityMax = timesCanBeCrated[item];

			//
			UpdateQuantityGauge();
		}

		void DecreaseQuantity() {
			selectedQuantity -= 1;
			if (selectedQuantity < quantityMin) {
				selectedQuantity = quantityMax;
			}

			UpdateQuantityGauge();
		}

		void IncreaseQuantity() {
			selectedQuantity += 1;
			if (selectedQuantity > quantityMax) {
				selectedQuantity = quantityMin;
			}

			UpdateQuantityGauge();
		}

		void UpdateQuantityGauge() {
			const float total = quantityRatio - quantityWidth;
			int max = quantityMax - quantityMin;
			float ratio = max > 0 ? (float) (selectedQuantity - quantityMin) / (float) (max) : 1;

			QuantityThumb.anchoredPosition = new Vector2(total * ratio, 0);
			QuantityLabel.text = $"{selectedQuantity:D2}";
		}

		void CraftItem(InputAction.CallbackContext ctx) {
			Submit.performed -= CraftItem;
			Cancel.performed -= CloseQuantityModal;

			//
			QuantityModal.SetActive(false);

			//
			selectedItem.Recipe.ForEach(ingredient =>
				Engine.Profile.Inventory
				.AdjustItem(
					ingredient.Item,
					-(ingredient.Quantity * selectedQuantity)
				)
			);

			Engine.Profile.Inventory
				.AdjustItem(selectedItem, selectedQuantity);

			//
			selectedItem = null;
			selectedQuantity = 0;
			quantityMax = 0;

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

		void CloseQuantityModal(InputAction.CallbackContext ctx) {
			Submit.performed -= CraftItem;
			Cancel.performed -= CloseQuantityModal;

			Cancel.performed += CloseMenu;

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

		void UpdateVisibleButtonRange(int index) {
			if (index < visibleButtonRangeMin) {
				visibleButtonRangeMin = index;
				visibleButtonRangeMax = index + 8;
			} else if (index > visibleButtonRangeMax) {
				visibleButtonRangeMax = index;
				visibleButtonRangeMin = index - 8;
			}

			//
			UpdateVisibleButtons();
			UpdateScrollbarThumb(index);
		}

		void UpdateVisibleButtons() {
			for (int i = 0; i < buttons.Count; i++) {
				bool enabled = i >= visibleButtonRangeMin && i <= visibleButtonRangeMax;
				var button = buttons[i].gameObject;
				if (button == null) {
					continue;
				}

				RectTransform rt = button.GetComponent<RectTransform>();
				Vector2 sizeDelta = rt.sizeDelta;
				sizeDelta.y = enabled ? 10 : 0;

				rt.sizeDelta = sizeDelta;

				foreach (var label in button.GetComponentsInChildren<TextMeshProUGUI>(true)) {
					label.gameObject.SetActive(enabled);
				}
			}
		}

		void UpdateScrollbarThumb(int index) {
			ScrollbarThumb.gameObject.SetActive(buttons.Count > 0);

			if (buttons.Count > 0) {
				var parent = ScrollbarThumb.parent.GetComponent<RectTransform>();

				float parentHeight = Mathf.Ceil(parent.rect.height);
				float rawButtonHeight = buttons.Count > 1 ? parentHeight / buttons.Count : parentHeight;
				float buttonHeight = Mathf.Round(Mathf.Clamp(rawButtonHeight, 1f, parentHeight));
				float track = parentHeight - buttonHeight;
				float offset = buttons.Count > 1 ? Mathf.Ceil(track * ((float) index / ((float) (buttons.Count - 1)))) : 0;

				ScrollbarThumb.anchoredPosition = new Vector2(0, -offset);
				ScrollbarThumb.sizeDelta = new Vector2(2, buttonHeight);
			}
		}
	}
}
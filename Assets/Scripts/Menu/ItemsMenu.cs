using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemsMenu : AbstractMenu {
	[SerializeField] Engine Engine;
	[SerializeField] PlayerInput PlayerInput;
	[SerializeField] GameObject ItemTemplate;
	[SerializeField] GameObject DescriptionContainer;
	[SerializeField] TextMeshProUGUI Description;
	[SerializeField] TextMeshProUGUI FlavorText;
	[SerializeField] List<GameObject> Categories;
	[SerializeField] List<TextMeshProUGUI> CategoriesTabs;
	[SerializeField] RectTransform ScrollbarThumb;

	InputAction CategoryLeft;
	InputAction CategoryRight;
	InputAction Cancel;

	Color SelectedTab = new Color(0.1254902f, 0.04313726f, 0.04313726f, 1f);
	Color UnselectedTab = new Color(0.1254902f, 0.04313726f, 0.04313726f, 0.5f);

	// readonly Dictionary<Item, GameObject> buttonCache = new();

	readonly Dictionary<Game.ItemType, List<Button>> categoryButtons = new() {
		{
			Game.ItemType.Consumable,
			new List<Button>()
		},
		{
			Game.ItemType.Reusable,
			new List<Button>()
		},
		{
			Game.ItemType.CraftingMaterial,
			new List<Button>()
		},
		{
			Game.ItemType.TrainingItem,
			new List<Button>()
		},
		{
			Game.ItemType.BodyPart,
			new List<Button>()
		},
		{
			Game.ItemType.KeyItem,
			new List<Button>()
		}
	};

	readonly List<Game.ItemType> categoryOrder = new() {
		Game.ItemType.Consumable,
		Game.ItemType.Reusable,
		Game.ItemType.CraftingMaterial,
		Game.ItemType.TrainingItem,
		Game.ItemType.BodyPart,
		Game.ItemType.KeyItem,
	};

	int categoryIndex = 0;
	int categoryRangeMin = 0;
	int categoryRangeMax = 6;
	int currentButtonIndex = 0;

	void Start() {
		DescriptionContainer.SetActive(true);
	}

	override public void Show(Action callback) {
		// remove existing buttons
		foreach (var key in categoryButtons.Keys) {
			categoryButtons[key].ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			categoryButtons[key].Clear();
		}

		// generate buttons
		Engine.Profile.Inventory.All
			.Where(entry => entry.Item != null && entry.Amount > 0)
			.OrderBy(entry => entry.Item.Type)
			.ToList()
			.ForEach(entry => {
				GameObject parent = Categories[categoryOrder.FindIndex(co => co == entry.Item.Type)];
				//if (!buttonCache.TryGetValue(entry.Item, out GameObject buttonGO)) {
				GameObject buttonGO = Instantiate(ItemTemplate, parent.transform);
				//buttonCache[entry.Item] = buttonGO;
				//}
				buttonGO.name = $"{entry.Item.Name} x{entry.Amount}";
				buttonGO.transform.SetParent(parent.transform);
				buttonGO.SetActive(true);

				// update text
				TextMeshProUGUI[] labels = buttonGO.GetComponentsInChildren<TextMeshProUGUI>();

				labels[0].text = entry.Item.Name;
				labels[1].text = entry.Amount < 10 ? $"x0{entry.Amount}" : $"x{entry.Amount}";

				if (entry.Item.Type != Game.ItemType.Consumable) {
					Color color = labels[0].color;
					color.a = 0.5f;

					labels[0].color = color;
					labels[1].color = color;
				}

				// configure button
				int buttonIndex = categoryButtons[entry.Item.Type].Count;
				Button button = buttonGO.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnItemSelected(entry));
				button
				.GetComponent<InformationButton>()
					.Configure(() => {
						Description.text = entry.Item.Description;
						FlavorText.text = entry.Item.FlavorText;

						currentButtonIndex = buttonIndex;

						UpdateVisibleButtonRange(buttonIndex);
					});

				//
				categoryButtons[entry.Item.Type].Add(button);
			});

		// wire up all the button navigation
		foreach (var buttons in categoryButtons.Values) {
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

		ChangeCategory(0);

		//
		CategoryLeft = PlayerInput.currentActionMap.FindAction("CategoryLeft");
		CategoryRight = PlayerInput.currentActionMap.FindAction("CategoryRight");
		Cancel = PlayerInput.currentActionMap.FindAction("Cancel");

		CategoryLeft.performed += HandleCategoryLeft;
		CategoryRight.performed += HandleCategoryRight;
		Cancel.performed += HandleCancel;

		//
		base.Show(callback);
	}

	void ChangeCategory(int newIndex) {
		categoryIndex = newIndex;
		currentButtonIndex = 0;

		Description.text = "";
		FlavorText.text = "";

		//
		for (int i = 0; i < Categories.Count; i++) {
			Categories[i].SetActive(i == newIndex);
			CategoriesTabs[i].color = i == newIndex
				? SelectedTab
				: UnselectedTab;
		}

		//
		UpdateVisibleButtonRange(0);

		//
		Game.ItemType category = categoryOrder[categoryIndex];
		var buttons = categoryButtons[category];

		if (buttons.Count > 0) {
			buttons[0].Select();
			buttons[0].OnSelect(null);
			buttons[0].GetComponent<InformationButton>().OnSelect(null);
		}
	}

	void UpdateVisibleButtons() {
		Game.ItemType category = categoryOrder[categoryIndex];
		var buttons = categoryButtons[category];

		for (int i = 0; i < buttons.Count; i++) {
			bool enabled = i >= categoryRangeMin && i <= categoryRangeMax;
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

	void UpdateVisibleButtonRange(int index) {
		if (index < categoryRangeMin) {
			categoryRangeMin = index;
			categoryRangeMax = index + 6;
		} else if (index > categoryRangeMax) {
			categoryRangeMax = index;
			categoryRangeMin = index - 6;
		}

		//
		UpdateVisibleButtons();
		UpdateScrollbarThumb();
	}

	void UpdateScrollbarThumb() {
		Game.ItemType category = categoryOrder[categoryIndex];
		var buttons = categoryButtons[category];

		ScrollbarThumb.gameObject.SetActive(buttons.Count > 0);

		if (buttons.Count > 0) {
			var parent = ScrollbarThumb.transform.parent.GetComponent<RectTransform>();

			float parentHeight = Mathf.Round(parent.rect.height);
			float rawButtonHeight = buttons.Count > 1 ? parentHeight / buttons.Count : parentHeight;
			float buttonHeight = Mathf.Round(Mathf.Clamp(rawButtonHeight, 1f, parentHeight));
			float track = parentHeight - buttonHeight;
			float offset = buttons.Count > 1 ? Mathf.Ceil(track * ((float) currentButtonIndex / ((float) (buttons.Count - 1)))) : 0;

			ScrollbarThumb.anchoredPosition = new Vector2(0, -offset);
			ScrollbarThumb.sizeDelta = new Vector2(3, buttonHeight);
		}

	}

	public void OnSelect(string description) {
		Debug.Log(description);
	}

	void OnItemSelected(Game.InventoryEntry entry) {
		if (entry.Item.Type != Game.ItemType.Consumable) {
			return;
		}

		Debug.Log("item: " + entry.Item.Name);
	}

	void HandleCategoryLeft(InputAction.CallbackContext ctx) {
		categoryIndex -= 1;
		if (categoryIndex < 0) {
			categoryIndex = Categories.Count - 1;
		}

		ChangeCategory(categoryIndex);
	}

	void HandleCategoryRight(InputAction.CallbackContext ctx) {
		categoryIndex += 1;
		if (categoryIndex >= Categories.Count) {
			categoryIndex = 0;
		}

		ChangeCategory(categoryIndex);
	}

	void HandleCancel(InputAction.CallbackContext ctx) {
		CategoryLeft.performed -= HandleCategoryLeft;
		CategoryRight.performed -= HandleCategoryRight;
		Cancel.performed -= HandleCancel;

		CategoryLeft = null;
		CategoryRight = null;
		Cancel = null;

		Exit();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemsMenu : AbstractMenu {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	GameObject Category;

	[SerializeField]
	GameObject Template;

	[SerializeField]
	Button CancelButton;

	override public void Show(Action callback) {
		int offset = 0;
		while (Menu.transform.childCount > 3 || offset > Menu.transform.childCount) {
			Transform child = Menu.transform.GetChild(offset);
			GameObject go = child.gameObject;

			if (go == CancelButton.gameObject || go == Template || go == Category) {
				offset += 1;
				continue;
			}

			child.SetParent(null);
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		// generate items
		Item.Type type = Item.Type.None;

		List<Button> buttons = new();
		Engine.Inventory.Entries
			.Where(entry => entry.Amount > 0)
			.OrderBy(entry => entry.ItemData.Type)
			.ToList()
			.ForEach(entry => {
				if (type != entry.ItemData.Type) {
					type = entry.ItemData.Type;

					GameObject category = Instantiate(Category, Menu.transform);
					TextMeshProUGUI categoryLabel = category.GetComponent<TextMeshProUGUI>();
					categoryLabel.text = Item.Data.TypeName(type);

					category.SetActive(true);
				}

				//
				GameObject newItem = Instantiate(Template, Menu.transform);

				TextMeshProUGUI[] labels = newItem.GetComponentsInChildren<TextMeshProUGUI>();

				labels[0].text = entry.ItemData.Name;
				labels[1].text = $"x{entry.Amount}";

				if (entry.ItemData.Type != Item.Type.Consumable) {
					Color color = labels[0].color;
					color.a = 0.5f;

					labels[0].color = color;
					labels[1].color = color;
				}

				Button button = newItem.GetComponent<Button>();
				button.onClick.AddListener(() => OnItemSelected(entry));
				buttons.Add(button);

				newItem.SetActive(true);
			});

		// set cancel button last
		CancelButton.transform.SetAsLastSibling();
		buttons.Add(CancelButton);

		for (int i = 0; i < buttons.Count; i++) {
			int previous = i <= 0 ? buttons.Count - 1 : i - 1;
			int next = i >= buttons.Count - 1 ? 0 : i + 1;

			Navigation navigation = buttons[i].navigation;
			navigation.selectOnUp = buttons[previous];
			navigation.selectOnDown = buttons[next];

			buttons[i].navigation = navigation;
		}

		//
		buttons[0].Select();
		buttons[0].OnSelect(null);
		EventSystem.current.sendNavigationEvents = true;

		//
		base.Show(callback);
	}

	public void OnCancel() {
		Exit();
	}

	void OnItemSelected(InventoryEntry entry) {
		if (entry.ItemData.Type != Item.Type.Consumable) {
			return;
		}

		Debug.Log("item: " + entry.ItemData.Name);
	}
}

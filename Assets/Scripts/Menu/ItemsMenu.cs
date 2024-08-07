using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;
using Unity.VisualScripting;
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

	[SerializeField]
	GameObject DescriptionContainer;

	[SerializeField]
	TextMeshProUGUI Description;

	[SerializeField]
	TextMeshProUGUI FlavorText;

	override public void Show(Action callback) {
		DescriptionContainer.SetActive(false);

		//
		int offset = 0;
		while (Menu.transform.childCount > 4 || offset > Menu.transform.childCount) {
			Transform child = Menu.transform.GetChild(offset);
			GameObject go = child.gameObject;

			if (
					go == CancelButton.gameObject ||
					go == Template ||
					go == Category ||
					go == DescriptionContainer
				) {
				offset += 1;
				continue;
			}

			child.SetParent(null);
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		// generate items
		Game.ItemType type = Game.ItemType.None;

		List<Button> buttons = new();
		Engine.Profile.Inventory
			.Where(entry => entry.Item != null)
			.Where(entry => entry.Amount > 0)
			.OrderBy(entry => entry.Item.Type)
			.ToList()
			.ForEach(entry => {
				if (type != entry.Item.Type) {
					type = entry.Item.Type;

					GameObject category = Instantiate(Category, Menu.transform);
					TextMeshProUGUI categoryLabel = category.GetComponent<TextMeshProUGUI>();
					categoryLabel.text = Item.TypeName(type);

					category.SetActive(true);
				}

				//
				GameObject newItem = Instantiate(Template, Menu.transform);

				TextMeshProUGUI[] labels = newItem.GetComponentsInChildren<TextMeshProUGUI>();

				labels[0].text = entry.Item.Name;
				labels[1].text = $"x{entry.Amount}";

				if (entry.Item.Type != Game.ItemType.Consumable) {
					Color color = labels[0].color;
					color.a = 0.5f;

					labels[0].color = color;
					labels[1].color = color;
				}

				Button button = newItem.GetComponent<Button>();
				button.onClick.AddListener(() => OnItemSelected(entry));
				button
				.GetComponent<InformationButton>()
					.Configure(() => {
						DescriptionContainer.SetActive(true);
						Description.text = entry.Item.Description;
						FlavorText.text = entry.Item.FlavorText;
					});

				buttons.Add(button);

				newItem.SetActive(true);
			});

		// set cancel button last
		CancelButton.transform.SetAsLastSibling();
		CancelButton
		.GetComponent<InformationButton>()
			.Configure(() => {
				DescriptionContainer.SetActive(false);
			});

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
		buttons[0].GetComponent<InformationButton>().OnSelect(null);
		EventSystem.current.sendNavigationEvents = true;

		//
		base.Show(callback);
	}

	public void OnCancel() {
		Exit();
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
}

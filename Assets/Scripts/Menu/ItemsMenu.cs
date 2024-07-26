using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemsMenu : AbstractMenu {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	Button CancelButton;

	override public void Show(Action callback) {
		while (Menu.transform.childCount > 1) {
			Transform child = Menu.transform.GetChild(0);

			if (child.gameObject == CancelButton.gameObject) {
				continue;
			}

			child.SetParent(null);
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}


		// generate items
		CancelButton.onClick.RemoveAllListeners();

		List<Button> buttons = new();
		Engine.Inventory.Entries.ForEach(entry => {
			GameObject newItem = Instantiate(CancelButton.gameObject, Menu.transform);
			newItem.GetComponentInChildren<TextMeshProUGUI>().text = entry.ItemData.Name;

			Button button = newItem.GetComponent<Button>();
			button.onClick.AddListener(() => OnItemSelected(newItem.transform.GetSiblingIndex()));

			buttons.Add(button);
		});

		// set cancel button last
		CancelButton.transform.SetAsLastSibling();
		CancelButton.onClick.AddListener(() => OnItemSelected(-1));
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

	public void OnItemSelected(int index) {
		Debug.Log("selected " + index);

		if (index < 0) {
			Exit();
		}
	}
}

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureAction : MonoBehaviour {

	[SerializeField]
	Engine Engine;

	[SerializeField]
	ChestId Id;

	[SerializeField]
	PlayerInput PlayerInput;

	[SerializeField]
	List<Item.LootDrop> Drops = new();

	bool isBeingTouched;
	InputAction SubmitAction;

	void Start() {
		isBeingTouched = false;

		SubmitAction = PlayerInput.currentActionMap.FindAction("Submit");
	}

	void Update() {
		if (
			!isBeingTouched ||
			!Engine.PlayerHasControl() ||
			!SubmitAction.WasPressedThisFrame()
		) {
			return;
		}

		OpenChest();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		isBeingTouched = true;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		isBeingTouched = false;
	}

	public void OpenChest() {
		if (Engine.Profile.OpenedChest(Id)) {
			WorldDialogue.Display("Nothing here!");
			return;
		}

		Engine.Profile.OpenChest(Id);

		if (Drops.Count < 1) {
			WorldDialogue.Display("Nothing here!");
			return;
		}

		int totalItems = 0;
		List<string> drops = new();
		Drops.ForEach(lootdrop => {
			drops.Add(
				lootdrop.Quantity > 1
				? $"{lootdrop.Quantity} {lootdrop.ItemData.Name}"
				: lootdrop.ItemData.Name
			);
			Engine.Profile.Inventory.AdjustItem(lootdrop.ItemData, lootdrop.Quantity);
			totalItems += lootdrop.Quantity;
		});
		string term = totalItems > 1 ? "them" : "it";

		WorldDialogue.Display(
				$"You found {String.Join(" and ", drops.ToArray())}.",
				$"You place {term} in your bag."
		);
	}
}

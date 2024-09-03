using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureAction : MonoBehaviour {

	[SerializeField] Engine Engine;
	[SerializeField] PlayerInput PlayerInput;

	[SerializeField] Game.ChestId Id;

	[SerializeField] List<Game.LootDrop> Drops = new();

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
		if (Engine.Profile.Acquired.Has(Id)) {
			StartCoroutine(ShowDialogue("Nothing here!"));
			return;
		}

		Engine.Profile.Acquired.Add(Id);

		if (Drops.Count < 1) {
			StartCoroutine(ShowDialogue("Nothing here!"));
			return;
		}

		int totalItems = 0;
		List<string> drops = new();
		Drops.ForEach(lootdrop => {
			drops.Add(
				lootdrop.Quantity > 1
				? $"{lootdrop.Quantity} {lootdrop.Item.Name}"
				: lootdrop.Item.Name
			);
			Engine.Profile.Inventory.AdjustItem(lootdrop.Item, lootdrop.Quantity);
			totalItems += lootdrop.Quantity;
		});
		string term = totalItems > 1 ? "them" : "it";

		//
		StartCoroutine(
			ShowDialogue(
				$"You found {String.Join(" and ", drops.ToArray())}.",
				$"You place {term} in your bag."
			)
		);
	}

	IEnumerator ShowDialogue(params string[] pages) {
		Engine.Mode = EngineMode.Dialogue;
		yield return Dialogue.Scene.Display(pages);
		Engine.Mode = EngineMode.PlayerControl;
	}
}

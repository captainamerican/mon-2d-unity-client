using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class HarvestAction : MonoBehaviour {

	[SerializeField]
	Engine Engine;

	[SerializeField]
	Sprite Normal;

	[SerializeField]
	Sprite Picked;

	[SerializeField]
	float RespawnTime;

	[SerializeField]
	int Rolls = 1;

	[SerializeField]
	List<Game.WeightedLootDrop> Drops = new();

	SpriteRenderer image;
	bool isBeingTouched;
	bool isPicked;

	void Start() {
		image = GetComponent<SpriteRenderer>();
		image.sprite = Normal;

		isBeingTouched = false;
		isPicked = false;
	}

	void Update() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		if (!isBeingTouched) {
			return;
		}

		if (!Input.GetButtonDown("Submit")) {
			return;
		}

		if (isPicked) {
			StartCoroutine(
				ShowDialogue(
					"Nothing here!",
					"I'll come back later."
				)
			);
			return;
		}

		Harvest();
	}

	IEnumerator ShowDialogue(params string[] pages) {
		Engine.Mode = EngineMode.Dialogue;
		yield return Dialogue.Scene.Display(pages);
		Engine.Mode = EngineMode.PlayerControl;
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

	public void Harvest() {
		isPicked = true;
		image.sprite = Picked;

		int totalItems = 0;
		List<string> drops = new();
		RollForItems().ForEach(lootdrop => {
			Engine.Profile.Acquired.Add(lootdrop.Item);
			Engine.Profile.Seen.Add(lootdrop.Item);

			//
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
				$"You harvested {String.Join(" and ", drops.ToArray())}.",
				$"You place {term} in your bag."
			)
		);
		StartCoroutine(PostHarvest());
	}

	IEnumerator PostHarvest() {
		yield return Wait.For(RespawnTime);

		isPicked = false;
		image.sprite = Normal;
	}


	public List<Game.WeightedLootDrop> RollForItems() {
		int total = Drops.Select(x => x.Weight).Sum();

		//
		return Do.Times(Rolls, () => {
			int random = UnityEngine.Random.Range(0, total);

			for (int j = 0; j < Drops.Count; j++) {
				Game.WeightedLootDrop lootDrop = Drops[j];
				if (random < lootDrop.Weight) {
					return lootDrop;
				}

				random -= lootDrop.Weight;
			}

			//
			return Drops[0];
		});
	}
}

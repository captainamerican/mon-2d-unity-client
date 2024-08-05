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
	List<Item.WeightedLootDrop> Drops = new();

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
			WorldDialogue.Display(
					"Nothing here!",
					"I'll come back later."
			);
			return;
		}

		Harvest();
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
				$"You harvested {String.Join(" and ", drops.ToArray())}.",
				$"You place {term} in your bag."
		);
		StartCoroutine(PostHarvest());
	}

	IEnumerator PostHarvest() {
		yield return Wait.For(RespawnTime);

		isPicked = false;
		image.sprite = Normal;
	}


	public List<Item.WeightedLootDrop> RollForItems() {
		int total = Drops.Select(x => x.Weight).Sum();

		//
		return Do.Times(Rolls, () => {
			int random = UnityEngine.Random.Range(0, total);

			for (int j = 0; j < Drops.Count; j++) {
				Item.WeightedLootDrop lootDrop = Drops[j];
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

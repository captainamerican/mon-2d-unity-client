using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
	List<Item.LootDrop> Drops = new();

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
		if (isPicked) {
			return;
		}

		if (isBeingTouched && Input.GetButtonDown("Submit")) {
			Harvest();
			return;
		}

	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(collision.gameObject);
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		Debug.Log("touch");
		isBeingTouched = true;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (!collision.gameObject.CompareTag("Player")) {
			return;
		}

		Debug.Log("touch2");
		isBeingTouched = false;
	}

	public void Harvest() {
		Debug.Log("Harvest!");

		isPicked = true;
		image.sprite = Picked;

		RollForItems().ForEach(lootdrop => Engine.Inventory.AdjustItem(lootdrop.ItemData, lootdrop.Quantity));
		StartCoroutine(PostHarvest());
	}

	IEnumerator PostHarvest() {
		yield return Wait.For(RespawnTime);

		isPicked = false;
		image.sprite = Normal;
	}


	public List<Item.LootDrop> RollForItems() {
		int total = Drops.Select(x => x.Weight).Sum();

		//
		return Do.Times(Rolls, () => {
			int random = UnityEngine.Random.Range(0, total);

			for (int j = 0; j < Drops.Count; j++) {
				Item.LootDrop lootDrop = Drops[j];
				if (random < lootDrop.Weight) {
					Debug.Log(lootDrop.ItemData.Name + " " + lootDrop.Quantity);
					return lootDrop;
				}

				random -= lootDrop.Weight;
			}

			//
			return Drops[0];
		});
	}
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile", menuName = "MoN/Profile")]
public class Profile : ScriptableObject {
	[Header("Stats")]
	public int Level = 1;
	public int Experience = 0;
	public int ExperienceForNextLevel = 20;

	public int Magic = 30;
	public int Wisdom = 5;

	[Header("Creature")]
	public Party Party = new();
	public List<Creature> Creatures = new();
	public List<Game.LearnedSkill> Skills = new();

	[Header("Inventory")]
	public List<Game.InventoryEntry> Inventory = new();

	public void AdjustItem(Item item, int quantity) {
		Game.InventoryEntry entry = Inventory.Find(e => e.Item == item);
		bool hadEntry = entry != null;

		entry ??= new Game.InventoryEntry() { Item = item };
		entry.Amount = Mathf.Clamp(entry.Amount + quantity, 0, 99);

		if (!hadEntry) {
			Inventory.Add(entry);
		}
	}

	[Header("Chests")]
	public List<ChestId> OpenedChests = new();

	public bool OpenedChest(ChestId chestId) {
		return OpenedChests.Contains(chestId);
	}

	public void OpenChest(ChestId chestId) {
		if (OpenedChest(chestId)) {
			return;
		}

		OpenedChests.Add(chestId);
	}
}
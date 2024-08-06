using System.Collections.Generic;
using Battle;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile", menuName = "MoN/Profile")]
public class Profile : ScriptableObject {
	[Header("Creature")]
	public List<Creature> Party = new();
	public List<Creature> Creatures = new();
	public List<LearnedSkill> Skills = new();

	[Header("Stats")]
	public int Level = 1;
	public int Experience = 0;
	public int ExperienceForNextLevel = 20;

	public int Magic = 30;
	public int Wisdom = 5;

	[Header("Inventory")]
	public List<InventoryEntry> Inventory = new();

	public void AdjustItem(Item.Data itemData, int quantity) {
		InventoryEntry entry = Inventory.Find(e => e.ItemData == itemData);
		bool hadEntry = entry != null;

		entry ??= new InventoryEntry() { ItemData = itemData };
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
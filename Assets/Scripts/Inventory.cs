using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "MoN/Inventory")]
public class Inventory : ScriptableObject {

	public List<InventoryEntry> Entries = new();

	public void AdjustItem(Item.Data itemData, int quantity) {
		InventoryEntry entry = Entries.Find(e => e.ItemData == itemData);
		bool hadEntry = entry != null;

		entry ??= new InventoryEntry() { ItemData = itemData };
		entry.Amount = Mathf.Clamp(entry.Amount + quantity, 0, 99);

		if (!hadEntry) {
			Entries.Add(entry);
		}
	}
}
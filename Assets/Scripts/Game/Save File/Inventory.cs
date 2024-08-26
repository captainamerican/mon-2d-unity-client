using System;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Inventory {

		// -------------------------------------------------------------------------

		public List<InventoryEntry> All = new();

		// -------------------------------------------------------------------------

		public int GetItemQuantity(Item item) {
			return All.Find(e => e.Item == item)?.Amount ?? 0;
		}

		public bool HasItem(Item item) {
			return GetItemQuantity(item) > 0;
		}

		public void AdjustItem(Item item, int quantity) {
			InventoryEntry entry = All.Find(e => e.Item == item);
			bool hadEntry = entry != null;

			entry ??= new InventoryEntry() { Item = item };
			entry.Amount = Mathf.Clamp(entry.Amount + quantity, 0, 999999);

			if (!hadEntry) {
				All.Add(entry);
			}
		}

		// -------------------------------------------------------------------------

	}
}
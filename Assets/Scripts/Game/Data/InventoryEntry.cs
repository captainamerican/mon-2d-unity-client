using System;

namespace Game {
	[Serializable]
	public class InventoryEntry {
		public ItemId ItemId;
		public int Amount;

		public Item Item {
			get {
				return Database.Engine.GameData.Get(ItemId);
			}
		}
	}
}

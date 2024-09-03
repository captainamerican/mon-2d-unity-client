using System;

namespace Game {
	[Serializable]
	public class LootDrop {
		public ItemId ItemId;
		public int Quantity = 1;

		public Item Item {
			get {
				return Database.Engine.GameData.Get(ItemId);
			}
		}
	}
}

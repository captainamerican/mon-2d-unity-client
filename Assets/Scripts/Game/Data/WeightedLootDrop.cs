using System;

namespace Game {
	[Serializable]
	public class WeightedLootDrop {
		public Item Item;
		public int Weight = 100;
		public int Quantity = 1;
	}
}

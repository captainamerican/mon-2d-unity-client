using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class RecipeIngredient {

		// -------------------------------------------------------------------------

		public ItemId ItemId;

		[Range(0, 99)]
		public int Quantity;

		// -------------------------------------------------------------------------

		public Item Item {
			get {
				return Database.Engine.GameData.Get(ItemId);
			}
		}

		// -------------------------------------------------------------------------

	}
}

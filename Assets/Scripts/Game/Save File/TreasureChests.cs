using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class TreasureChests {

		// -------------------------------------------------------------------------

		public List<ChestId> All = new();

		// ------------------------------------------------------------------------- 

		public bool Opened(ChestId chestId) {
			return All.Contains(chestId);
		}

		public void Open(ChestId chestId) {
			if (Opened(chestId)) {
				return;
			}

			//
			All.Add(chestId);
		}

		// -------------------------------------------------------------------------

	}
}
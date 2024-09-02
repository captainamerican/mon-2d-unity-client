using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Spirits {

		// -------------------------------------------------------------------------

		public List<SpiritId> All = new();

		// ------------------------------------------------------------------------- 

		public bool Defeated(SpiritId spiritId) {
			return All.Contains(spiritId);
		}

		public void Defeat(SpiritId spiritId) {
			if (Defeated(spiritId)) {
				return;
			}

			//
			All.Add(spiritId);
		}

		// -------------------------------------------------------------------------

	}
}
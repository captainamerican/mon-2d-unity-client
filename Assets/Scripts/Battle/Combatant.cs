using System.Collections.Generic;
using System.Linq;

namespace Battle {

	public class Combatant {
		public int Health;
		public int HealthTotal;

		public List<CombatantStatus> Statuses = new();

		public bool HasStatus(Status status) {
			return Statuses.Any(cs => cs.Status == status);
		}

		public void AddStatus(Status status, int turns) {
			CombatantStatus combatantStatus = Statuses.Find(cs => cs.Status == status);
			if (combatantStatus != null) {
				Statuses.Remove(combatantStatus);
			}

			//
			Statuses.Add(new CombatantStatus {
				Status = status,
				turns = turns
			});
		}
	}

	public class CombatantStatus {
		public Status Status;
		public int turns = 1;
	}
}

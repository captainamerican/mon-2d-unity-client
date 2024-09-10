using System;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class BodyPartStorage {

		// -------------------------------------------------------------------------

		[Header("Storage")]
		public List<HeadBodyPartEntry> Head = new();
		public List<TorsoBodyPartEntry> Torso = new();
		public List<TailBodyPartEntry> Tail = new();
		public List<AppendageBodyPartEntry> Appendage = new();

		[Header("Reclaimable")]
		public List<HeadBodyPartEntry> ReclaimableHead = new();
		public List<TorsoBodyPartEntry> ReclaimableTorso = new();
		public List<TailBodyPartEntry> ReclaimableTail = new();
		public List<AppendageBodyPartEntry> ReclaimableAppendage = new();

		// -------------------------------------------------------------------------

		public void Add(BodyPartBase bodyPart, float quality = 0, int experience = 0) {
			if (bodyPart is HeadBodyPart head) {
				Add(head, quality, experience);
			} else if (bodyPart is TorsoBodyPart torso) {
				Add(torso, quality, experience);
			} else if (bodyPart is TailBodyPart tail) {
				Add(tail, quality, experience);
			} else if (bodyPart is AppendageBodyPart appendage) {
				Add(appendage, quality, experience);
			}
		}

		public void Add(HeadBodyPart head, float quality = 0, int experience = 0) {
			Add(new HeadBodyPartEntry {
				Id = Id.Generate(),
				BodyPartId = head.Id,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(TorsoBodyPart torso, float quality = 0, int experience = 0) {
			Add(new TorsoBodyPartEntry {
				Id = Id.Generate(),
				BodyPartId = torso.Id,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(TailBodyPart tail, float quality = 0, int experience = 0) {
			Add(new TailBodyPartEntry {
				Id = Id.Generate(),
				BodyPartId = tail.Id,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(AppendageBodyPart appendage, float quality = 0, int experience = 0) {
			Add(new AppendageBodyPartEntry {
				Id = Id.Generate(),
				BodyPartId = appendage.Id,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(BodyPartEntry entry) {
			if (entry is HeadBodyPartEntry head) {
				Add(head);
			} else if (entry is TorsoBodyPartEntry torso) {
				Add(torso);
			} else if (entry is TailBodyPartEntry tail) {
				Add(tail);
			} else if (entry is AppendageBodyPartEntry appendage) {
				Add(appendage);
			}
		}

		public void Add(HeadBodyPartEntry entry) {
			Head.Add(entry);
		}

		public void Add(TorsoBodyPartEntry entry) {
			Torso.Add(entry);
		}

		public void Add(TailBodyPartEntry entry) {
			Tail.Add(entry);
		}

		public void Add(AppendageBodyPartEntry entry) {
			Appendage.Add(entry);
		}

		public void Remove(BodyPartEntry entry) {
			if (entry is HeadBodyPartEntry head) {
				Remove(head);
			} else if (entry is TorsoBodyPartEntry torso) {
				Remove(torso);
			} else if (entry is TailBodyPartEntry tail) {
				Remove(tail);
			} else if (entry is AppendageBodyPartEntry appendage) {
				Remove(appendage);
			}
		}

		public void Remove(HeadBodyPartEntry entry) {
			Head.Remove(entry);
		}

		public void Remove(TorsoBodyPartEntry entry) {
			Torso.Remove(entry);
		}

		public void Remove(TailBodyPartEntry entry) {
			Tail.Remove(entry);
		}

		public void Remove(AppendageBodyPartEntry entry) {
			Appendage.Remove(entry);
		}

		public void AddToReclaimable(BodyPartEntry entry) {
			if (entry is HeadBodyPartEntry head) {
				AddToReclaimable(head);
			} else if (entry is TorsoBodyPartEntry torso) {
				AddToReclaimable(torso);
			} else if (entry is TailBodyPartEntry tail) {
				AddToReclaimable(tail);
			} else if (entry is AppendageBodyPartEntry appendage) {
				AddToReclaimable(appendage);
			}
		}

		public void AddToReclaimable(HeadBodyPartEntry entry) {
			ReclaimableHead.Insert(0, entry);
			PruneReclaimables();
		}

		public void AddToReclaimable(TorsoBodyPartEntry entry) {
			ReclaimableTorso.Insert(0, entry);
			PruneReclaimables();
		}

		public void AddToReclaimable(TailBodyPartEntry entry) {
			ReclaimableTail.Insert(0, entry);
			PruneReclaimables();
		}

		public void AddToReclaimable(AppendageBodyPartEntry entry) {
			ReclaimableAppendage.Insert(0, entry);
			PruneReclaimables();
		}

		public void RemoveFromReclaimable(BodyPartEntry entry) {
			if (entry is HeadBodyPartEntry head) {
				RemoveFromReclaimable(head);
			} else if (entry is TorsoBodyPartEntry torso) {
				RemoveFromReclaimable(torso);
			} else if (entry is TailBodyPartEntry tail) {
				RemoveFromReclaimable(tail);
			} else if (entry is AppendageBodyPartEntry appendage) {
				RemoveFromReclaimable(appendage);
			}
		}

		public void RemoveFromReclaimable(HeadBodyPartEntry entry) {
			ReclaimableHead.Remove(entry);
		}

		public void RemoveFromReclaimable(TorsoBodyPartEntry entry) {
			ReclaimableTorso.Remove(entry);
		}

		public void RemoveFromReclaimable(TailBodyPartEntry entry) {
			ReclaimableTail.Remove(entry);
		}

		public void RemoveFromReclaimable(AppendageBodyPartEntry entry) {
			ReclaimableAppendage.Remove(entry);
		}

		void PruneReclaimables() {
			if (ReclaimableHead.Count > 20) {
				ReclaimableHead.RemoveRange(20, ReclaimableHead.Count - 20);
			}

			if (ReclaimableTorso.Count > 20) {
				ReclaimableTorso.RemoveRange(20, ReclaimableTorso.Count - 20);
			}

			if (ReclaimableTail.Count > 20) {
				ReclaimableTail.RemoveRange(20, ReclaimableTail.Count - 20);
			}

			if (ReclaimableAppendage.Count > 20) {
				ReclaimableAppendage.RemoveRange(20, ReclaimableAppendage.Count - 20);
			}
		}

		// -------------------------------------------------------------------------

	}
}
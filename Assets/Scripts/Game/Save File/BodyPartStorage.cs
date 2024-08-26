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

		public void Add(HeadBodyPart head, float quality = 0, int experience = 0) {
			Head.Add(new HeadBodyPartEntry {
				Id = Id.Generate(),
				BodyPart = head,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(TorsoBodyPart torso, float quality = 0, int experience = 0) {
			Torso.Add(new TorsoBodyPartEntry {
				Id = Id.Generate(),
				BodyPart = torso,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(TailBodyPart tail, float quality = 0, int experience = 0) {
			Tail.Add(new TailBodyPartEntry {
				Id = Id.Generate(),
				BodyPart = tail,
				Quality = quality,
				Experience = experience
			});
		}

		public void Add(AppendageBodyPart appendage, float quality = 0, int experience = 0) {
			Appendage.Add(new AppendageBodyPartEntry {
				Id = Id.Generate(),
				BodyPart = appendage,
				Quality = quality,
				Experience = experience
			});
		}

		public void Remove(BodyPartEntryBase entry) {
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

		public void AddToReclaimable(BodyPartEntryBase entry) {
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
			ReclaimableHead.Add(entry);
		}

		public void AddToReclaimable(TorsoBodyPartEntry entry) {
			ReclaimableTorso.Add(entry);
		}

		public void AddToReclaimable(TailBodyPartEntry entry) {
			ReclaimableTail.Add(entry);
		}

		public void AddToReclaimable(AppendageBodyPartEntry entry) {
			ReclaimableAppendage.Add(entry);
		}

		public void RemoveFromReclaimable(BodyPartEntryBase entry) {
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

		// -------------------------------------------------------------------------

	}
}
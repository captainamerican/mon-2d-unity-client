using System;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class BodyPartStorage {

		// -------------------------------------------------------------------------

		public List<HeadBodyPartEntry> Head = new();
		public List<TorsoBodyPartEntry> Torso = new();
		public List<TailBodyPartEntry> Tail = new();
		public List<AppendageBodyPartEntry> Appendage = new();

		[Header("Reclaimable")]
		public List<BodyPartEntryBase> Reclaimable = new();

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

		// -------------------------------------------------------------------------

	}
}
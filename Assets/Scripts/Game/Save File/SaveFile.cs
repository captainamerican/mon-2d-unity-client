using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class SaveFile {

		// -------------------------------------------------------------------------

		[Header("Stats")]
		public int Level = 1;
		public int Experience = 0;
		public int ExperienceForNextLevel = 20;

		public int Magic = 30;
		public int Wisdom = 5;

		[Header("Data")]
		public List<string> Party = new();
		public List<ConstructedCreature> Creatures = new();
		public List<SkillEntry> Skills = new();
		public Inventory Inventory = new();
		public BodyPartStorage Storage = new();
		public SparringPit SparringPit = new();

		// -------------------------------------------------------------------------

		public int PartyMembersAvailableToFight {
			get {
				return Creatures
					.Where(c => Party.Contains(c.Id) && c.Health > 0)
					.Count();
			}
		}

		public ConstructedCreature GetPartyCreature(int index) {
			if (index > Party.Count - 1) {
				return null;

			}

			//
			string id = Party[index];

			//
			return Creatures.Find(c => c.Id == id);
		}


		// -------------------------------------------------------------------------

		[Header("Spirits")]
		public List<SpiritId> SpiritsDefeated = new();

		public bool DefeatedSpirit(SpiritId spiritId) {
			return SpiritsDefeated.Contains(spiritId);
		}

		public void DefeatSpirit(SpiritId spiritId) {
			if (DefeatedSpirit(spiritId)) {
				return;
			}

			SpiritsDefeated.Add(spiritId);
		}

		// -------------------------------------------------------------------------

		[Header("Chests")]
		public List<ChestId> OpenedChests = new();

		public bool OpenedChest(ChestId chestId) {
			return OpenedChests.Contains(chestId);
		}

		public void OpenChest(ChestId chestId) {
			if (OpenedChest(chestId)) {
				return;
			}

			OpenedChests.Add(chestId);
		}

		// -------------------------------------------------------------------------

	}
}

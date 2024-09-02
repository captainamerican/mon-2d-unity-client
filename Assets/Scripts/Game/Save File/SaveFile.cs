using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class SaveFile {

		// -------------------------------------------------------------------------

		[Header("File")]
		public int FileIndex = -1;
		public bool IsAutoSave = false;

		[Header("Stats")]
		public int Level = 1;
		public int Experience = 0;
		public int ExperienceForNextLevel = 20;

		public int Magic = 30;
		public int Wisdom = 5;
		public float Hunger = 1;

		public float PlaytimeAsSeconds;

		[Header("Data")]
		public List<string> Party = new();
		public List<ConstructedCreature> Creatures = new();
		public List<SkillEntry> Skills = new();
		public Inventory Inventory = new();
		public BodyPartStorage Storage = new();
		public SparringPit SparringPit = new();
		public Options Options = new();

		public List<BodyPartBase> AcquiredBodyPart = new();
		public List<Skill> AcquiredSkills = new();
		public List<Item> AcquiredItem = new();
		public List<SpiritWisdom> AcquiredSpiritWisdom = new();
		public List<Tag> AcquiredTags = new();
		public List<Lore> AcquiredLore = new();

		public List<MapId> TeleportUnlocked = new() {
			MapId.Village
		};

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


		public int MagicTotal {
			get {
				return Mathf.RoundToInt(Mathf.Clamp((float) Wisdom * (5f + ((float) Level / 2f)), 1, 999));
			}
		}

		public string PlayTimeAsString {

			get {
				float time = PlaytimeAsSeconds;

				int hours = Mathf.FloorToInt(time / 3600f);
				int minutes = Mathf.FloorToInt((time - hours) / 60f);
				int seconds = Mathf.FloorToInt(time % 60f);
				int microseconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 1000);

				//
				return Options.Speedrunning
					? $"{hours:d2}:{minutes:d2}:{seconds:d2}.{microseconds:d3}"
					: $"{hours:d2}:{minutes:d2}:{seconds:d2}";
			}
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

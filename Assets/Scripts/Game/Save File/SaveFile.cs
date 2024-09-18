using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class SaveFile {

		// -------------------------------------------------------------------------

		[Header("File")]
		public int FileIndex = -1;
		public bool IsAutoSave = false;
		public DateTime SavedAt;
		public float PlaytimeAsSeconds;

		[Header("Location")]
		public string SceneName;
		public MapId MapId;
		public Vector3 CurrentLocation;

		[Header("Stats")]
		public int Level = 1;
		public int Experience = 0;
		public int ExperienceForNextLevel = 20;

		public int Magic = 30;
		public int Wisdom = 5;
		public float Hunger = 1;

		[Header("Data")]
		public List<string> Party = new();
		public List<Creature> Creatures = new();
		public List<SkillEntry> Skills = new();

		public Options Options = new();
		public Inventory Inventory = new();
		public BodyPartStorage BodyPartStorage = new();
		public SparringPit SparringPit = new();
		public Tally Acquired = new();
		public Tally Seen = new();
		public StoryPoints StoryPoints = new();

		// -------------------------------------------------------------------------

		public int CreaturesAvailableToFight {
			get {
				return Creatures
					.Where(c => Party.Contains(c.Id) && c.Health > 0)
					.Count();
			}
		}

		public Creature GetPartyCreature(int index) {
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

		public void AdjustMagic(int adjustment) {
			Magic = Mathf.RoundToInt(Mathf.Clamp(Magic + adjustment, 0, MagicTotal));
		}

		public void SetPartyMember(int index, string id) {
			if (Party.Contains(id))
				return;
			if (index < 0 || index > 5)
				return;

			if (index >= Party.Count) {
				Party.Add(id);
				return;
			}

			//
			Party[index] = id;
		}

		public void RemovePartyMember(int index) {
			if (index < 0 || index >= Party.Count)
				return;
			if (Party.Count <= 1)
				return;

			//
			Party.RemoveAt(index);
		}

		// -------------------------------------------------------------------------

	}
}

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

		[Header("Creature")]
		public List<string> Party = new();
		public List<ConstructedCreature> Creatures = new();
		public List<LearnedSkill> Skills = new();

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

		[Header("Inventoryies")]
		public List<InventoryEntry> Inventory = new();
		public List<BodyPartEntry> BodyParts = new();

		public int GetItemQuantity(Item item) {
			return Inventory.Find(e => e.Item == item)?.Amount ?? 0;
		}

		public bool HasItem(Item item) {
			return GetItemQuantity(item) > 0;
		}

		public void AddBodyPart(BodyPart bodyPart, float quality = 1) {
			BodyParts.Add(new BodyPartEntry {
				Id = Engine.GenerateRandomId(),
				BodyPart = bodyPart,
				Quality = quality,
				Experience = 0
			});
		}

		public void ReturnBodyPart(BodyPartEntry entry) {
			BodyParts.Remove(entry);
			BodyParts.Add(entry);
		}

		public void AdjustItem(Item item, int quantity) {
			if (item.Type == ItemType.BodyPart) {
				return;
			}

			//
			InventoryEntry entry = Inventory.Find(e => e.Item == item);
			bool hadEntry = entry != null;

			entry ??= new InventoryEntry() { Item = item };
			entry.Amount = Mathf.Clamp(entry.Amount + quantity, 0, 99);

			if (!hadEntry) {
				Inventory.Add(entry);
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

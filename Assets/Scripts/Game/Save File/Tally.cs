using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Tally {

		// ------------------------------------------------------------------------- 

		public List<BodyPartId> BodyPart = new();
		public List<SkillId> Skill = new();
		public List<ItemId> Item = new();
		public List<SpiritWisdomId> SpiritWisdom = new();
		public List<EssenceTagId> Tag = new();
		public List<LoreId> Lore = new();
		public List<ChestId> TreasureChest = new();
		public List<MapId> TeleportLocation = new() {
			MapId.Village
		};

		// -------------------------------------------------------------------------

		public int Total {
			get {
				return BodyPart.Count +
					Skill.Count +
					Item.Count +
					SpiritWisdom.Count +
					Tag.Count +
					Lore.Count +
					TreasureChest.Count
				;
			}
		}

		// -------------------------------------------------------------------------

		public bool Has(BodyPartId bodyPartId) {
			return BodyPart.Contains(bodyPartId);
		}

		public bool Has(SkillId skillId) {
			return Skill.Contains(skillId);
		}

		public bool Has(ItemId itemId) {
			return Item.Contains(itemId);
		}

		public bool Has(SpiritWisdomId spiritId) {
			return SpiritWisdom.Contains(spiritId);
		}

		public bool Has(EssenceTagId tagId) {
			return Tag.Contains(tagId);
		}

		public bool Has(LoreId loreId) {
			return Lore.Contains(loreId);
		}

		public bool Has(ChestId chestId) {
			return TreasureChest.Contains(chestId);
		}

		public bool Has(MapId mapId) {
			return TeleportLocation.Contains(mapId);
		}

		// -------------------------------------------------------------------------

		public void Add(BodyPartId bodyPartId) {
			if (!BodyPart.Contains(bodyPartId)) {
				BodyPart.Add(bodyPartId);
			}
		}

		public void Add(SkillId skillId) {
			if (!Skill.Contains(skillId)) {
				Skill.Add(skillId);
			}
		}

		public void Add(ItemId itemId) {
			if (!Item.Contains(itemId)) {
				Item.Add(itemId);
			}
		}

		public void Add(SpiritWisdomId spiritId) {
			if (!SpiritWisdom.Contains(spiritId)) {
				SpiritWisdom.Add(spiritId);
			}
		}

		public void Add(EssenceTagId tagId) {
			if (!Tag.Contains(tagId)) {
				Tag.Add(tagId);
			}
		}

		public void Add(LoreId loreId) {
			if (!Lore.Contains(loreId)) {
				Lore.Add(loreId);
			}
		}

		public void Add(ChestId chestId) {
			if (!TreasureChest.Contains(chestId)) {
				TreasureChest.Add(chestId);
			}
		}

		public void Add(MapId mapId) {
			if (!TeleportLocation.Contains(mapId)) {
				TeleportLocation.Add(mapId);
			}
		}

		// -------------------------------------------------------------------------

	}
}
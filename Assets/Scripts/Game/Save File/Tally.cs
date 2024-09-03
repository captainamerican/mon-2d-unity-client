using System;
using System.Collections.Generic;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Tally {

		// ------------------------------------------------------------------------- 

		public List<BodyPartBase> BodyPart = new();
		public List<Skill> Skill = new();
		public List<Item> Item = new();
		public List<SpiritWisdom> SpiritWisdom = new();
		public List<EssenceTag> Tag = new();
		public List<Lore> Lore = new();
		public List<ChestId> TreasureChest = new();
		public List<MapId> TeleportLocation = new() {
			MapId.Village
		};
		public List<SpiritId> Spirit = new();

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

		public bool Has(BodyPartBase bodyPartBase) {
			return BodyPart.Contains(bodyPartBase);
		}

		public bool Has(Skill skill) {
			return Skill.Contains(skill);
		}

		public bool Has(Item item) {
			return Item.Contains(item);
		}

		public bool Has(SpiritWisdom item) {
			return SpiritWisdom.Contains(item);
		}

		public bool Has(EssenceTag tag) {
			return Tag.Contains(tag);
		}

		public bool Has(Lore lore) {
			return Lore.Contains(lore);
		}

		public bool Has(ChestId chestId) {
			return TreasureChest.Contains(chestId);
		}

		public bool Has(MapId mapId) {
			return TeleportLocation.Contains(mapId);
		}

		public bool Has(SpiritId spiritId) {
			return Spirit.Contains(spiritId);
		}

		// -------------------------------------------------------------------------

		public void Add(BodyPartBase bodyPartBase) {
			if (!BodyPart.Contains(bodyPartBase)) {
				BodyPart.Add(bodyPartBase);
			}
		}

		public void Add(Skill skill) {
			if (!Skill.Contains(skill)) {
				Skill.Add(skill);
			}
		}

		public void Add(Item item) {
			if (!Item.Contains(item)) {
				Item.Add(item);
			}
		}

		public void Add(SpiritWisdom spiritWisdom) {
			if (!SpiritWisdom.Contains(spiritWisdom)) {
				SpiritWisdom.Add(spiritWisdom);
			}
		}

		public void Add(EssenceTag tag) {
			if (!Tag.Contains(tag)) {
				Tag.Add(tag);
			}
		}

		public void Add(Lore lore) {
			if (!Lore.Contains(lore)) {
				Lore.Add(lore);
			}
		}

		public void Add(ChestId id) {
			if (!TreasureChest.Contains(id)) {
				TreasureChest.Add(id);
			}
		}

		public void Add(MapId mapId) {
			if (!TeleportLocation.Contains(mapId)) {
				TeleportLocation.Add(mapId);
			}
		}

		public void Add(SpiritId spiritId) {
			if (!Spirit.Contains(spiritId)) {
				Spirit.Add(spiritId);
			}
		}

		// -------------------------------------------------------------------------

	}
}
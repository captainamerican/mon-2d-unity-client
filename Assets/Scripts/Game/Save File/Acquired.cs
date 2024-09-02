using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Acquired {

		// ------------------------------------------------------------------------- 

		public List<BodyPartBase> BodyPart;
		public List<Skill> Skill;
		public List<Item> Item;
		public List<SpiritWisdom> SpiritWisdom;
		public List<Tag> Tag;
		public List<Lore> Lore;

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

		public bool Has(Tag tag) {
			return Tag.Contains(tag);
		}

		public bool Has(Lore lore) {
			return Lore.Contains(lore);
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

		public void Add(Tag tag) {
			if (!Tag.Contains(tag)) {
				Tag.Add(tag);
			}
		}

		public void Add(Lore lore) {
			if (!Lore.Contains(lore)) {
				Lore.Add(lore);
			}
		}

		// -------------------------------------------------------------------------

	}
}
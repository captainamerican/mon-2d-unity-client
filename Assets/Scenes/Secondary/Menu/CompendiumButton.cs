using UnityEngine;

namespace Menu {
	public enum CompendiumButtonType {
		None,
		BodyPart,
		Skill,
		Item,
		SpiritWisdom,
		Gameplay,
		Tag
	}

	public class CompendiumButton : MonoBehaviour {
		public CompendiumButtonType Type;
		public BodyPartBase BodyPart;
		public Skill Skill;
		public Item Item;
		public SpiritWisdom SpiritWisdom;
		public Gameplay Gameplay;
		public Tag Tag;
	}
}
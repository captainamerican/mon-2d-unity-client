using System.Collections.Generic;

using Game;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Game Data", menuName = "MoN/Game Data")]
public class GameData : ScriptableObject {

	// ---------------------------------------------------------------------------

	public List<Item> CraftingEquipment;
	public List<Item> Items;
	public List<BodyPartBase> BodyParts;
	public List<Skill> Skills;
	public List<SpiritWisdom> SpiritWisdom;
	public List<EssenceTag> Tags;
	public List<Lore> Lore;

	[Header("\"Static\"")]
	public List<Gameplay> Gameplay;

	// ---------------------------------------------------------------------------

	readonly Dictionary<ItemId, Item> craftingEquipmentById = new();
	readonly Dictionary<ItemId, Item> itemsById = new();
	readonly Dictionary<BodyPartId, BodyPartBase> bodyPartsById = new();
	readonly Dictionary<SkillId, Skill> skillsById = new();
	readonly Dictionary<SpiritWisdomId, SpiritWisdom> spiritWisdomById = new();
	readonly Dictionary<EssenceTagId, EssenceTag> tagsById = new();
	readonly Dictionary<LoreId, Lore> loreById = new();

	// --------------------------------------------------------------------------- 

	public void BuildDictionaries() {
		craftingEquipmentById.Clear();
		itemsById.Clear();
		bodyPartsById.Clear();
		skillsById.Clear();
		spiritWisdomById.Clear();
		tagsById.Clear();
		loreById.Clear();

		//
		CraftingEquipment.ForEach(item => craftingEquipmentById[item.Id] = item);
		Items.ForEach(item => itemsById[item.Id] = item);
		BodyParts.ForEach(bodyPart => bodyPartsById[bodyPart.Id] = bodyPart);
		Skills.ForEach(skill => skillsById[skill.Id] = skill);
		SpiritWisdom.ForEach(spiritWisdom => spiritWisdomById[spiritWisdom.Id] = spiritWisdom);
		Tags.ForEach(tag => tagsById[tag.Id] = tag);
	}

	// ---------------------------------------------------------------------------

	public int Total {
		get {
			return Items.Count +
				BodyParts.Count +
				Skills.Count +
				SpiritWisdom.Count +
				Tags.Count +
				Lore.Count
			;
		}
	}

	public Item Get(ItemId id) {
		return itemsById[id];
	}

	public T Get<T>(BodyPartId id) where T : BodyPartBase {
		return (T) bodyPartsById[id];
	}

	public Skill Get(SkillId id) {
		return skillsById[id];
	}

	public SpiritWisdom Get(SpiritWisdomId id) {
		return spiritWisdomById[id];
	}

	// ---------------------------------------------------------------------------

}
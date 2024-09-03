using System.Collections.Generic;

using Game;

using UnityEngine;

// -----------------------------------------------------------------------------

[CreateAssetMenu(fileName = "Game Data", menuName = "MoN/Game Data")]
public class GameData : ScriptableObject {

	// ---------------------------------------------------------------------------

	public List<Item> CraftingEquipment = new();
	public List<Item> Items = new();
	public List<BodyPartBase> BodyParts = new();
	public List<Skill> Skills = new();
	public List<SpiritWisdom> SpiritWisdom = new();
	public List<EssenceTag> Tags = new();
	public List<Lore> Lore = new();

	[Header("\"Static\"")]
	public List<Gameplay> Gameplay = new();

	// ---------------------------------------------------------------------------

	Dictionary<ItemId, Item> craftingEquipmentById = new();
	Dictionary<ItemId, Item> itemsById = new();
	Dictionary<BodyPartId, BodyPartBase> bodyPartsById = new();
	Dictionary<SkillId, Skill> skillsById = new();
	Dictionary<string, SpiritWisdom> spiritWisdombyId = new();
	Dictionary<EssenceTagId, EssenceTag> tagsById = new();
	Dictionary<string, Lore> loreById = new();

	// ---------------------------------------------------------------------------

	public void BuildDictionaries() {
		craftingEquipmentById.Clear();
		itemsById.Clear();
		bodyPartsById.Clear();
		skillsById.Clear();
		spiritWisdombyId.Clear();
		tagsById.Clear();
		loreById.Clear();

		//
		CraftingEquipment.ForEach(item => craftingEquipmentById[item.Id] = item);
		Items.ForEach(item => itemsById[item.Id] = item);
		BodyParts.ForEach(bodyPart => bodyPartsById[bodyPart.Id] = bodyPart);
		Skills.ForEach(skill => skillsById[skill.Id] = skill);
		Tags.ForEach(tag => tagsById[tag.Id] = tag);
	}

	// ---------------------------------------------------------------------------

	public Item Get(ItemId id) {
		return itemsById[id];
	}

	public T Get<T>(BodyPartId id) where T : BodyPartBase {
		return (T) bodyPartsById[id];
	}

	public Skill Get(SkillId id) {
		return skillsById[id];
	}

	// ---------------------------------------------------------------------------

}
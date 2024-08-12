using System.Collections.Generic;

using Game;

using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MoN/Item")]
public class Item : ScriptableObject {
	[Header("Important")]
	public Game.ItemType Type;
	public bool UseInBattle;

	[Header("Information")]
	public string Name;
	public string Description;
	public string FlavorText;

	public List<RecipeIngredient> Recipe = new();

	[Header("Data")]
	public List<Game.ApplicableTarget> Targets = new();
	public List<Game.Effect> Effects = new();

	[Header("FX")]
	public List<Game.SkillFX> FX = new();

	static public string TypeName(Game.ItemType type) {
		switch (type) {
			case Game.ItemType.None:
				return "None";

			case Game.ItemType.Consumable:
				return "Consumable";

			case Game.ItemType.Reusable:
				return "Reusable";

			case Game.ItemType.CraftingMaterial:
				return "Crafting Material";

			case Game.ItemType.TrainingItem:
				return "Training Item";

			case Game.ItemType.BodyPart:
				return "BodyPart";

			case Game.ItemType.KeyItem:
				return "KeyItem";
		}

		return "Unknown";
	}
}
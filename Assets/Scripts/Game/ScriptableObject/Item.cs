using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MoN/Item")]
public class Item : ScriptableObject {
	[Header("Important")]
	public Game.ItemId Id;
	public Game.ItemType Type;


	public bool UseInBattle;

	[Header("Information")]
	public string Name;
	public string Description;
	public string FlavorText;

	public string SortName;

	public List<Game.RecipeIngredient> Recipe = new();

	[Header("Data")]
	public List<Game.ApplicableTarget> Targets = new();
	public List<Game.Effect> Effects = new();

	[Header("FX")]
	public List<Game.SkillFX> FX = new();

	static public string TypeName(Game.ItemType type) {
		return type switch {
			Game.ItemType.None => "None",
			Game.ItemType.Consumable => "Consumable",
			Game.ItemType.Reusable => "Reusable",
			Game.ItemType.CraftingMaterial => "Crafting Material",
			Game.ItemType.TrainingItem => "Training Item",
			Game.ItemType.KeyItem => "Key Item",
			_ => "Unknown",
		};
	}
}
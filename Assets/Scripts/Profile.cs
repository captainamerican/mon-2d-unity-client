using System.Collections.Generic;
using Battle;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile", menuName = "MoN/Profile")]
public class Profile : ScriptableObject {
	public Inventory Inventory;

	[Header("Creature")]
	public List<Creature> Party = new();
	public List<Creature> Creatures = new();
	public List<LearnedSkill> Skills = new();

	[Header("Stats")]
	public int Level = 1;
	public int Experience = 0;
	public int ExperienceForNextLevel = 20;

	public int Magic = 50;
	public int MagicTotal = 50;

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
}
using System.Collections.Generic;
using Battle;
using UnityEngine;



[CreateAssetMenu(fileName = "Creature Base", menuName = "MoN/Creature Base")]
public class CreatureBase : ScriptableObject {
	[Header("Information")]
	public string Name;
	public string Description;

	[Header("Statistics")]
	public Game.NumberOfAppendages Appendages;
	public int Strength = 1;
	public int Endurance = 1;
	public int Dexterity = 1;
	public int Intelligence = 1;
	public int Wisdom = 1;
	public int Luck = 1;

	[Header("Skills")]
	public List<Skill> InnateSkills = new();

}
using System;
using System.Collections.Generic;

using UnityEngine;

// ---------------------------------------------------------------------------

[Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "MoN/Skill")]
public class Skill : ScriptableObject {

	// --------------------------------------------------------------------------- 

	public Game.SkillId Id;
	public string Slug;
	public string Name;

	[TextArea(2, 10)]
	public string Description;

	[Header("Stats")]
	public int Cost = 1;
	public int ExperienceToLearn = 100;

	public List<Game.Effect> Effect = new();
	public List<Game.EssenceTagId> Tags = new();
	public List<Game.SkillFX> FX = new();

	// ---------------------------------------------------------------------------

}
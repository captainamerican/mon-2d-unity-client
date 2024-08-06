using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Battle {
	public enum Tag {
		None,
		Claw,
		Teeth,

	}

	[CreateAssetMenu(fileName = "Skill", menuName = "MoN/Skill")]
	public class Skill : ScriptableObject {
		public string Name;

		[TextArea(2, 10)]
		public string Description;

		[Header("Stats")]
		public int Cost = 1;

		public List<Effect> Effect = new();

		[Header("FX")]
		public List<SkillFX> FX = new();
	}

	[Serializable]
	public class LearnedSkill {
		public Skill Skill;
		public float Experience = 0;
	}

	[Serializable]
	public class SkillFX {
		public bool Actor;
		public float Delay;
	}
}
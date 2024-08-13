using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game {
	[Serializable]
	public class ConstructedCreature {
		public string Id;
		public string Name;

		[Header("Live Stats")]
		public int Health;

		public List<Skill> Skills = new();

		[Header("Appendages")]
		public BodyPart Head;
		public BodyPart Torso;
		public BodyPart Tail;

		public BodyPart LeftFrontAppendage;
		public BodyPart LeftMiddleAppendage;
		public BodyPart LeftRearAppendage;

		public BodyPart RightFrontAppendage;
		public BodyPart RightMiddleAppendage;
		public BodyPart RightRearAppendage;
	}

}
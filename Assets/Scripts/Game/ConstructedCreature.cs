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
		public BodyPartEntry Head;
		public BodyPartEntry Torso;
		public BodyPartEntry Tail;

		public BodyPartEntry LeftFrontAppendage;
		public BodyPartEntry LeftMiddleAppendage;
		public BodyPartEntry LeftRearAppendage;

		public BodyPartEntry RightFrontAppendage;
		public BodyPartEntry RightMiddleAppendage;
		public BodyPartEntry RightRearAppendage;

		public string AppendagesLabel() {
			switch (Torso.BodyPart.Base.Appendages) {
				case NumberOfAppendages.OneLowerNoUpper:
				case NumberOfAppendages.OneLowerTwoUpper:
					return "Uniped";

				case NumberOfAppendages.TwoLowerNoUpper:
				case NumberOfAppendages.TwoLowerTwoUpper:
					return "Biped";

				case NumberOfAppendages.FourLower:
				case NumberOfAppendages.FourLowerTwoUpper:
					return "Quadraped";

				case NumberOfAppendages.SixLower:
					return "Sexaped";
			}

			//
			return "Nulped";
		}

		public Skill GetSkillAt(int index) {
			return (Skills.Count > index) ? Skills[index] : null;
		}

		public ConstructedCreature Clone() {
			return new ConstructedCreature {
				Id = Id,
				Name = Name,
				Health = Health,
				Skills = new List<Skill>(Skills),
				Head = Head,
				Torso = Torso,
				Tail = Tail,
				LeftFrontAppendage = LeftFrontAppendage,
				LeftMiddleAppendage = LeftMiddleAppendage,
				LeftRearAppendage = LeftRearAppendage,
				RightFrontAppendage = RightFrontAppendage,
				RightMiddleAppendage = RightMiddleAppendage,
				RightRearAppendage = RightRearAppendage
			};
		}
	}
}
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

		public bool MissingHead {
			get {
				return Head?.BodyPart == null;
			}
		}

		public int HeadMaxSkills {
			get {
				if (MissingHead) {
					return 0;
				}

				//
				int experience = Head.Experience;
				int toLevel = Head.BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
				int level = Mathf.FloorToInt(rawLevel);

				//
				return 1 + level;
			}
		}

		public bool IsComplete {
			get {
				if (Skills.Count < 1) {
					return false;
				}

				if (Head?.BodyPart == null || Torso?.BodyPart == null) {
					return false;
				}

				switch (Torso.BodyPart.Base.Appendages) {
					case NumberOfAppendages.OneLowerNoUpper:
						if (RightMiddleAppendage?.BodyPart == null) {
							return false;
						}
						break;

					case NumberOfAppendages.OneLowerTwoUpper:
						if (
							LeftFrontAppendage?.BodyPart == null ||
							RightFrontAppendage?.BodyPart == null ||
							RightMiddleAppendage?.BodyPart == null
						) {
							return false;
						}
						break;

					case NumberOfAppendages.TwoLowerNoUpper:
						if (
							LeftRearAppendage?.BodyPart == null ||
							RightRearAppendage?.BodyPart == null
						) {
							return false;
						}
						break;

					case NumberOfAppendages.TwoLowerTwoUpper:
						if (
							LeftFrontAppendage?.BodyPart == null ||
							RightFrontAppendage?.BodyPart == null ||
							LeftRearAppendage?.BodyPart == null ||
							RightRearAppendage?.BodyPart == null
						) {
							return false;
						}
						break;

					case NumberOfAppendages.FourLower:
						if (
							LeftFrontAppendage?.BodyPart == null ||
							RightFrontAppendage?.BodyPart == null ||
							LeftRearAppendage?.BodyPart == null ||
							RightRearAppendage?.BodyPart == null
						) {
							return false;
						}
						break;

					case NumberOfAppendages.FourLowerTwoUpper:
						if (
							LeftFrontAppendage?.BodyPart == null ||
							RightFrontAppendage?.BodyPart == null ||
							LeftMiddleAppendage?.BodyPart == null ||
							RightMiddleAppendage?.BodyPart == null ||
							LeftRearAppendage?.BodyPart == null ||
							RightRearAppendage?.BodyPart == null
						) {
							return false;
						}
						break;

					case NumberOfAppendages.SixLower:
						if (
							LeftFrontAppendage?.BodyPart == null ||
							RightFrontAppendage?.BodyPart == null ||
							LeftMiddleAppendage?.BodyPart == null ||
							RightMiddleAppendage?.BodyPart == null ||
							LeftRearAppendage?.BodyPart == null ||
							RightRearAppendage?.BodyPart == null
						) {
							return false;
						}
						break;
				}

				//
				return true;
			}
		}

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
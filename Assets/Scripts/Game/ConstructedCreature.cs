using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class ConstructedCreature {

		// -------------------------------------------------------------------------

		public string Id;
		public string Name;

		[Header("Live Stats")]
		public int Health;

		[Header("Skills")]
		public List<Skill> Skills = new();

		[Header("Appendages")]
		public BodyPartEntry Head;
		public BodyPartEntry Torso;
		public BodyPartEntry Tail;
		public List<BodyPartEntry> Appendages = new();

		// -------------------------------------------------------------------------

		Dictionary<NumberOfAppendages, int> AppendageCount = new() {
			{
				NumberOfAppendages.None,
				0
			},
			{
				NumberOfAppendages.OneLowerNoUpper,
				1
			},
			{
				NumberOfAppendages.TwoLowerNoUpper,
				2
			},
			{
				NumberOfAppendages.NoLowerTwoUpper,
				2
			},
			{
				NumberOfAppendages.OneLowerTwoUpper,
				3
			},
			{
				NumberOfAppendages.TwoLowerTwoUpper,
				4
			},
			{
				NumberOfAppendages.FourLower,
				4
			},
			{
				NumberOfAppendages.FourLowerTwoUpper,
				6
			},
			{
				NumberOfAppendages.SixLower,
				6
			}
		};

		Dictionary<NumberOfAppendages, List<string>> NameOfAppendages = new() {
			{
				NumberOfAppendages.None,
				new() {
				}
			},
			{
				NumberOfAppendages.OneLowerNoUpper,
				new() {
					"Lower",
				}
			},
			{
				NumberOfAppendages.TwoLowerNoUpper,
				new() {
					"L. Leg",
					"R. Leg"
				}
			},
			{
				NumberOfAppendages.NoLowerTwoUpper,
				new() {
					"L. Arm",
					"R. Arm"
				}
			},
			{
				NumberOfAppendages.OneLowerTwoUpper,
				new() {
					"L. Arm",
					"R. Arm",
					"Lower",
				}
			},
			{
				NumberOfAppendages.TwoLowerTwoUpper,
				new() {
					"L. Arm",
					"R. Arm",
					"L. Leg",
					"R. Leg"
				}
			},
			{
				NumberOfAppendages.FourLower,
				new() {
					"L. Front Leg",
					"R. Front Leg",
					"L. Rear Leg",
					"R. Rear Leg"
				}
			},
			{
				NumberOfAppendages.FourLowerTwoUpper,
				new() {
					"L. Arm",
					"R. Arm",
					"L. Front Leg",
					"R. Front Leg",
					"L. Rear Leg",
					"R. Rear Leg"
				}
			},
			{
				NumberOfAppendages.SixLower,
				new() {
					"L. Front Leg",
					"R. Front Leg",
					"L. Middle Leg",
					"R. Middle Leg",
					"L. Rear Leg",
					"R. Rear Leg"
				}
			}
		};

		// -------------------------------------------------------------------------

		public string NameOfAppendage(int index) {
			List<string> names = NameOfAppendages[
				Torso?.BodyPart?.Base?.Appendages
				?? NumberOfAppendages.None
			];

			//
			return index < names.Count ? names[index] : "???";
		}

		public int HowManyAppendages {
			get {
				return AppendageCount[
					Torso?.BodyPart?.Base?.Appendages
					?? NumberOfAppendages.None
				];
			}

		}

		public bool MissingHead {
			get {
				return Head?.BodyPart == null;
			}
		}

		public bool MissingTorso {
			get {
				return Torso?.BodyPart == null;
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
				if (
					Skills.Count < 1 ||
					Head?.BodyPart == null ||
					Torso?.BodyPart == null
				) {
					return false;
				}

				//
				int appendages = AppendageCount[Torso.BodyPart.Base.Appendages];
				return Appendages.Sum(a => a?.BodyPart == null ? 0 : 1) >= appendages;
			}
		}

		public string AppendagesLabel() {
			switch (Torso.BodyPart.Base.Appendages) {
				case NumberOfAppendages.NoLowerTwoUpper:
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

		public BodyPartEntry GetAppendage(int i) {
			return i < (Appendages?.Count ?? 0) ? Appendages[i] : null;
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
				Appendages = Appendages
			};
		}

		// -------------------------------------------------------------------------

	}
}
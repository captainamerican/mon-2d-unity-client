using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class ConstructedCreature {

		// -------------------------------------------------------------------------

		public string Id = Game.Id.Generate();
		public string Name = "";

		[Header("Live Stats")]
		public int Health;

		[Header("Appendages")]
		public HeadBodyPartEntry Head;
		public TorsoBodyPartEntry Torso;
		public TailBodyPartEntry Tail;
		public List<AppendageBodyPartEntry> Appendages = new();

		[Header("Skills")]
		public List<Skill> Skills = new();

		// ------------------------------------------------------------------------- 

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

		public bool IsComplete {
			get {
				return HasSetName && HasAllRequiredBodyParts && HasAtLeastOneSkill;
			}
		}
		public bool HasSetName {
			get {
				return Name.Trim() != "";
			}
		}

		public bool HasAtLeastOneSkill {
			get {
				return Skills.Count > 0;
			}
		}

		public bool HasAllRequiredBodyParts {
			get {
				if (
					Head?.BodyPart == null ||
					Torso?.BodyPart == null
				) {
					return false;

				}

				//
				int appendages = Torso.BodyPart.HowManyAppendages;
				return Appendages.Sum(a => a?.BodyPart == null ? 0 : 1) >= appendages;
			}
		}

		public Skill GetSkillAt(int index) {
			return (Skills.Count > index) ? Skills[index] : null;
		}

		public AppendageBodyPartEntry GetAppendage(int i) {
			return i < (Appendages?.Count ?? 0) ? Appendages[i] : null;
		}

		public string NameOfAppendage(int index) {
			return MissingTorso
				? "???"
				: AppendageBodyPart.Label(
					Torso.BodyPart.TypeOfAppendages,
					index
				);
		}

		public ConstructedCreature Clone() {
			return new ConstructedCreature {
				Id = Id,
				Name = Name,
				Health = Health,
				Head = Head,
				Torso = Torso,
				Tail = Tail,
				Appendages = new List<AppendageBodyPartEntry>(Appendages),
				Skills = new List<Skill>(Skills)
			};
		}

		// -------------------------------------------------------------------------

	}
}
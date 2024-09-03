using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Creature {

		// -------------------------------------------------------------------------

		public string Id = Game.Id.Generate();
		public string Name = "";

		[Header("Live Stats")]
		public int Health;

		[Header("Body Parts")]
		public HeadBodyPartEntry Head;
		public TorsoBodyPartEntry Torso;
		public TailBodyPartEntry Tail;
		public List<AppendageBodyPartEntry> Appendages = new();

		[Header("Skills")]
		public List<SkillId> Skills = new();

		// ------------------------------------------------------------------------- 

		public bool MissingHead {
			get {
				return (Head?.BodyPartId ?? BodyPartId.None) == BodyPartId.None;
			}
		}

		public bool MissingTorso {
			get {
				return (Torso?.BodyPartId ?? BodyPartId.None) == BodyPartId.None;
			}
		}

		public bool IsComplete {
			get {
				return HasSetName && HasAllRequiredBodyParts && HasAtLeastOneSkill;
			}
		}
		public bool HasSetName {
			get {
				return (Name?.Trim() ?? "") != "";
			}
		}

		public bool HasAtLeastOneSkill {
			get {
				return Skills.Count > 0;
			}
		}

		public bool HasAllRequiredBodyParts {
			get {
				if (MissingHead || MissingTorso) {
					return false;

				}

				//
				int appendages = Torso.BodyPart.HowManyAppendages;
				return Appendages.Sum(
					a => (a?.BodyPartId ?? BodyPartId.None) == BodyPartId.None ? 0 : 1
				) >= appendages;
			}
		}

		public Skill GetSkill(int index) {
			return (Skills.Count > index)
				? Database.Engine.GameData.Get(Skills[index])
				: null;
		}

		public AppendageBodyPartEntry GetAppendage(int i) {
			return i < Appendages.Count
				? Appendages[i]
				: null;
		}

		public string NameOfAppendage(int index) {
			return MissingTorso
				? "???"
				: AppendageBodyPart.Label(
					Torso.BodyPart.TypeOfAppendages,
					index
				);
		}

		public Creature Clone() {
			return new Creature {
				Id = Id,
				Name = Name,
				Health = Health,
				Head = Head,
				Torso = Torso,
				Tail = Tail,
				Appendages = new List<AppendageBodyPartEntry>(Appendages),
				Skills = new List<SkillId>(Skills)
			};
		}

		// -------------------------------------------------------------------------

	}
}
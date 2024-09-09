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

		[Header("Body Parts")]
		public HeadBodyPartEntry Head;
		public TorsoBodyPartEntry Torso;
		public TailBodyPartEntry Tail;
		public List<AppendageBodyPartEntry> Appendages = new();

		[Header("Skills")]
		public List<SkillId> Skills = new();

		[Header("Live Stats")]
		[SerializeField] int CurrentHealth;
		public List<CombatantStatus> Statuses = new();

		// -------------------------------------------------------------------------

		[NonSerialized] List<int> attributes = new() { 1, 1, 1, 1, 1, 1 };
		[NonSerialized] List<int> attributeAdjustments = new() { 0, 0, 0, 0, 0, 0 };

		// -------------------------------------------------------------------------

		public bool MissingBodyPart(BodyPartId? id) {
			return (id ?? BodyPartId.None) == BodyPartId.None;
		}

		public bool MissingHead {
			get {
				return MissingBodyPart(Head?.BodyPartId);
			}
		}

		public bool MissingTorso {
			get {
				return MissingBodyPart(Torso?.BodyPartId);
			}
		}

		public bool MissingTail {
			get {
				return MissingBodyPart(Tail?.BodyPartId);
			}
		}

		public bool MissingAppendage(int index) {
			return MissingBodyPart(GetAppendage(index)?.BodyPartId);
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
					a => MissingBodyPart(a?.BodyPartId)
						? 0
						: 1
				) >= appendages;
			}
		}

		public Skill GetSkill(int index) {
			return (Skills.Count > index)
				? Database.Engine.GameData.Get(Skills[index])
				: null;
		}

		public AppendageBodyPartEntry GetAppendage(int index) {
			return index < Appendages.Count
				? Appendages[index]
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
				CurrentHealth = CurrentHealth,
				Head = Head,
				Torso = Torso,
				Tail = Tail,
				Appendages = new(Appendages),
				Skills = new(Skills),
				Statuses = new(Statuses)
			};
		}

		// Combat
		// -------------------------------------------------------------------------

		public int Health {
			get {
				return Mathf.Clamp(CurrentHealth, 0, HealthTotal);
			}

			set {
				CurrentHealth = Mathf.Clamp(value, 0, HealthTotal);
			}
		}

		public int HealthTotal {
			get {
				return 99;
			}
		}

		public int Strength {
			get {
				return attributes[0] + attributeAdjustments[0];
			}
		}

		public int Endurance {
			get {
				return attributes[1] + attributeAdjustments[1];
			}
		}

		public int Dexterity {
			get {
				return attributes[2] + attributeAdjustments[2];
			}
		}

		public int Intelligence {
			get {
				return attributes[3] + attributeAdjustments[3];
			}
		}

		public int Wisdom {
			get {
				return attributes[4] + attributeAdjustments[4];
			}
		}

		public int Luck {
			get {
				return attributes[5] + attributeAdjustments[5];
			}
		}

		public int Experience {
			get {
				return attributes.Sum();
			}
		}

		public void PrepareForBattle() {
			CalculateAttributes();
			CalculateAttributeAdjustments();
		}

		public void CalculateAttributes() {
			attributes[0] = 1;
			attributes[1] = 1;
			attributes[2] = 1;
			attributes[3] = 1;
			attributes[4] = 1;
			attributes[5] = 1;

			//
			CalculateBodyPartAttributes(Head);
			CalculateBodyPartAttributes(Torso);
			CalculateBodyPartAttributes(Tail);
			Appendages.ForEach(CalculateBodyPartAttributes);
		}

		public void AdvanceStatuses() {
			Statuses.ForEach(status => status.Turns -= 1);
			Statuses.RemoveAll(status => status.Turns < 1);

			//
			CalculateAttributeAdjustments();
		}

		public bool HasStatus(Status status) {
			return Statuses.Any(cs => cs.Status == status);
		}

		public void AddStatus(Status status, int turns, float strength) {
			CombatantStatus combatantStatus = Statuses.Find(cs => cs.Status == status);
			if (combatantStatus != null) {
				Statuses.Remove(combatantStatus);
			}

			//
			Statuses.Add(new CombatantStatus {
				Status = status,
				Turns = turns,
				Strength = strength
			});
		}

		public void AdjustHealth(int amount) {
			CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, HealthTotal);
		}

		// -------------------------------------------------------------------------

		void CalculateBodyPartAttributes(HeadBodyPartEntry head) {
			if ((head?.BodyPartId ?? BodyPartId.None) == BodyPartId.None) {
				return;
			}

			//
			CalculateBodyPartAttribute(0, head.BodyPart.Strength, head.Quality);
			CalculateBodyPartAttribute(1, head.BodyPart.Endurance, head.Quality);
			CalculateBodyPartAttribute(2, head.BodyPart.Dexterity, head.Quality);
			CalculateBodyPartAttribute(3, head.BodyPart.Intelligence, head.Quality);
			CalculateBodyPartAttribute(4, head.BodyPart.Wisdom, head.Quality);
			CalculateBodyPartAttribute(5, head.BodyPart.Luck, head.Quality);
		}

		void CalculateBodyPartAttributes(TorsoBodyPartEntry torso) {
			if ((torso?.BodyPartId ?? BodyPartId.None) == BodyPartId.None) {
				return;
			}

			//
			CalculateBodyPartAttribute(0, torso.BodyPart.Strength, torso.Quality);
			CalculateBodyPartAttribute(1, torso.BodyPart.Endurance, torso.Quality);
			CalculateBodyPartAttribute(2, torso.BodyPart.Dexterity, torso.Quality);
			CalculateBodyPartAttribute(3, torso.BodyPart.Intelligence, torso.Quality);
			CalculateBodyPartAttribute(4, torso.BodyPart.Wisdom, torso.Quality);
			CalculateBodyPartAttribute(5, torso.BodyPart.Luck, torso.Quality);
		}

		void CalculateBodyPartAttributes(TailBodyPartEntry tail) {
			if ((tail?.BodyPartId ?? BodyPartId.None) == BodyPartId.None) {
				return;
			}

			//
			CalculateBodyPartAttribute(0, tail.BodyPart.Strength, tail.Quality);
			CalculateBodyPartAttribute(1, tail.BodyPart.Endurance, tail.Quality);
			CalculateBodyPartAttribute(2, tail.BodyPart.Dexterity, tail.Quality);
			CalculateBodyPartAttribute(3, tail.BodyPart.Intelligence, tail.Quality);
			CalculateBodyPartAttribute(4, tail.BodyPart.Wisdom, tail.Quality);
			CalculateBodyPartAttribute(5, tail.BodyPart.Luck, tail.Quality);
		}

		void CalculateBodyPartAttributes(AppendageBodyPartEntry appendage) {
			if ((appendage?.BodyPartId ?? BodyPartId.None) == BodyPartId.None) {
				return;
			}

			//
			CalculateBodyPartAttribute(0, appendage.BodyPart.Strength, appendage.Quality);
			CalculateBodyPartAttribute(1, appendage.BodyPart.Endurance, appendage.Quality);
			CalculateBodyPartAttribute(2, appendage.BodyPart.Dexterity, appendage.Quality);
			CalculateBodyPartAttribute(3, appendage.BodyPart.Intelligence, appendage.Quality);
			CalculateBodyPartAttribute(4, appendage.BodyPart.Wisdom, appendage.Quality);
			CalculateBodyPartAttribute(5, appendage.BodyPart.Luck, appendage.Quality);
		}

		void CalculateBodyPartAttribute(int index, int value, float quality) {
			attributes[index] = (int) Mathf.Clamp((float) Math.Round(value * quality), 1, 999);
		}

		void CalculateAttributeAdjustments() {
			attributeAdjustments[0] = 0;
			attributeAdjustments[1] = 0;
			attributeAdjustments[2] = 0;
			attributeAdjustments[3] = 0;
			attributeAdjustments[4] = 0;
			attributeAdjustments[5] = 0;

			//
			Statuses.ForEach(status => {
				switch (status.Status) {
					case Status.StrengthBuff:
					case Status.StrengthDebuff:
						attributeAdjustments[0] += (int) Math.Round(status.Strength);
						break;
					case Status.EnduranceBuff:
					case Status.EnduranceDebuff:
						attributeAdjustments[1] += (int) Math.Round(status.Strength);
						break;
					case Status.DexterityBuff:
					case Status.DexterityDebuff:
						attributeAdjustments[2] += (int) Math.Round(status.Strength);
						break;
					case Status.IntelligenceBuff:
					case Status.IntelligenceDebuff:
						attributeAdjustments[3] += (int) Math.Round(status.Strength);
						break;
					case Status.WisdomBuff:
					case Status.WisdomDebuff:
						attributeAdjustments[4] += (int) Math.Round(status.Strength);
						break;
					case Status.LuckBuff:
					case Status.LuckDebuff:
						attributeAdjustments[5] += (int) Math.Round(status.Strength);
						break;
				}
			});
		}

		// -------------------------------------------------------------------------

	}
}
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using S = UnityEngine.SerializeField;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Creature {

		// -------------------------------------------------------------------------

		public string Id = Game.Id.Generate();
		public string Name = "";

		[Header("Live Stats")]
		public float Adjustment = 1;

		[S] int CurrentHealth;
		public List<CombatantStatus> Statuses = new();

		[Header("Body Parts")]
		public HeadBodyPartEntry Head;
		public TorsoBodyPartEntry Torso;
		public TailBodyPartEntry Tail;
		public List<AppendageBodyPartEntry> Appendages = new();

		[Header("Skills")]
		public List<SkillId> Skills = new();

		// -------------------------------------------------------------------------

		[NonSerialized] readonly List<float> attributes = new() { 1, 1, 1, 1, 1, 1 };
		[NonSerialized] readonly List<float> attributeAdjustments = new() { 0, 0, 0, 0, 0, 0 };

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

		public float AdjustmentUsable {
			get {
				return Adjustment < 1
					? Adjustment
					: 1 + ((Adjustment - 1) * 0.1f);
			}
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
				return Mathf.RoundToInt(Endurance * 2);
			}
		}

		public int Strength {
			get {
				return GetAttribute(0);
			}
		}

		public int Endurance {
			get {
				return GetAttribute(1);
			}
		}

		public int Dexterity {
			get {
				return GetAttribute(2);
			}
		}

		public int Intelligence {
			get {
				return GetAttribute(3);
			}
		}

		public int Wisdom {
			get {
				return GetAttribute(4);
			}
		}

		public int Luck {
			get {
				return GetAttribute(5);
			}
		}

		public int Experience {
			get {
				return Mathf.RoundToInt(attributes.Sum());
			}
		}

		int GetAttribute(int index) {
			return Mathf.Clamp(
				Mathf.RoundToInt(
					attributes[index] + attributeAdjustments[index]
				),
				1,
				999
			);
		}

		public void PrepareForBattle() {
			CalculateAttributes();
			CalculateAttributeAdjustments();


			Debug.Log(Endurance + " " + Adjustment + " " + HealthTotal);
		}

		public void CalculateAttributes() {
			attributes[0] = 0;
			attributes[1] = 0;
			attributes[2] = 0;
			attributes[3] = 0;
			attributes[4] = 0;
			attributes[5] = 0;

			//
			CalculateBodyPartAttributes(Head);
			CalculateBodyPartAttributes(Torso);
			CalculateBodyPartAttributes(Tail);
			Appendages.ForEach(CalculateBodyPartAttributes);

			//
			attributes[0] = Mathf.Clamp(Mathf.RoundToInt(attributes[0]), 1, 999);
			attributes[1] = Mathf.Clamp(Mathf.RoundToInt(attributes[1]), 1, 999);
			attributes[2] = Mathf.Clamp(Mathf.RoundToInt(attributes[2]), 1, 999);
			attributes[3] = Mathf.Clamp(Mathf.RoundToInt(attributes[3]), 1, 999);
			attributes[4] = Mathf.Clamp(Mathf.RoundToInt(attributes[4]), 1, 999);
			attributes[5] = Mathf.Clamp(Mathf.RoundToInt(attributes[5]), 1, 999);
		}

		public void AdvanceStatuses() {
			Statuses.ForEach(status => status.Turns -= 1);
			_ = Statuses.RemoveAll(status => status.Turns < 1);

			//
			CalculateAttributeAdjustments();
		}

		public bool HasStatus(Status status) {
			return Statuses.Any(cs => cs.Status == status);
		}

		public void AddStatus(Status status, int turns, float strength) {
			CombatantStatus combatantStatus = Statuses.Find(cs => cs.Status == status);
			if (combatantStatus != null) {
				_ = Statuses.Remove(combatantStatus);
			}

			//
			Statuses.Add(new CombatantStatus {
				Status = status,
				Turns = turns,
				Strength = strength
			});
		}

		public void AdjustHealth(int amount) {
			CurrentHealth = Mathf.Clamp(Health + amount, 0, HealthTotal);
		}

		public void Heal() {
			Health = HealthTotal;
		}

		// -------------------------------------------------------------------------

		void CalculateBodyPartAttributes(HeadBodyPartEntry head) {
			if (MissingBodyPart(head?.BodyPartId)) {
				return;
			}

			//
			float adjustment = AdjustmentUsable * head.Quality * head.GradeAsAdjustment;
			CalculateBodyPartAttribute(0, head.BodyPart.Strength, adjustment);
			CalculateBodyPartAttribute(1, head.BodyPart.Endurance, adjustment);
			CalculateBodyPartAttribute(2, head.BodyPart.Dexterity, adjustment);
			CalculateBodyPartAttribute(3, head.BodyPart.Intelligence, adjustment);
			CalculateBodyPartAttribute(4, head.BodyPart.Wisdom, adjustment);
			CalculateBodyPartAttribute(5, head.BodyPart.Luck, adjustment);
		}

		void CalculateBodyPartAttributes(TorsoBodyPartEntry torso) {
			if (MissingBodyPart(torso?.BodyPartId))
				return;

			//
			float adjustment = AdjustmentUsable * torso.Quality * torso.GradeAsAdjustment;
			CalculateBodyPartAttribute(0, torso.BodyPart.Strength, adjustment);
			CalculateBodyPartAttribute(1, torso.BodyPart.Endurance, adjustment);
			CalculateBodyPartAttribute(2, torso.BodyPart.Dexterity, adjustment);
			CalculateBodyPartAttribute(3, torso.BodyPart.Intelligence, adjustment);
			CalculateBodyPartAttribute(4, torso.BodyPart.Wisdom, adjustment);
			CalculateBodyPartAttribute(5, torso.BodyPart.Luck, adjustment);
		}

		void CalculateBodyPartAttributes(TailBodyPartEntry tail) {
			if (MissingBodyPart(tail?.BodyPartId))
				return;

			//
			float adjustment = AdjustmentUsable * tail.Quality * tail.GradeAsAdjustment;
			CalculateBodyPartAttribute(0, tail.BodyPart.Strength, adjustment);
			CalculateBodyPartAttribute(1, tail.BodyPart.Endurance, adjustment);
			CalculateBodyPartAttribute(2, tail.BodyPart.Dexterity, adjustment);
			CalculateBodyPartAttribute(3, tail.BodyPart.Intelligence, adjustment);
			CalculateBodyPartAttribute(4, tail.BodyPart.Wisdom, adjustment);
			CalculateBodyPartAttribute(5, tail.BodyPart.Luck, adjustment);
		}

		void CalculateBodyPartAttributes(AppendageBodyPartEntry appendage) {
			if (MissingBodyPart(appendage?.BodyPartId))
				return;

			//
			float adjustment = AdjustmentUsable * appendage.Quality * appendage.GradeAsAdjustment;
			CalculateBodyPartAttribute(0, appendage.BodyPart.Strength, adjustment);
			CalculateBodyPartAttribute(1, appendage.BodyPart.Endurance, adjustment);
			CalculateBodyPartAttribute(2, appendage.BodyPart.Dexterity, adjustment);
			CalculateBodyPartAttribute(3, appendage.BodyPart.Intelligence, adjustment);
			CalculateBodyPartAttribute(4, appendage.BodyPart.Wisdom, adjustment);
			CalculateBodyPartAttribute(5, appendage.BodyPart.Luck, adjustment);
		}

		void CalculateBodyPartAttribute(int index, int value, float quality) {
			attributes[index] += value * quality;
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
					default:
						break;
				}
			});
		}

		// -------------------------------------------------------------------------

	}
}
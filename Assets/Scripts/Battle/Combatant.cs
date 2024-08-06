using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Battle {

	public class Combatant {
		string name;

		int level;
		int health;
		int magic;
		int strength;
		int endurance;
		int dexterity;
		int intelligence;
		int wisdom;
		int luck;

		float strengthAdjustment = 1;
		float enduranceAdjustment = 1;
		float dexterityAdjustment = 1;
		float intelligenceAdjustment = 1;
		float wisdomAdjustment = 1;
		float luckAdjustment = 1;

		static public Combatant New(string name, int level, int strength, int endurance, int dexterity, int intelligence, int wisdom, int luck, int health, int magic = 999) {
			return new Combatant(name, level, strength, endurance, dexterity, intelligence, wisdom, luck, health, magic);
		}

		public Combatant(string name, int level, int strength, int endurance, int dexterity, int intelligence, int wisdom, int luck, int health, int magic = 999) {
			this.name = name;
			this.level = level;
			this.strength = strength;
			this.endurance = endurance;
			this.dexterity = dexterity;
			this.intelligence = intelligence;
			this.wisdom = wisdom;
			this.luck = luck;
			this.health = health;
			this.magic = magic;

			Update();
		}

		public string Name {
			get {
				return name;
			}
		}

		public int Level {
			get {
				return level;
			}
		}

		public int Health {
			get {
				return health;
			}
		}

		public int HealthTotal {
			get {
				return Clamp((float) (Endurance + level) * (5f + ((float) level / 2f)));
			}
		}

		public int Magic {
			get {
				return magic;
			}
		}

		public int MagicTotal {
			get {
				return Clamp((float) Wisdom * (5f + ((float) level / 2f)));
			}
		}

		public int Strength {
			get {
				return Adjust(strength, strengthAdjustment);
			}
		}

		public int Endurance {
			get {
				return Adjust(endurance, enduranceAdjustment);
			}
		}

		public int Dexterity {
			get {
				return Adjust(dexterity, dexterityAdjustment);
			}
		}

		public int Intelligence {
			get {
				return Adjust(intelligence, intelligenceAdjustment);
			}
		}

		public int Wisdom {
			get {
				return Adjust(wisdom, wisdomAdjustment);
			}
		}

		public int Luck {
			get {
				return Adjust(luck, luckAdjustment);
			}
		}

		public List<CombatantStatus> Statuses = new();

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

		public void Update() {
			strengthAdjustment = 1;
			enduranceAdjustment = 1;
			dexterityAdjustment = 1;
			intelligenceAdjustment = 1;
			wisdomAdjustment = 1;
			luckAdjustment = 1;

			Statuses.ForEach(cs => {
				switch (cs.Status) {
					case Status.StrengthBuff:
					case Status.StrengthDebuff:
						strengthAdjustment += cs.Strength;
						break;

					case Status.EnduranceBuff:
					case Status.EnduranceDebuff:
						enduranceAdjustment += cs.Strength;
						break;

					case Status.DexterityBuff:
					case Status.DexterityDebuff:
						dexterityAdjustment += cs.Strength;
						break;

					case Status.IntelligenceBuff:
					case Status.IntelligenceDebuff:
						intelligenceAdjustment += cs.Strength;
						break;

					case Status.WisdomBuff:
					case Status.WisdomDebuff:
						wisdomAdjustment += cs.Strength;
						break;

					case Status.LuckBuff:
					case Status.LuckDebuff:
						luckAdjustment += cs.Strength;
						break;
				}
			});

			if (Health > HealthTotal) {
				health = HealthTotal;
			}

			if (Magic > MagicTotal) {
				magic = MagicTotal;
			}
		}

		public void Advance() {
			List<CombatantStatus> statusesToRemove = new();
			Statuses.ForEach(status => {
				status.Turns -= 1;

				if (status.Turns < 1) {
					statusesToRemove.Add(status);
				}
			});
			statusesToRemove.ForEach(status => Statuses.Remove(status));

			//
			Update();
		}

		int Adjust(float value, float adjustment, int min = 1, int max = 999) {
			return Clamp(value * adjustment, min, max);
		}

		int Clamp(float value, int min = 0, int max = 999) {
			return Mathf.Clamp(
				Mathf.RoundToInt(value),
				min,
				max
			);
		}

		public void AdjustHealth(int amount) {
			health = Clamp(health + amount, 0, HealthTotal);
		}

		public void AdjustMagic(int amount) {
			magic = Clamp(magic + amount, 0, MagicTotal);
		}
	}

	public class CombatantStatus {
		public Status Status;
		public int Turns = 1;
		public float Strength = 1;
	}
}

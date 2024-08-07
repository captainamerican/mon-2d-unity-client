﻿using System;

namespace Game {
	public enum EffectType {
		None,
		Health = 10,
		Magic = 20,
		Status = 30,
		BodyPartQuality = 40,
	}

	public enum Status {
		None = 0,

		Entranced = 10,
		Poisoned = 20,
		Beserked = 30,
		Sleeping = 40,
		Burning = 50,
		Stunned = 60,
		Frozen = 70,
		Constricted = 80,

		StrengthBuff = 1000,
		StrengthDebuff = 1005,

		EnduranceBuff = 1010,
		EnduranceDebuff = 1015,

		DexterityBuff = 1020,
		DexterityDebuff = 1025,

		IntelligenceBuff = 1030,
		IntelligenceDebuff = 1035,

		WisdomBuff = 1040,
		WisdomDebuff = 1045,

		LuckBuff = 1050,
		LuckDebuff = 1055
	}

	public enum ApplicableTarget {
		None,
		Player = 10,
		Creature = 20,
		Enemy = 30
	}

	public enum ItemType {
		None,
		Consumable = 10,
		Reusable = 20,
		CraftingMaterial = 30,
		TrainingItem = 40,
		BodyPart = 50,
		KeyItem = 60
	}

	public enum NumberOfAppendages {
		None,
		One,
		Two,
		Four,
		Six
	}

	public enum BodyPart {
		None,
		Head,
		Torso,
		Tail,
		LeftArm,
		RightArm,
		LeftLeg,
		RightLeg
	}

	public enum BodyPartTag {
		None,

		Claw = 10,
		Teeth = 11,
		OpposableThumb = 13,

		Magical = 100,
		Physical = 101,

		Fire = 200,
		Earth = 201,
		Wind = 202,
		Water = 203,
		Lighting = 204,
		Holy = 205,
		Dark = 206,
	}

	[Serializable]
	public class Effect {
		public EffectType Type;
		public Status Status;
		public int Duration = 0;
		public int Strength = 0;
		public float Variance = 0;
		public bool ApplyToSelf;
	}

	[Serializable]
	public class WeightedLootDrop {
		public Item Item;
		public int Weight = 100;
		public int Quantity = 1;
	}

	[Serializable]
	public class LootDrop {
		public Item Item;
		public int Quantity = 1;
	}

	[Serializable]
	public class InventoryEntry {
		public Item Item;
		public int Amount;
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

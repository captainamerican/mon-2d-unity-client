using System;

using NanoidDotNet;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game {
	public enum EffectType {
		None = 0,

		Health = 10,
		Magic = 20,
		Status = 30,
		BodyPartQuality = 40,
		Preservative = 50,
		Experience = 60
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

	public enum BodyPartId {
		None = 0,

		WolfHead = 1001,
		WolfTorso = 1002,
		WolfTail = 1003,
		WolfFrontLeg = 1004,
		WolfRearLeg = 1004,
	}

	public enum SkillId {
		None = 0,

		Scratch = 1,
		Bite = 2,
		Kick = 3
	}

	public enum ApplicableTarget {
		None = 0,

		Player = 10,
		Creature = 20,
		Enemy = 30
	}

	public enum ItemType {
		None = 0,

		Consumable = 10,
		Reusable = 20,
		CraftingMaterial = 30,
		TrainingItem = 40,
		BodyPart = 50,
		KeyItem = 60
	}

	public enum TypeOfAppendages {
		None = 0,

		OneLowerNoUpper = 10,
		OneLowerTwoUpper = 20,
		TwoLowerNoUpper = 30,
		TwoLowerTwoUpper = 40,
		FourLower = 50,
		FourLowerTwoUpper = 60,
		SixLower = 70,
		NoLowerTwoUpper = 80,
	}

	public enum PartOfBody {
		None = 0,

		Head,
		Torso,
		Tail,
		Appendage
	}

	public enum EssenceTag {
		None = 0,

		Claw = 10,
		Teeth = 11,
		OpposableThumb = 13,
		Leg = 14,
		Arm = 15,
		Front = 16,
		Rear = 17,

		Physical = 100,
		Magical = 101,

		Fire = 200,
		Earth = 201,
		Wind = 202,
		Water = 203,
		Lighting = 204,
		Holy = 205,
		Dark = 206,
		Poison = 207,
		Infected = 208
	}

	public enum ChestId {
		None = 0,

		ForestEntranceFirst,
		ForestEntranceSecond,
		ForestCaveFirst
	}

	public enum SpiritId {
		None = 0,

		ForestEntrance01
	}

	public enum ItemId {
		None = 0
			,
		PotionS = 1000,
		PotionA = 1001,
		PotionB = 1002,
		PotionC = 1003,
		PotionD = 1004,
		PotionE = 1005,
		PotionF = 1006,
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
	public class SkillEntry {
		public Skill Skill;
		public int Experience = 0;
	}

	[Serializable]
	public class SkillFX {
		public bool Actor;
		public float Delay;
	}

	[Serializable]
	public class RecipeIngredient {
		public Item Item;

		[Range(0, 99)]
		public int Quantity;
	}

	[Serializable]
	public class EncounterPossibility {
		public int Weight = 100;
		public int Level = 1;
		public ConstructedCreature Creature;
	}

	static public class Focus {
		static public void This(UnityEngine.UI.Slider slider) {
			if (slider == null) {
				throw new SystemException("Slider was null");
			}

			slider.Select();
			slider.OnSelect(null);

			//
			var informationButton = slider.GetComponent<InformationButton>();
			if (informationButton != null) {
				informationButton.OnSelect(null);
			}
		}

		static public void This(UnityEngine.UI.Button button) {
			if (button == null) {
				throw new SystemException("Button was null");
			}

			button.Select();
			button.OnSelect(null);

			//
			var informationButton = button.GetComponent<InformationButton>();
			if (informationButton != null) {
				informationButton.OnSelect(null);
			}
		}
	}

	static public class Id {
		static readonly string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		static public string Generate() {
			return Nanoid.Generate(alphabet, 16);
		}
	}

	static public class Control {
		static public InputAction Get(PlayerInput playerInput, string action) {
			return playerInput.currentActionMap.FindAction(action);
		}
	}

	public class CompletionData {
		public int current;
		public int total;
		public float ratio;
	}
}

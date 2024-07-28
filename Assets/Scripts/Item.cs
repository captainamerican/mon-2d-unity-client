using System;

using UnityEngine;

namespace Item {
	public enum Type {
		None,
		Consumable,
		Reusable,
		CraftingMaterial,
		TrainingItem,
		BodyPart,
		KeyItem
	}

	[CreateAssetMenu(fileName = "ItemData", menuName = "MoN/ItemData")]
	public class Data : ScriptableObject {
		public Type Type;
		public string Name;
		public string Description;


		static public string TypeName(Type type) {
			switch (type) {
				case Type.None:
					return "None";

				case Type.Consumable:
					return "Consumable";

				case Type.Reusable:
					return "Reusable";

				case Type.CraftingMaterial:
					return "Crafting Material";

				case Type.TrainingItem:
					return "Training Item";

				case Type.BodyPart:
					return "BodyPart";

				case Type.KeyItem:
					return "KeyItem";
			}

			return "Unknown";
		}
	}

	[Serializable]
	public class WeightedLootDrop {
		[SerializeField]
		public Data ItemData;
		[SerializeField]
		public int Weight = 100;
		[SerializeField]
		public int Quantity = 1;
	}

	[Serializable]
	public class LootDrop {
		[SerializeField]
		public Data ItemData;
		[SerializeField]
		public int Quantity = 1;
	}
}

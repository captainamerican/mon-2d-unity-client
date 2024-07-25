using System;
using UnityEngine;

namespace Item {
	public enum Type {
		None,
		CraftingMaterial,
		BodyPart,
		Consumable,
		Reusable,
		KeyItem
	}

	[CreateAssetMenu(fileName = "ItemData", menuName = "MoN/ItemData")]
	public class Data : ScriptableObject {
		public Type Type;
		public string Name;
		public string Description;
	}

	[Serializable]
	public class LootDrop {
		[SerializeField]
		public Data ItemData;
		[SerializeField]
		public int Weight = 100;
		[SerializeField]
		public int Quantity = 1;
	}
}

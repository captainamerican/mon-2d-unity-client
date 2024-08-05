using System;

namespace Battle {
	public enum EffectType {
		Health,
		Status
	}

	public enum Status {
		None = 0,

		Entranced = 10,
		Poisoned = 20,
		Beserked = 30,
		Sleeping = 40,
		Burning = 50,

		HealthBuff = 1000,
		HealthDebuff = 1005,

		AttackBuff = 1010,
		AttackDebuff = 1015,

		SpeedBuff = 1020,
		SpeedDebuff = 1025,
	}

	[Serializable]
	public class Effect {
		public EffectType Type;
		public Status Status;
		public int Value;
		public bool ApplyToSelf;
	}
}
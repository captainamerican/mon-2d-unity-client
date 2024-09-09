using System;

namespace Game {
	[Serializable]
	public class Effect {
		public EffectType Type;
		public Status Status;
		public int Duration = 0;
		public int Strength = 0;
		public float Variance = 0;
		public bool ApplyToActor;
	}
}

using System;

namespace Game {
	[Serializable]
	public class Effect {
		public EffectType Type;
		public EffectTarget Target;
		public Status Status;
		public int Duration = 0;
		public int Strength = 0;
		public float Variance = 0;
	}
}

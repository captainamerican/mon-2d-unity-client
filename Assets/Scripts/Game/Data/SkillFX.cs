using System;

namespace Game {
	public enum EffectTarget {
		Actor,
		Recipient
	}

	[Serializable]
	public class SkillFX {
		public EffectTarget Target;
		public float Delay;
		public string AnimationName;
	}
}

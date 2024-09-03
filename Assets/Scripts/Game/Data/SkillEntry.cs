using System;

namespace Game {
	[Serializable]
	public class SkillEntry {
		public SkillId SkillId;
		public int Experience = 0;

		public Skill Skill {
			get {
				return Database.Engine.GameData.Get(SkillId);
			}
		}
	}
}

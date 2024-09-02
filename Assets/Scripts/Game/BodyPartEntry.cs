using System;

using UnityEngine;

namespace Game {
	[Serializable]
	public class BodyPartEntryBase {
		public string Id = Game.Id.Generate();
		public int Experience = 0;
		public float Quality = 1;


	}

	[Serializable]
	public class HeadBodyPartEntry : BodyPartEntryBase, ISerializationCallbackReceiver {
		[SerializeField, HideInInspector] string bodyPartSlug;

		public HeadBodyPart BodyPart;

		public int MaxSkills {
			get {
				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
				int level = Mathf.FloorToInt(rawLevel);

				//
				return 1 + level;
			}
		}

		public int Grade {
			get {

				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}


		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			if (BodyPart == null && bodyPartSlug != "") {
				BodyPart = (HeadBodyPart) FuckYouUnity.Engine.BodyParts.Find(bp => bp.Slug == bodyPartSlug);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			bodyPartSlug = BodyPart?.Slug;
		}
	}

	[Serializable]
	public class TorsoBodyPartEntry : BodyPartEntryBase {
		public TorsoBodyPart BodyPart;

		public int Grade {
			get {

				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}
	}

	[Serializable]
	public class TailBodyPartEntry : BodyPartEntryBase {
		public TailBodyPart BodyPart;

		public int Grade {
			get {

				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}
	}

	[Serializable]
	public class AppendageBodyPartEntry : BodyPartEntryBase {
		public AppendageBodyPart BodyPart;

		public int Grade {
			get {

				int experience = Experience;
				int toLevel = BodyPart.ExperienceToLevel;
				float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);

				//
				return Mathf.FloorToInt(rawLevel);
			}
		}
	}

}

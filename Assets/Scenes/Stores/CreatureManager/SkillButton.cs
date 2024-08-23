using Game;

using TMPro;

using UnityEngine;

// -----------------------------------------------------------------------------

public class SkillButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] TextMeshProUGUI NameLabel;
	[SerializeField] TextMeshProUGUI GradeLabel;
	[SerializeField] RectTransform GradeProgress;

	// ---------------------------------------------------------------------------

	public SkillEntry LearnedSkill {
		get;
		private set;
	}


	// ---------------------------------------------------------------------------


	public void Configure(SkillEntry learnedSkill) {
		LearnedSkill = learnedSkill;

		//
		NameLabel.text = learnedSkill.Skill.Name;

		//
		int experience = learnedSkill.Experience;
		int toLevel = learnedSkill.Skill.ExperienceToLearn;

		float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
		int level = Mathf.FloorToInt(rawLevel);
		int nextLevel = level < 3 ? level + 1 : 3;
		float ratio = level < 3 ? (rawLevel - level) : 1;

		//
		GradeProgress.localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);
		GradeLabel.text = string.Join(
			"",
			Do.Times(
				3,
				i => experience >= toLevel * (i + 1) ? "★" : "☆"
			)
		);

		//
		if (level < 1) {
			GradeLabel.color = new Color(0, 0, 0, 0.5f);
			NameLabel.color = new Color(0, 0, 0, 0.5f);
		} else {
			GradeLabel.color = Color.black;
			NameLabel.color = Color.black;
		}
	}

	// ---------------------------------------------------------------------------

}

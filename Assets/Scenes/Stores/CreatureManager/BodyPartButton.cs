
using Game;

using TMPro;

using UnityEngine;

// -----------------------------------------------------------------------------

public class BodyPartButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] TextMeshProUGUI NameLabel;
	[SerializeField] TextMeshProUGUI GradeLabel;
	[SerializeField] RectTransform GradeProgress;
	[SerializeField] RectTransform QualityProgress;

	// ---------------------------------------------------------------------------

	public BodyPartEntry BodyPartEntry {
		get;
		private set;
	}

	// ---------------------------------------------------------------------------

	public void Configure(BodyPartEntry bodyPartEntry, string defaultName = "???") {
		BodyPartEntry = bodyPartEntry;

		//
		if (bodyPartEntry?.BodyPart == null) {
			NameLabel.text = $"({defaultName})";
			GradeLabel.text = "☆☆☆";
			GradeProgress.localScale = new Vector3(0, 1, 1);
			QualityProgress.localScale = new Vector3(0, 1, 1);
			return;
		}

		//
		NameLabel.text = bodyPartEntry.BodyPart.Name;

		//
		int experience = bodyPartEntry.Experience;
		int toLevel = bodyPartEntry.BodyPart.ExperienceToLevel;

		float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
		int level = Mathf.FloorToInt(rawLevel);
		int nextLevel = level < 3 ? level + 1 : 3;
		float ratio = level < 3 ? (rawLevel - level) : 1;

		GradeProgress.localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);
		GradeLabel.text = string.Join(
			"",
			Do.Times(
				3,
				i => experience >= toLevel * (i + 1) ? "★" : "☆"
			)
		);

		//
		QualityProgress.localScale = new Vector3(Mathf.Clamp01(bodyPartEntry.Quality), 1, 1);
	}

	// ---------------------------------------------------------------------------

}

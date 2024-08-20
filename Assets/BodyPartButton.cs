using System;

using Game;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

// -----------------------------------------------------------------------------

public class BodyPartButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] TextMeshProUGUI NameLabel;
	[SerializeField] TextMeshProUGUI GradeLabel;
	[SerializeField] RectTransform GradeProgress;
	[SerializeField] RectTransform QualityProgress;

	// ---------------------------------------------------------------------------

	Action onSelect;

	// ---------------------------------------------------------------------------

	public void Configure(BodyPartEntry bodyPartEntry, Action onSelect) {
		this.onSelect = onSelect;

		// 
		if (bodyPartEntry?.BodyPart == null) {
			return;
		}

		//
		NameLabel.text = bodyPartEntry.BodyPart.Name;

		//
		int experience = bodyPartEntry.Experience;
		int toLevel = bodyPartEntry.BodyPart.ExperienceToLevel;

		float rawLevel = 3f * ((float) experience / (float) (toLevel * 3f));
		int level = Mathf.FloorToInt(rawLevel);
		int nextLevel = level < 3 ? level + 1 : 3;
		float ratio = (rawLevel - level);

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

	public void OnSelect(BaseEventData _) {
		onSelect?.Invoke();
	}

	// ---------------------------------------------------------------------------

}


using System;

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

	public BodyPartEntryBase BodyPartEntry {
		get; private set;
	}

	// ---------------------------------------------------------------------------

	void Clear(string defaultName) {
		NameLabel.text = $"({defaultName})";
		NameLabel.color = new Color(0, 0, 0, 0.5f);
		GradeLabel.text = "☆☆☆";
		GradeLabel.color = new Color(0, 0, 0, 0.5f);
		GradeProgress.localScale = new Vector3(0, 1, 1);
		QualityProgress.localScale = new Vector3(0, 1, 1);
	}

	public void Configure(BodyPartEntryBase entry) {
		switch (entry.GetType().Name) {
			case nameof(HeadBodyPartEntry):
				Configure((HeadBodyPartEntry) entry);
				break;
			case nameof(TorsoBodyPartEntry):
				Configure((TorsoBodyPartEntry) entry);
				break;
			case nameof(TailBodyPartEntry):
				Configure((TailBodyPartEntry) entry);
				break;
			case nameof(AppendageBodyPartEntry):
				Configure((AppendageBodyPartEntry) entry);
				break;
		}
	}

	public void Configure(HeadBodyPartEntry entry, string defaultName = null) {
		BodyPartEntry = entry;

		//
		Clear(HeadBodyPart.Label ?? defaultName ?? "???");
		if (entry?.BodyPart == null) {
			return;
		}

		// 
		Configure(
			entry.BodyPart.Name,
			entry.Experience,
			entry.BodyPart.ExperienceToLevel,
			entry.Quality
		);
	}

	public void Configure(TorsoBodyPartEntry entry) {
		BodyPartEntry = entry;

		//
		Clear(TorsoBodyPart.Label);
		if (entry?.BodyPart == null) {
			return;
		}

		//
		Configure(
			entry.BodyPart.Name,
			entry.Experience,
			entry.BodyPart.ExperienceToLevel,
			entry.Quality
		);
	}

	public void Configure(TailBodyPartEntry entry) {
		BodyPartEntry = entry;

		//
		Clear(TailBodyPart.Label);
		if (entry?.BodyPart == null) {
			return;
		}

		//
		Configure(
			entry.BodyPart.Name,
			entry.Experience,
			entry.BodyPart.ExperienceToLevel,
			entry.Quality
		);
	}

	public void Configure(AppendageBodyPartEntry entry, string defaultName = "???") {
		BodyPartEntry = entry;

		//
		Clear(defaultName);
		if (entry?.BodyPart == null) {
			return;
		}

		//
		Configure(
			entry.BodyPart.Name,
			entry.Experience,
			entry.BodyPart.ExperienceToLevel,
			entry.Quality
		);
	}

	public void Configure(string name, int experience, int toLevel, float quality) {
		NameLabel.text = name;
		NameLabel.color = Color.black;

		// 
		float rawLevel = toLevel > 0
			? Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3)
			: 0;
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
		GradeLabel.color = Color.black;

		//
		QualityProgress.localScale = new Vector3(Mathf.Clamp01(quality), 1, 1);
	}

	// ---------------------------------------------------------------------------

}

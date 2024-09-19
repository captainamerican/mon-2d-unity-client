using System;

using TMPro;

using UnityEngine;

// ----------------------------------------------------------------------------

public class SaveFileButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;

	[SerializeField] TextMeshProUGUI TimestampLabel;
	[SerializeField] TextMeshProUGUI LevelAndLocationLabel;
	[SerializeField] TextMeshProUGUI LevelLabel;
	[SerializeField] TextMeshProUGUI CompletionLabel;
	[SerializeField] TextMeshProUGUI PlaytimeLabel;

	// ---------------------------------------------------------------------------

	public void Configure(Game.SaveFile saveFile) {
		DateTime date = new DateTime(saveFile.SavedAt);
		TimestampLabel.text = $"{date:G}";

		LevelAndLocationLabel.text = $"Lvl {saveFile.Level} {saveFile.SceneName}";

		var completion = Engine.SaveFileCompletion(saveFile);
		CompletionLabel.text = completion.ratio < 1 ? $"{completion.ratio * 100:n2}%" : "100%";

		PlaytimeLabel.text = saveFile.PlayTimeAsString;
	}

	// ---------------------------------------------------------------------------

}

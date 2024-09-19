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

	public void Configure(Game.SaveFile saveFile, bool isAutoSave) {
		DateTime date = new(saveFile.SavedAt);
		TimestampLabel.text = isAutoSave ? $"AUTO {date:yy/MM/dd hh:mm:ss tt}" : $"{date:yy/MM/dd hh:mm:ss tt}";

		LevelAndLocationLabel.text = $"Lvl {saveFile.Level} {saveFile.SceneName}";

		var completion = Engine.SaveFileCompletion(saveFile);
		CompletionLabel.text = completion.ratio < 1 ? $"{completion.ratio * 100:n2}%" : "100%";

		PlaytimeLabel.text = saveFile.PlayTimeAsString;
	}

	// ---------------------------------------------------------------------------

}

using TMPro;

using UnityEngine;

// ----------------------------------------------------------------------------

public class SaveFileButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;

	[SerializeField] TextMeshProUGUI SaveLabel;
	[SerializeField] TextMeshProUGUI LevelLabel;
	[SerializeField] TextMeshProUGUI CompletionLabel;
	[SerializeField] TextMeshProUGUI PlaytimeLabel;

	// ---------------------------------------------------------------------------

	public void Configure(Game.SaveFile saveFile) {
		SaveLabel.text = saveFile.IsAutoSave ? "Autosave" : $"Save #{100 - saveFile.FileIndex:d2}";

		LevelLabel.text = $"Lvl {saveFile.Level}";

		var completion = Engine.SaveFileCompletion(saveFile);
		CompletionLabel.text = completion.ratio < 1 ? $"{completion.ratio * 100:n2}%" : "100%";

		PlaytimeLabel.text = saveFile.PlayTimeAsString;
	}

	// ---------------------------------------------------------------------------

}

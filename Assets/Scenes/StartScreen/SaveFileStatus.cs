using TMPro;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace StartScreen {
	public class SaveFileStatus : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] TextMeshProUGUI LevelLabel;
		[SerializeField] TextMeshProUGUI ExperienceLabel;
		[SerializeField] RectTransform ExperienceRatio;

		[SerializeField] TextMeshProUGUI MagicLabel;

		[SerializeField] TextMeshProUGUI LocationLabel;

		[SerializeField] TextMeshProUGUI WisdomLabel;

		[SerializeField] RectTransform HungerRatio;

		[SerializeField] TextMeshProUGUI CompletionPercentLabel;
		[SerializeField] TextMeshProUGUI CompletionLabel;
		[SerializeField] RectTransform CompletionRatio;

		[SerializeField] TextMeshProUGUI PlaytimeLabel;

		// -------------------------------------------------------------------------

		public void Configure(Game.SaveFile saveFile) {
			LevelLabel.text = $"Lvl {saveFile.Level}";
			ExperienceLabel.text = $"{saveFile.ExperienceForNextLevel - saveFile.Experience:n0} to next";

			float experienceRatio = (float) saveFile.Experience / (float) saveFile.ExperienceForNextLevel;
			ExperienceRatio.localScale = new Vector3(Mathf.Clamp01(experienceRatio), 1, 1);

			MagicLabel.text = $"{saveFile.Magic}/{saveFile.MagicTotal}";

			WisdomLabel.text = $"{saveFile.Wisdom}";

			LocationLabel.text = saveFile.SceneName;

			HungerRatio.localScale = new Vector3(Mathf.Clamp01(saveFile.Hunger), 1, 1);

			//
			var completion = Engine.SaveFileCompletion();

			CompletionPercentLabel.text = completion.ratio < 1 ? $"{completion.ratio * 100:n2}%" : "100%";
			CompletionLabel.text = $"{completion.current}/{completion.total}";
			CompletionRatio.localScale = new Vector3(Mathf.Clamp01(completion.ratio), 1, 1);
		}

		// -------------------------------------------------------------------------
	}
}
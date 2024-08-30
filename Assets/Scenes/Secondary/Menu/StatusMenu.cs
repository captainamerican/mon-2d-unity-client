using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class StatusMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] TextMeshProUGUI LevelLabel;
		[SerializeField] TextMeshProUGUI ExperienceLabel;
		[SerializeField] RectTransform ExperienceRatio;

		[SerializeField] TextMeshProUGUI MagicLabel;

		[SerializeField] TextMeshProUGUI WisdomLabel;

		[SerializeField] RectTransform HungerRatio;

		[SerializeField] TextMeshProUGUI CompletionPercentLabel;
		[SerializeField] TextMeshProUGUI CompletionLabel;
		[SerializeField] RectTransform CompletionRatio;

		[SerializeField] TextMeshProUGUI PlaytimeLabel;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		bool isSpeedrunning;

		// --------------------------------------------------------------------------

		void OnEnable() {
			isSpeedrunning = Engine.Profile.Options.Speedrunning;

			//
			LevelLabel.text = $"Lvl {Engine.Profile.Level}";
			ExperienceLabel.text = $"{Engine.Profile.ExperienceForNextLevel - Engine.Profile.Experience:n0} to next";

			float experienceRatio = (float) Engine.Profile.Experience / (float) Engine.Profile.ExperienceForNextLevel;
			ExperienceRatio.localScale = new Vector3(Mathf.Clamp01(experienceRatio), 1, 1);

			MagicLabel.text = $"{Engine.Profile.Magic}/{Engine.Profile.MagicTotal}";

			WisdomLabel.text = $"{Engine.Profile.Wisdom}";

			HungerRatio.localScale = new Vector3(Mathf.Clamp01(Engine.Profile.Hunger), 1, 1);

			//
			int total = 0;
			total += Engine.AllBodyParts.Count;
			total += Engine.AllGameplay.Count;
			total += Engine.AllItems.Count;
			total += Engine.AllLore.Count;
			total += Engine.AllSkills.Count;
			total += Engine.AllSpiritWisdom.Count;
			total += Engine.AllTags.Count;

			int current = 0;
			current += Engine.Profile.AcquiredBodyPart.Count;
			current += Engine.AllGameplay.Count;
			current += Engine.Profile.AcquiredItem.Count;
			current += Engine.Profile.AcquiredLore.Count;
			current += Engine.Profile.AcquiredSkills.Count;
			current += Engine.Profile.AcquiredSpiritWisdom.Count;
			current += Engine.Profile.AcquiredTags.Count;

			float completionRatio = total > 0 ? (float) current / (float) total : 0;

			CompletionPercentLabel.text = completionRatio < 1 ? $"{completionRatio * 100:n2}%" : "100%";
			CompletionLabel.text = $"{current}/{total}";
			CompletionRatio.localScale = new Vector3(Mathf.Clamp01(completionRatio), 1, 1);

			//
			RemoveInputCallbacks();
			Cancel = Game.Control.Get(PlayerInput, "Cancel");
			Cancel.performed += OnGoBack;
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			InitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		void Update() {
			float time = Time.unscaledTime;

			int hours = Mathf.FloorToInt(time / 3600f);
			int minutes = Mathf.FloorToInt((time - hours) / 60f);
			int seconds = Mathf.FloorToInt(time % 60f);
			int microseconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 1000);

			PlaytimeLabel.text = isSpeedrunning
				? $"{hours:d2}:{minutes:d2}:{seconds:d2}.{microseconds:d3}"
				: $"{hours:d2}:{minutes:d2}:{seconds:d2}";
		}

		// -------------------------------------------------------------------------  

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		// -------------------------------------------------------------------------
	}
}
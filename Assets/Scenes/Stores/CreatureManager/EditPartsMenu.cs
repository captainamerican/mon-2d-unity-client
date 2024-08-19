using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ---------------------------------------------------------------------------

namespace CreatureManager {

	// ---------------------------------------------------------------------------

	public class EditPartsMenu : MonoBehaviour {

		// ---------------------------------------------------------------------------

		enum FocusPhase {
			BodyPart,
			AvailableBodyPart
		}

		// ---------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] List<Button> BodyParts;
		[SerializeField] List<TextMeshProUGUI> BodyPartName;
		[SerializeField] List<TextMeshProUGUI> BodyPartGrade;
		[SerializeField] List<RectTransform> BodyPartLevelRatio;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// ---------------------------------------------------------------------------

		EditingCreature editing;

		InputAction Cancel;

		FocusPhase phase = FocusPhase.BodyPart;
		int selectedBodyPartIndex;
		int selectedAvailableBodyPartIndex;

		// ---------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();

		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= OnGoBack;
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}


		// ---------------------------------------------------------------------------

		void GoBack() {
			EditInitialMenu.gameObject.SetActive(true);
			EditInitialMenu.Configure(editing);

			gameObject.SetActive(false);
		}


		// ---------------------------------------------------------------------------

		void ConfigureCancelAction() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;

			//
			ConfigureBodyPartList();
		}


		void ConfigureBodyPartList() {
			BodyPartName[0].text = editing.Creature?.Head?.BodyPart?.Name ?? "(Head)";
			BodyPartName[1].text = editing.Creature?.Torso?.BodyPart?.Name ?? "(Torso)";
			BodyPartName[2].text = editing.Creature?.Tail?.BodyPart?.Name ?? "(Tail)";
			BodyPartName[3].text = editing.Creature?.LeftFrontAppendage?.BodyPart?.Name ?? "(L. Front)";
			BodyPartName[4].text = editing.Creature?.LeftMiddleAppendage?.BodyPart?.Name ?? "(L. Middle)";
			BodyPartName[5].text = editing.Creature?.LeftRearAppendage?.BodyPart?.Name ?? "(L. Rear)";
			BodyPartName[6].text = editing.Creature?.RightFrontAppendage?.BodyPart?.Name ?? "(R. Front)";
			BodyPartName[7].text = editing.Creature?.RightMiddleAppendage?.BodyPart?.Name ?? "(R. Middle)";
			BodyPartName[8].text = editing.Creature?.RightRearAppendage?.BodyPart?.Name ?? "(R. Rear)";

			SetQualityAndExperience(0, editing.Creature.Head);
			SetQualityAndExperience(1, editing.Creature.Torso);
			SetQualityAndExperience(2, editing.Creature.Tail);
			SetQualityAndExperience(3, editing.Creature.LeftFrontAppendage);
			SetQualityAndExperience(4, editing.Creature.LeftMiddleAppendage);
			SetQualityAndExperience(5, editing.Creature.LeftRearAppendage);
			SetQualityAndExperience(6, editing.Creature.RightFrontAppendage);
			SetQualityAndExperience(7, editing.Creature.RightMiddleAppendage);
			SetQualityAndExperience(8, editing.Creature.RightRearAppendage);

			//
			switch (editing.Creature?.Torso.BodyPart.Base.Appendages ?? Game.NumberOfAppendages.None) {
				case Game.NumberOfAppendages.OneLowerNoUpper:
					BodyParts[3].gameObject.SetActive(false);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(false);
					BodyParts[6].gameObject.SetActive(false);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(false);
					break;

				case Game.NumberOfAppendages.OneLowerTwoUpper:
					BodyParts[3].gameObject.SetActive(true);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(false);
					BodyParts[6].gameObject.SetActive(true);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(false);
					break;

				case Game.NumberOfAppendages.TwoLowerNoUpper:
					BodyParts[3].gameObject.SetActive(false);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(true);
					BodyParts[6].gameObject.SetActive(false);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.TwoLowerTwoUpper:
					BodyParts[3].gameObject.SetActive(true);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(true);
					BodyParts[6].gameObject.SetActive(true);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.FourLower:
					BodyParts[3].gameObject.SetActive(true);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(true);
					BodyParts[6].gameObject.SetActive(true);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.FourLowerTwoUpper:
					BodyParts[3].gameObject.SetActive(true);
					BodyParts[4].gameObject.SetActive(true);
					BodyParts[5].gameObject.SetActive(true);
					BodyParts[6].gameObject.SetActive(true);
					BodyParts[7].gameObject.SetActive(true);
					BodyParts[8].gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.SixLower:
					BodyParts[3].gameObject.SetActive(true);
					BodyParts[4].gameObject.SetActive(true);
					BodyParts[5].gameObject.SetActive(true);
					BodyParts[6].gameObject.SetActive(true);
					BodyParts[7].gameObject.SetActive(true);
					BodyParts[8].gameObject.SetActive(true);
					break;
			}
		}

		void SetQualityAndExperience(int index, Game.BodyPartEntry bodyPartEntry) {
			int experience = bodyPartEntry?.Experience ?? 0;
			int experienceToLevel = bodyPartEntry?.BodyPart?.ExperienceToLevel ?? 0;

			//
			float rawLevel = experienceToLevel > 0 ? 3f * ((float) experience / (float) (experienceToLevel * 3f)) : 0;
			int level = Mathf.FloorToInt(rawLevel);
			int nextLevel = level < 3 ? level + 1 : 3;
			float ratio = (rawLevel - level);

			Debug.Log($"{bodyPartEntry?.BodyPart?.Name ?? "null"}, {bodyPartEntry?.Experience}, {bodyPartEntry?.BodyPart?.ExperienceToLevel}, { rawLevel}, {level}, {nextLevel}, {ratio}");

			//
			BodyPartLevelRatio[index].localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);

			// 
			BodyPartGrade[index].text = "";
			Do.Times(3, i => {
				BodyPartGrade[index].text += i < level
						? "★"
						: "☆";
			});
		}
	}
}
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {

	// ---------------------------------------------------------------------------

	public class EditPartsMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum FocusPhase {
			BodyPart,
			AvailableBodyPart
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] List<BodyPartButton> BodyParts;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		EditingCreature editing;

		InputAction Cancel;

		FocusPhase phase = FocusPhase.BodyPart;
		int selectedBodyPartIndex;
		int selectedAvailableBodyPartIndex;

		List<BodyPartButton> bodyPartButtons = new();

		// -------------------------------------------------------------------------

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


		// -------------------------------------------------------------------------

		void GoBack() {
			EditInitialMenu.gameObject.SetActive(true);
			EditInitialMenu.Configure(editing);

			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

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
			ConfigureBodyPartButtons();
		}

		void ConfigureBodyPartList() {
			BodyParts[0].Configure(editing.Creature.Head, () => { });
			BodyParts[1].Configure(editing.Creature.Torso, () => { });
			BodyParts[2].Configure(editing.Creature.Tail, () => { });
			BodyParts[3].Configure(editing.Creature.LeftFrontAppendage, () => { });
			BodyParts[4].Configure(editing.Creature.LeftMiddleAppendage, () => { });
			BodyParts[5].Configure(editing.Creature.LeftRearAppendage, () => { });
			BodyParts[6].Configure(editing.Creature.RightFrontAppendage, () => { });
			BodyParts[7].Configure(editing.Creature.RightMiddleAppendage, () => { });
			BodyParts[8].Configure(editing.Creature.RightRearAppendage, () => { });

			//
			var appendages = editing.Creature?.Torso.BodyPart.Base.Appendages ?? Game.NumberOfAppendages.None;
			switch (appendages) {
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

				default:
					BodyParts[3].gameObject.SetActive(false);
					BodyParts[4].gameObject.SetActive(false);
					BodyParts[5].gameObject.SetActive(false);
					BodyParts[6].gameObject.SetActive(false);
					BodyParts[7].gameObject.SetActive(false);
					BodyParts[8].gameObject.SetActive(false);
					break;
			}
		}

		void ConfigureBodyPartButtons() {
			bodyPartButtons.Clear();

			BodyParts.ForEach(bpb => {
				if (bpb.gameObject.activeSelf) {
					bodyPartButtons.Add(bpb);
				}
			});

			for (int i = 0; i < bodyPartButtons.Count; i++) {
				int up = i == 0 ? bodyPartButtons.Count - 1 : i - 1;
				int down = i == bodyPartButtons.Count - 1 ? 0 : i + 1;

				//
				Button button = bodyPartButtons[i].GetComponent<Button>();

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = bodyPartButtons[up].GetComponent<Button>();
				navigation.selectOnDown = bodyPartButtons[down].GetComponent<Button>();

				button.navigation = navigation;
			}
		}

		// -------------------------------------------------------------------------

	}
}
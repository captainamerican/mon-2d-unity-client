using System.Collections.Generic;
using System.Linq;

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

		[Header("Available Body Parts")]
		[SerializeField] Transform AvailableBodyPartsList;
		[SerializeField] GameObject TemplateButton;
		[SerializeField] Button RemoveButton;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		EditingCreature editing;

		InputAction Cancel;

		FocusPhase phase = FocusPhase.BodyPart;
		int selectedBodyPartIndex;
		int selectedAvailableBodyPartIndex;

		Game.PartOfBody currentPartOfBody = Game.PartOfBody.None;

		List<Button> buttons = new();
		List<Button> availableButtons = new();

		List<Game.PartOfBody> bodyPartOrder = new() {
			Game.PartOfBody.Head,
			Game.PartOfBody.Torso,
			Game.PartOfBody.Tail,
			Game.PartOfBody.Appendage,
			Game.PartOfBody.Appendage,
			Game.PartOfBody.Appendage,
			Game.PartOfBody.Appendage,
			Game.PartOfBody.Appendage,
			Game.PartOfBody.Appendage,
		};

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
			switch (phase) {
				case FocusPhase.BodyPart:
					GoBack();
					break;

				case FocusPhase.AvailableBodyPart:
					GoBackToBodyPartList();
					break;
			}
		}


		// -------------------------------------------------------------------------

		void GoBackToBodyPartList() {
			phase = FocusPhase.BodyPart;

			//
			Game.Button.Select(buttons[selectedBodyPartIndex]);
		}

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

			//
			Game.Button.Select(buttons[0]);
		}

		void ConfigureBodyPartList() {
			BodyParts[0].Configure(editing.Creature.Head, "Head");
			BodyParts[1].Configure(editing.Creature.Torso, "Torso");
			BodyParts[2].Configure(editing.Creature.Tail, "Tail");

			for (int i = 3; i < BodyParts.Count; i++) {
				var button = BodyParts[i];

				var appendage = editing.Creature.GetAppendage(i - 3);
				if (appendage == null) {
					button.gameObject.SetActive(false);
					continue;
				}

				//
				button.Configure(appendage, editing.Creature.NameOfAppendage(i - 3));
			}
		}

		void ConfigureBodyPartButtons() {
			buttons.Clear();
			BodyParts.ForEach(AddBodyPartButtonIfActive);

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				//
				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up].GetComponent<Button>();
				navigation.selectOnDown = buttons[down].GetComponent<Button>();

				button.navigation = navigation;

				//
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => {
						selectedBodyPartIndex = j;
						DescribeBodyPart(BodyParts[j].BodyPartEntry);
						UpdateAvailableBodyPartList(bodyPartOrder[j]);
					});
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnBodyPartSelected);
			}
		}

		void DescribeBodyPart(Game.BodyPartEntry bodyPartEntry) {
		}

		void UpdateAvailableBodyPartList(Game.PartOfBody newPartOfBody) {
			if (newPartOfBody == currentPartOfBody) {
				return;
			}

			//
			availableButtons.Remove(RemoveButton);
			availableButtons.ForEach(child => {
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			});
			availableButtons.Clear();
			availableButtons.Add(RemoveButton);

			// 
			Engine.Profile.BodyParts
				.Where(bodyPartEntry => bodyPartEntry.BodyPart.Part == newPartOfBody)
				.ToList()
				.ForEach(bodyPartEntry => {
					var buttonGO = Instantiate(TemplateButton, AvailableBodyPartsList);
					buttonGO.SetActive(true);

					var bodyPartButton = buttonGO.GetComponent<BodyPartButton>();
					bodyPartButton.Configure(bodyPartEntry, "???");

					//
					var button = buttonGO.GetComponent<Button>();
					availableButtons.Add(button);
				});

			//
			currentPartOfBody = newPartOfBody;
		}

		void OnBodyPartSelected() {
			phase = FocusPhase.AvailableBodyPart;

			//
			Game.Button.Select(availableButtons[0]);
		}

		void OnAvailableBodyPartSelected() {
		}

		// -------------------------------------------------------------------------

		void AddBodyPartButtonIfActive(BodyPartButton button) {
			if (!button.gameObject.activeSelf) {
				return;
			}

			//
			buttons.Add(button.GetComponent<Button>());
		}

		// -------------------------------------------------------------------------

	}
}
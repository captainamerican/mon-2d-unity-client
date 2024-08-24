using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ----------------------------------------------------------------------------- 

namespace CreatureManager {
	public class CreaturesMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;

		[SerializeField] Transform CreaturesParent;
		[SerializeField] GameObject CreatureTemplate;

		[SerializeField] TextMeshProUGUI HeadLabel;
		[SerializeField] TextMeshProUGUI TorsoLabel;
		[SerializeField] TextMeshProUGUI TailLabel;
		[SerializeField] List<TextMeshProUGUI> AppendageLabels;
		[SerializeField] List<TextMeshProUGUI> SkillLabels;

		[SerializeField] TextMeshProUGUI Description;

		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		List<Button> buttons = new();
		int selectedButtonIndex = 0;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();
			BuildCreaturesList();
			FocusPreviouslySelectedButton();
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
			selectedButtonIndex = 0;

			//
			InitialMenu.gameObject.SetActive(true);

			//
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

		void BuildCreaturesList() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			buttons = Engine.Profile.Creatures
				.Select(creature => {
					var buttonGO = Instantiate(CreatureTemplate, CreaturesParent);
					buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = creature.Name;
					buttonGO.name = creature.Name;
					buttonGO.SetActive(true);

					//
					var button = buttonGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => EditCreature(creature));

					//
					return button;
				})
				.ToList();

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				int j = i;
				Game.ConstructedCreature creature = Engine.Profile.Creatures[i];

				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;

				//
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						selectedButtonIndex = j;
						DescribeCreature(creature);
					});
			}
		}

		void EditCreature(Game.ConstructedCreature creature) {
			EditInitialMenu.Configure(new EditingCreature {
				IsNew = false,
				Creature = creature.Clone(),
				Original = creature,
				AvailableHead = new(Engine.Profile.Storage.Head),
				AvailableTorso = new(Engine.Profile.Storage.Torso),
				AvailableTail = new(Engine.Profile.Storage.Tail),
				AvailableAppendage = new(Engine.Profile.Storage.Appendage)
			});
			EditInitialMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		void DescribeCreature(Game.ConstructedCreature creature) {
			Description.text = $@"{creature.Torso?.BodyPart?.LocomotionLabel ?? "Incomplete"}".Trim();

			HeadLabel.text = creature.Head?.BodyPart?.Name ?? HeadBodyPart.Label;
			TorsoLabel.text = creature.Torso?.BodyPart?.Name ?? TorsoBodyPart.Label;
			TailLabel.text = creature.Tail?.BodyPart?.Name ?? TailBodyPart.Label;

			for (int i = 0; i < AppendageLabels.Count; i++) {
				var label = AppendageLabels[i];

				if (i >= creature.Appendages.Count) {
					label.transform.parent.gameObject.SetActive(false);
					continue;
				}

				var appendage = creature.GetAppendage(i);
				label.text = appendage == null
					? creature.NameOfAppendage(i)
					: appendage.BodyPart.Name;
				label.transform.parent.gameObject.SetActive(true);
			}

			for (int i = 0; i < SkillLabels.Count; i++) {
				if (i >= (creature?.Skills?.Count ?? 0)) {
					SkillLabels[i].gameObject.SetActive(false);
					continue;
				}

				//
				SkillLabels[i].text = creature.Skills[i].Name;
				SkillLabels[i].gameObject.SetActive(true);
			}
		}

		void FocusPreviouslySelectedButton() {
			if (buttons.Count < 1) {
				selectedButtonIndex = 0;
				return;
			}

			//
			if (selectedButtonIndex >= buttons.Count) {
				selectedButtonIndex = buttons.Count - 1;
			}

			Game.Btn.Select(buttons[selectedButtonIndex]);
		}

		// -------------------------------------------------------------------------
	}
}
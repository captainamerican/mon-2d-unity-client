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
			RemoveAllCreatures();
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
			buttons.Clear();
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
						DescribeCreature(creature);
						selectedButtonIndex = j;
					});
			}
		}

		void EditCreature(Game.ConstructedCreature creature) {
			EditInitialMenu.Configure(new EditingCreature {
				IsNew = false,
				Changed = false,
				Creature = creature.Clone()
			});
			EditInitialMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		void DescribeCreature(Game.ConstructedCreature creature) {
			Description.text = $@"{creature.AppendagesLabel()}".Trim();

			HeadLabel.text = creature?.Head?.BodyPart?.Name ?? "(Head)";
			TorsoLabel.text = creature?.Torso?.BodyPart?.Name ?? "(Torso)";
			TailLabel.text = creature?.Tail?.BodyPart?.Name ?? "(Tail)";

			for (int i = 0; i < AppendageLabels.Count; i++) {
				var label = AppendageLabels[i];

				if (i >= (creature?.Appendages?.Count ?? 0)) {
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
				return;
			}

			//
			Game.Button.Select(buttons[selectedButtonIndex]);
		}

		void RemoveAllCreatures() {
			List<GameObject> toRemove = new();
			foreach (Transform child in CreaturesParent) {
				if (child.gameObject == CreatureTemplate) {
					continue;
				}

				toRemove.Add(child.gameObject);
			}

			toRemove.ForEach(child => {
				child.SetActive(false);
				Destroy(child);
			});
		}

		// -------------------------------------------------------------------------
	}
}
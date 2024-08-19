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
		[SerializeField] TextMeshProUGUI LeftFrontAppendage;
		[SerializeField] TextMeshProUGUI LeftMiddleAppendage;
		[SerializeField] TextMeshProUGUI LeftRearAppendage;
		[SerializeField] TextMeshProUGUI RightFrontAppendage;
		[SerializeField] TextMeshProUGUI RightMiddleAppendage;
		[SerializeField] TextMeshProUGUI RightRearAppendage;

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
			HeadLabel.text = creature?.Head?.BodyPart?.Name ?? "(none)";
			TorsoLabel.text = creature?.Torso?.BodyPart?.Name ?? "(none)";
			TailLabel.text = creature?.Tail?.BodyPart?.Name ?? "(none)";
			LeftFrontAppendage.text = creature?.LeftFrontAppendage?.BodyPart?.Name ?? "(none)";
			LeftMiddleAppendage.text = creature?.LeftMiddleAppendage?.BodyPart?.Name ?? "(none)";
			LeftRearAppendage.text = creature?.LeftRearAppendage?.BodyPart?.Name ?? "(none)";
			RightFrontAppendage.text = creature?.RightFrontAppendage?.BodyPart?.Name ?? "(none)";
			RightMiddleAppendage.text = creature?.RightMiddleAppendage?.BodyPart?.Name ?? "(none)";
			RightRearAppendage.text = creature?.RightRearAppendage?.BodyPart?.Name ?? "(none)";

			Description.text = $@"{creature.AppendagesLabel()}".Trim();

			switch (creature?.Torso.BodyPart.Base.Appendages ?? Game.NumberOfAppendages.None) {
				case Game.NumberOfAppendages.OneLowerNoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(false);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(false);
					RightFrontAppendage.transform.parent.gameObject.SetActive(false);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(false);
					break;

				case Game.NumberOfAppendages.OneLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(false);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(false);
					break;

				case Game.NumberOfAppendages.TwoLowerNoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(false);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(false);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.TwoLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.FourLower:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.FourLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(true);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(true);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case Game.NumberOfAppendages.SixLower:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(true);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(true);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;
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
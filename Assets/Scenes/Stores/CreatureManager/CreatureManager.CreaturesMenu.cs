using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CreatureManager {
	public class CreaturesMenu : MonoBehaviour {
		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] StartMenu StartMenu;
		[SerializeField] EditStart EditStartMenu;

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

		InputAction Cancel;

		List<Button> buttons = new();

		void OnEnable() {
			ConfigureCancelAction();
			RemoveAllCreatures();
			BuildCreaturesList();
			FocusFirstCreature();
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

		void GoBack() {
			StartMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

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
					buttonGO.GetComponent<InformationButton>().Configure(() => DescribeCreature(creature));

					//
					var button = buttonGO.GetComponent<Button>();
					button.onClick.AddListener(() => EditCreature(creature));

					//
					return button;
				})
				.ToList();

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;
			}
		}

		void EditCreature(ConstructedCreature creature) {
			EditStartMenu.Configure(creature);
			EditStartMenu.gameObject.SetActive(true);

			gameObject.SetActive(false);
		}

		void DescribeCreature(ConstructedCreature creature) {
			HeadLabel.text = creature?.Head?.Name ?? "(none)";
			TorsoLabel.text = creature?.Torso?.Name ?? "(none)";
			TailLabel.text = creature?.Tail?.Name ?? "(none)";
			LeftFrontAppendage.text = creature?.LeftFrontAppendage?.Name ?? "(none)";
			LeftMiddleAppendage.text = creature?.LeftMiddleAppendage?.Name ?? "(none)";
			LeftRearAppendage.text = creature?.LeftRearAppendage?.Name ?? "(none)";
			RightFrontAppendage.text = creature?.RightFrontAppendage?.Name ?? "(none)";
			RightMiddleAppendage.text = creature?.RightMiddleAppendage?.Name ?? "(none)";
			RightRearAppendage.text = creature?.RightRearAppendage?.Name ?? "(none)";

			Description.text = $@"{creature.AppendagesLabel()}".Trim();

			switch (creature?.Torso?.Base?.Appendages ?? NumberOfAppendages.None) {
				case NumberOfAppendages.OneLowerNoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(false);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(false);
					RightFrontAppendage.transform.parent.gameObject.SetActive(false);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(false);
					break;

				case NumberOfAppendages.OneLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(false);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(false);
					break;

				case NumberOfAppendages.TwoLowerNoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(false);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(false);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case NumberOfAppendages.TwoLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case NumberOfAppendages.FourLower:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(false);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(false);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case NumberOfAppendages.FourLowerTwoUpper:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(true);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(true);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;

				case NumberOfAppendages.SixLower:
					LeftFrontAppendage.transform.parent.gameObject.SetActive(true);
					LeftMiddleAppendage.transform.parent.gameObject.SetActive(true);
					LeftRearAppendage.transform.parent.gameObject.SetActive(true);
					RightFrontAppendage.transform.parent.gameObject.SetActive(true);
					RightMiddleAppendage.transform.parent.gameObject.SetActive(true);
					RightRearAppendage.transform.parent.gameObject.SetActive(true);
					break;
			}
		}

		void FocusFirstCreature() {
			buttons[0].Select();
			buttons[0].OnSelect(null);
			buttons[0].GetComponent<InformationButton>().OnSelect(null);
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
	}
}
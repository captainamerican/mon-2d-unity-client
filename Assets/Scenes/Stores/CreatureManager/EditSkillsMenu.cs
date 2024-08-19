using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class EditSkillsMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum ButtonPhase {
			SkillSelection,
			LearnedSkillSelection
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;
		[SerializeField] Transform LearnedSkillParent;
		[SerializeField] GameObject LearnedSkillTemplate;
		[SerializeField] List<Button> Skills;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		Game.ConstructedCreature creature;

		InputAction Cancel;

		List<Button> buttons = new();

		ButtonPhase buttonPhase = ButtonPhase.SkillSelection;
		int selectedSkillIndex = 0;
		int selectedLearnedSkillIndex = 0;

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();
			ConfigureLearnedSkills();
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
			selectedSkillIndex = 0;
			selectedLearnedSkillIndex = 0;

			//
			EditInitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		public void Configure(Game.ConstructedCreature creature) {
			this.creature = creature;

			//
			ConfigureCreatureSkills();

			//
			buttonPhase = ButtonPhase.SkillSelection;
			Skills[0].Select();
			Skills[0].OnSelect(null);
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

		void ConfigureCreatureSkills() {
			for (int i = 0; i < Skills.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				int j = i;
				Skill skill = i < creature.Skills.Count - 1
					? creature.Skills[i]
					: null;

				Button button = Skills[i];
				TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
				label.text = skill == null ? "-" : skill.Name;

				//
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						DescribeSkill(skill);
						selectedSkillIndex = j;
					});
			}
		}

		void ConfigureLearnedSkills() {
			RemoveAllLearnedSkills();

			buttons.Clear();
			buttons = Engine.Profile.Skills
				.Select(learnedSkill => {
					var buttonGO = Instantiate(
						LearnedSkillTemplate,
						LearnedSkillParent
					);

					var labels = buttonGO.GetComponentsInChildren<TextMeshProUGUI>();

					// 
					labels[0].text = learnedSkill.Skill.Name;

					//
					labels[1].text = "";
					Do.Times(3, i => {
						labels[1].text += learnedSkill.Experience >= learnedSkill.Skill.ExperienceToLearn * (i + 1)
							? "★"
							: "☆";
					});

					// too low-level
					if (learnedSkill.Experience < learnedSkill.Skill.ExperienceToLearn * 1) {
						labels.ToList().ForEach(label => {
							label.color = new Color(0, 0, 0, 0.5f);
						});
					}

					//
					float rawLevel = 3f * ((float) learnedSkill.Experience / (float) (learnedSkill.Skill.ExperienceToLearn * 3f));
					int level = Mathf.FloorToInt(rawLevel);
					int nextLevel = level < 3 ? level + 1 : 3;
					float ratio = (rawLevel - level);

					var images = buttonGO.GetComponentsInChildren<Image>();
					images[2].rectTransform.localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);

					//
					buttonGO.name = learnedSkill.Skill.Name;
					buttonGO.SetActive(true);

					//
					return buttonGO.GetComponent<Button>();
				})
				.ToList();

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				int j = i;
				Game.LearnedSkill learnedSkill = Engine.Profile.Skills[i];

				//
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
						DescribeSkill(learnedSkill.Skill);
						selectedLearnedSkillIndex = j;
					});

				//
				button.onClick.AddListener(() => SetSkill(learnedSkill.Skill));
			}
		}

		void SetSkill(Skill skill) {
		}

		void DescribeSkill(Skill skill) {
		}

		void RemoveAllLearnedSkills() {
			List<GameObject> toRemove = new();
			foreach (Transform child in LearnedSkillParent) {
				if (child.gameObject == LearnedSkillTemplate) {
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
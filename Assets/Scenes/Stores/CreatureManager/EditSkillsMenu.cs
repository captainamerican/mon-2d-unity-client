using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
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
		[SerializeField] GameObject Scrollbar;
		[SerializeField] RectTransform ScrollbarThumb;
		[SerializeField] Button RemoveSkillButton;

		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		Game.ConstructedCreature creature;

		InputAction Cancel;

		List<Button> buttons = new();

		ButtonPhase buttonPhase = ButtonPhase.SkillSelection;
		int selectedSkillIndex = 0;
		int selectedLearnedSkillIndex = 0;

		const int totalVisibleButtons = 6;
		int visibleButtonRangeMin = 0;
		int visibleButtonRangeMax = totalVisibleButtons;

		readonly Dictionary<Skill, bool> notYetLearned = new();

		// -------------------------------------------------------------------------

		void OnEnable() {
			ConfigureCancelAction();
			ConfigureLearnedSkills();
			UpdateVisibleButtonRange(0);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= HandleCancelAction;
		}

		void HandleCancelAction(InputAction.CallbackContext ctx) {
			switch (buttonPhase) {
				case ButtonPhase.LearnedSkillSelection:
					buttonPhase = ButtonPhase.SkillSelection;

					Skills[0].Select();
					Skills[0].OnSelect(null);
					break;

				case ButtonPhase.SkillSelection:
					GoBack();
					break;
			}
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
				Cancel.performed -= HandleCancelAction;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += HandleCancelAction;
		}

		void ConfigureCreatureSkills() {
			notYetLearned.Clear();
			Engine.Profile.Skills.ForEach(ls => {
				notYetLearned[ls.Skill] = ls.Experience < ls.Skill.ExperienceToLearn;
			});

			//
			List<Button> creatureButtons = new();

			for (int i = 0; i < Skills.Count; i++) {
				int j = i;
				Skill skill = creature.Skills.Count > i
					? creature.Skills[i]
					: null;

				Button button = Skills[i];
				TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
				label.text = skill == null ? "-" : skill.Name;
				label.color = Color.black;

				if (skill != null && notYetLearned[skill]) {
					label.color = new Color(0, 0, 0, 0.5f);
				}

				//
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						DescribeSkill(skill);
						selectedSkillIndex = j;
					});

				button
					.onClick
					.RemoveAllListeners();

				if (skill == null || (skill != null && !notYetLearned[skill])) {
					button
						.onClick
						.AddListener(() => {
							buttonPhase = ButtonPhase.LearnedSkillSelection;

							buttons[selectedLearnedSkillIndex].Select();
							buttons[selectedLearnedSkillIndex].OnSelect(null);
						});
				}

				//
				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = null;
				navigation.selectOnDown = null;

				button.navigation = navigation;

				// 
				if (i <= creature.Skills.Count) {
					creatureButtons.Add(button);
				}
			}

			for (int i = 0; i < creatureButtons.Count; i++) {
				int up = i == 0 ? creatureButtons.Count - 1 : i - 1;
				int down = i == creatureButtons.Count - 1 ? 0 : i + 1;

				//
				Button button = creatureButtons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = creatureButtons[up];
				navigation.selectOnDown = creatureButtons[down];

				button.navigation = navigation;
			}
		}

		void ConfigureLearnedSkills() {
			RemoveAllLearnedSkills();

			//
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
			buttons.Insert(0, RemoveSkillButton);

			//
			for (int i = 0; i < buttons.Count; i++) {
				int up = i == 0 ? buttons.Count - 1 : i - 1;
				int down = i == buttons.Count - 1 ? 0 : i + 1;

				//
				Button button = buttons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = buttons[up];
				navigation.selectOnDown = buttons[down];

				button.navigation = navigation;

				//
				if (button == RemoveSkillButton) {
					continue;
				}

				//
				int j = i;
				Game.LearnedSkill learnedSkill = Engine.Profile.Skills[i];

				//
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						selectedLearnedSkillIndex = j;

						//
						DescribeSkill(learnedSkill.Skill);
						UpdateVisibleButtonRange(j);
					});

				//
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => SetSkill(learnedSkill.Skill));
			}
		}

		void SetSkill(Skill skill) {
			Skills[selectedSkillIndex].Select();
			Skills[selectedSkillIndex].OnSelect(null);

			//
			if (notYetLearned[skill]) {
				return;
			}

			//
			if (creature.Skills.Contains(skill)) {
				int index = creature.Skills.IndexOf(skill);
				if (index == selectedSkillIndex) {
					return;
				}

				Skill otherSkill = creature.GetSkillAt(selectedSkillIndex);
				if (otherSkill == null) {
					creature.Skills.RemoveAt(index);
					creature.Skills.Add(skill);
				} else {
					creature.Skills[index] = otherSkill;
					creature.Skills[selectedSkillIndex] = skill;
				}
			} else {
				creature.Skills.Add(skill);
			}

			// 
			ConfigureCreatureSkills();

			//
			Skills[selectedSkillIndex].Select();
			Skills[selectedSkillIndex].OnSelect(null);
		}

		void DescribeSkill(Skill skill) {
		}

		void RemoveAllLearnedSkills() {
			List<GameObject> toRemove = new();
			foreach (Transform child in LearnedSkillParent) {
				if (
					child.gameObject == LearnedSkillTemplate ||
					child.gameObject == Scrollbar ||
					child.gameObject == RemoveSkillButton.gameObject
				) {
					continue;
				}

				toRemove.Add(child.gameObject);
			}

			toRemove.ForEach(child => {
				child.SetActive(false);
				Destroy(child);
			});
		}

		public void RemoveSkill() {
			Skill otherSkill = creature.GetSkillAt(selectedSkillIndex);
			if (otherSkill == null) {
				return;
			}

			creature.Skills.Remove(otherSkill);

			//
			ConfigureCreatureSkills();
		}

		// -------------------------------------------------------------------------

		void UpdateVisibleButtonRange(int index) {
			if (index < visibleButtonRangeMin) {
				visibleButtonRangeMin = index;
				visibleButtonRangeMax = index + totalVisibleButtons;
			} else if (index > visibleButtonRangeMax) {
				visibleButtonRangeMax = index;
				visibleButtonRangeMin = index - totalVisibleButtons;
			}

			//
			UpdateVisibleButtons();
			UpdateScrollbarThumb(index);
		}

		void UpdateVisibleButtons() {
			for (int i = 0; i < buttons.Count; i++) {
				bool enabled = i >= visibleButtonRangeMin && i <= visibleButtonRangeMax;
				var button = buttons[i].gameObject;
				if (button == null) {
					continue;
				}

				RectTransform rt = button.GetComponent<RectTransform>();
				Vector2 sizeDelta = rt.sizeDelta;
				sizeDelta.y = enabled ? 15 : 0;

				rt.sizeDelta = sizeDelta;

				foreach (Transform transform in button.transform) {
					transform.gameObject.SetActive(enabled);
				}
			}
		}

		void UpdateScrollbarThumb(int index) {
			ScrollbarThumb.gameObject.SetActive(buttons.Count > 0);

			if (buttons.Count > 0) {
				var parent = ScrollbarThumb.parent.GetComponent<RectTransform>();

				float parentHeight = Mathf.Ceil(parent.rect.height);
				float rawButtonHeight = buttons.Count > 1 ? parentHeight / buttons.Count : parentHeight;
				float buttonHeight = Mathf.Round(Mathf.Clamp(rawButtonHeight, 1f, parentHeight));
				float track = parentHeight - buttonHeight;
				float offset = buttons.Count > 1 ? Mathf.Ceil(track * ((float) index / ((float) (buttons.Count - 1)))) : 0;

				ScrollbarThumb.anchoredPosition = new Vector2(0, -offset);
				ScrollbarThumb.sizeDelta = new Vector2(2, buttonHeight);
			}
		}

		// -------------------------------------------------------------------------

	}
}
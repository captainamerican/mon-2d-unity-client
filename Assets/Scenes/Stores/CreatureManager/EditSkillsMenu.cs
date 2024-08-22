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
		[SerializeField] GameObject Scrollbar;
		[SerializeField] RectTransform ScrollbarThumb;
		[SerializeField] Button RemoveSkillButton;

		[Header("Move Information")]
		[SerializeField] GameObject MoveInformation;
		[SerializeField] TextMeshProUGUI MoveName;
		[SerializeField] TextMeshProUGUI MagicCost;
		[SerializeField] TextMeshProUGUI Grade;
		[SerializeField] Transform GradeProgress;
		[SerializeField] TextMeshProUGUI Tags;
		[SerializeField] TextMeshProUGUI MoveDescription;


		[Header("Menus")]
		[SerializeField] EditInitialMenu EditInitialMenu;

		// -------------------------------------------------------------------------

		EditingCreature editing;

		InputAction Cancel;

		List<Button> buttons = new();

		ButtonPhase buttonPhase = ButtonPhase.SkillSelection;
		int selectedSkillIndex = 0;
		int selectedLearnedSkillIndex = 0;

		const int totalVisibleButtons = 10;
		int visibleButtonRangeMin = 0;
		int visibleButtonRangeMax = totalVisibleButtons;

		int maxSkills = 0;

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
			EditInitialMenu.Configure(editing);

			//
			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;
			maxSkills = editing.Creature.HeadMaxSkills;

			//
			ConfigureCreatureSkills();

			//
			buttonPhase = ButtonPhase.SkillSelection;
			Game.Button.Select(Skills[0]);
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
				Skill skill = editing.Creature.Skills.Count > i
					? editing.Creature.Skills[i]
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
							Game.Button.Select(buttons[selectedLearnedSkillIndex]);
						});
				}

				//
				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = null;
				navigation.selectOnDown = null;

				button.navigation = navigation;

				// 
				if (i <= editing.Creature.Skills.Count && i < maxSkills) {
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

			//
			Do.Times(4, i => {
				if (i < maxSkills) {
					return;
				}

				Button button = Skills[i];
				var label = button.GetComponentInChildren<TextMeshProUGUI>();
				label.text = $"(Head Grade {i})";
				label.color = new Color(0, 0, 0, 0.5f);
			});
		}

		void ConfigureLearnedSkills() {
			RemoveAllLearnedSkills();

			// 
			buttons.Clear();
			buttons = Engine.Profile.Skills
				.OrderByDescending(ls => ls.Experience >= ls.Skill.ExperienceToLearn)
				.ThenBy(ls => ls.Skill.Name)
				.Select(learnedSkill => {
					var buttonGO = Instantiate(
						LearnedSkillTemplate,
						LearnedSkillParent
					);

					buttonGO.GetComponent<SkillButton>()
						.Configure(learnedSkill);

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
				Game.LearnedSkill learnedSkill = button
					.GetComponent<SkillButton>()
					.LearnedSkill;

				//
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						selectedLearnedSkillIndex = j;

						//
						DescribeSkill(learnedSkill);
						UpdateVisibleButtonRange(j);
					});

				//
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => SetSkill(learnedSkill.Skill));
			}

			//
			RemoveSkillButton.GetComponent<InformationButton>()
				.Configure(() => {
					selectedLearnedSkillIndex = 0;

					//
					DescribeSkill();
					UpdateVisibleButtonRange(0);
				});

			//
			UpdateVisibleButtonRange(0);
		}

		void SetSkill(Skill skill) {
			buttonPhase = ButtonPhase.SkillSelection;
			Game.Button.Select(Skills[selectedSkillIndex]);

			//
			if (notYetLearned[skill]) {
				return;
			}

			//
			if (editing.Creature.Skills.Contains(skill)) {
				int index = editing.Creature.Skills.IndexOf(skill);
				if (index == selectedSkillIndex) {
					return;
				}

				//
				Skill otherSkill = editing.Creature.GetSkillAt(selectedSkillIndex);
				if (otherSkill == null) {
					editing.Creature.Skills.RemoveAt(index);
					editing.Creature.Skills.Add(skill);
				} else {
					editing.Creature.Skills[index] = otherSkill;
					editing.Creature.Skills[selectedSkillIndex] = skill;
				}
			} else {
				if (selectedSkillIndex < editing.Creature.Skills.Count) {
					editing.Creature.Skills[selectedSkillIndex] = skill;
				} else {
					editing.Creature.Skills.Add(skill);
				}
			}

			// 
			ConfigureCreatureSkills();

			// 
			Game.Button.Select(Skills[selectedSkillIndex]);

			editing.Changed = true;
		}

		void DescribeSkill(Skill skill = null) {
			DescribeSkill(
				Engine.Profile.Skills.Find(ls => ls.Skill == skill)
				?? new Game.LearnedSkill { Skill = skill }
			);
		}

		void DescribeSkill(Game.LearnedSkill learnedSkill) {
			MoveInformation.SetActive(learnedSkill?.Skill != null);
			if (learnedSkill?.Skill == null) {
				return;
			}

			//
			int experience = learnedSkill.Experience;
			int toLevel = learnedSkill.Skill.ExperienceToLearn;

			float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
			int level = Mathf.FloorToInt(rawLevel);
			int nextLevel = level < 3 ? level + 1 : 3;
			float ratio = level < 3 ? (rawLevel - level) : 1;

			//
			MoveName.text = learnedSkill.Skill.Name;
			MagicCost.text = $"{learnedSkill.Skill.Cost}";
			MoveDescription.text = learnedSkill.Skill.Description;
			GradeProgress.localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);
			Grade.text = string.Join(
				"",
				Do.Times(
					3,
					i => experience >= toLevel * (i + 1) ? "★" : "☆"
				)
			);
			Tags.text = "(none)";
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
			buttonPhase = ButtonPhase.SkillSelection;
			Game.Button.Select(Skills[selectedSkillIndex]);

			//
			Skill otherSkill = editing.Creature.GetSkillAt(selectedSkillIndex);
			if (otherSkill == null) {
				return;
			}

			editing.Creature.Skills.Remove(otherSkill);
			editing.Changed = true;

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
				var button = buttons[i];
				var buttonGO = button.gameObject;
				if (buttonGO == null) {
					continue;
				}

				RectTransform rt = buttonGO.GetComponent<RectTransform>();

				Vector2 sizeDelta = rt.sizeDelta;
				sizeDelta.y = enabled
					? button == RemoveSkillButton
						? 8
						: 10
					: 0;

				rt.sizeDelta = sizeDelta;

				foreach (Transform transform in buttonGO.transform) {
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
				ScrollbarThumb.sizeDelta = new Vector2(4, buttonHeight);
			}
		}

		// -------------------------------------------------------------------------

	}
}
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class CreaturesMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum Phase {
			Normal,
			Switching,
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] List<Button> Creatures;
		[SerializeField] List<BodyPartButton> BodyParts;
		[SerializeField] List<BodyPartButton> Appendages;
		[SerializeField] List<SkillButton> Skills;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		Phase phase;
		int selectedCreatureIndex;
		int swapCreatureIndex;

		List<Button> buttons = new();

		// --------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			selectedCreatureIndex = 0;
			swapCreatureIndex = 0;

			//
			ConfigureInput();
			ConfigureCreatureButtons();

			// 
			Game.Focus.This(Creatures[0]);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			switch (phase) {
				case Phase.Normal:
					GoBack();
					break;

				case Phase.Switching:
					GoBackToCreatures();
					break;
			}
		}

		void GoBack() {
			InitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		void GoBackToCreatures() {
			phase = Phase.Normal;

			//
			Game.Focus.This(Creatures[selectedCreatureIndex]);
		}

		// ------------------------------------------------------------------------- 

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureCreatureButtons() {
			buttons.Clear();
			Do.ForEach(Creatures, (button, i) => {
				if (i >= Engine.Profile.Party.Count) {
					button.gameObject.SetActive(false);
					return;
				}

				// 
				string id = Engine.Profile.Party[i];
				Game.ConstructedCreature creature = Engine.Profile.Creatures.Find(
					creature => creature.Id == id
				);
				if (creature == null) {
					button.gameObject.SetActive(false);
					return;
				}

				//
				var label = button.GetComponentInChildren<TextMeshProUGUI>();
				label.text = creature.Name;

				//
				buttons.Add(button);
			});

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

				//
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => OnCreatureButtonHighlighted(j));

				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnCreatureButtonSelected);
			}
		}


		// -------------------------------------------------------------------------

		void OnCreatureButtonHighlighted(int index) {
			switch (phase) {
				case Phase.Normal:
					selectedCreatureIndex = index;
					break;
				case Phase.Switching:
					swapCreatureIndex = index;
					break;
			}

			//
			string id = Engine.Profile.Party[selectedCreatureIndex];
			Game.ConstructedCreature creature = Engine.Profile.Creatures.Find(
				creature => creature.Id == id
			);
			if (creature == null) {
				return;
			}

			// update body parts
			BodyParts[0].Configure(creature.Head);
			BodyParts[1].Configure(creature.Torso);
			BodyParts[2].Configure(creature.Tail);
			BodyParts[2].gameObject.SetActive(creature.Tail?.BodyPart != null);

			int appendageCount = creature.Torso.BodyPart.HowManyAppendages;
			Do.ForEach(Appendages, (button, i) => {
				button.gameObject.SetActive(i < appendageCount);
				if (i >= appendageCount) {
					return;
				}

				//
				button.Configure(creature.Appendages[i]);
			});


			// update skills
			Do.ForEach(Skills, (button, i) => {
				button.gameObject.SetActive(i < creature.Skills.Count);
				if (i >= creature.Skills.Count) {
					return;
				}

				Skill skill = creature.Skills[i];
				Game.SkillEntry skillEntry = Engine.Profile.Skills
					.Find(entry => entry.Skill == skill);

				button.GetComponent<SkillButton>()
					.Configure(skillEntry);
			});
		}

		void OnCreatureButtonSelected() {
			switch (phase) {
				case Phase.Normal: {
						phase = Phase.Switching;
						swapCreatureIndex = selectedCreatureIndex;

						//
						string id = Engine.Profile.Party[selectedCreatureIndex];
						Game.ConstructedCreature creature = Engine.Profile.Creatures.Find(
							creature => creature.Id == id
						);

						var button = buttons[selectedCreatureIndex];
						var label = button.GetComponentInChildren<TextMeshProUGUI>();

						label.text = $"⤭ {creature.Name}";
						break;
					}

				case Phase.Switching: {
						string id = Engine.Profile.Party[selectedCreatureIndex];
						Game.ConstructedCreature creature = Engine.Profile.Creatures.Find(
							creature => creature.Id == id
						);

						var button = buttons[selectedCreatureIndex];
						var label = button.GetComponentInChildren<TextMeshProUGUI>();
						label.text = creature.Name;

						//
						if (selectedCreatureIndex != swapCreatureIndex) {
							string swap = Engine.Profile.Party[selectedCreatureIndex];
							Engine.Profile.Party[selectedCreatureIndex] = Engine.Profile.Party[swapCreatureIndex];
							Engine.Profile.Party[swapCreatureIndex] = swap;

							//
							ConfigureCreatureButtons();
						}

						//
						phase = Phase.Normal;
						selectedCreatureIndex = swapCreatureIndex;
						Game.Focus.This(Creatures[selectedCreatureIndex]);
						break;
					}
			}
		}

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		// -------------------------------------------------------------------------
	}
}
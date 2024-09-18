using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace CreatureManager {
	public class EditPartyMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum FocusPhase {
			Normal,
			Swapping
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;

		[Header("Locals")]
		[SerializeField] PlayerInput PlayerInput;

		[Header("Party")]
		[SerializeField] GameObject Party;
		[SerializeField] List<Button> PartyButtons;
		[SerializeField] List<InformationButton> PartyInformationButtons;
		[SerializeField] List<CreatureNoHealthButton> PartyCreatureButtons;

		[Header("Available")]
		[SerializeField] GameObject AvailableCreatures;
		[SerializeField] GameObject AvailableCreatureTemplate;
		[SerializeField] ScrollView AvailableCreaturesScrollView;
		[SerializeField] Button RemoveButton;
		[SerializeField] InformationButton RemoveInformationButton;
		[SerializeField] CreatureNoHealthButton RemoveCreatureButton;



		[Header("Menus")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;

		FocusPhase phase = FocusPhase.Normal;
		List<Button> availableButtons = new();
		int selectedPartyIndex = 0;
		int selectedAvailableIndex = 0;


		// -------------------------------------------------------------------------  

		void OnEnable() {
			selectedPartyIndex = 0;

			//
			ConfigureCancelAction();
			ConfigureRemoveButton();
			ConfigurePartyButtons();

			GoBackToPartyList();
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			Cancel.performed -= OnGoBack;
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			switch (phase) {
				case FocusPhase.Normal:
					GoBack();
					break;

				case FocusPhase.Swapping:
					GoBackToPartyList();
					break;
			}
		}

		// -------------------------------------------------------------------------

		void GoBack() {
			selectedPartyIndex = 0;

			//
			InitialMenu.gameObject.SetActive(true);
			gameObject.SetActive(false);
		}

		void GoBackToPartyList() {
			AvailableCreatures.SetActive(false);
			Party.SetActive(true);

			Game.Focus.This(PartyButtons[selectedPartyIndex]);

			phase = FocusPhase.Normal;
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

		void ConfigureRemoveButton() {
			RemoveCreatureButton.Configure(null);
			RemoveInformationButton.Configure(RemoveCreatureButton.Display);
		}

		void ConfigurePartyButtons() {
			Do.ForEach(PartyButtons, (button, index) => {
				Game.Creature creature = Engine.Profile.GetPartyCreature(index);

				// 
				var creatureButton = PartyCreatureButtons[index];
				creatureButton.Configure(creature);

				PartyInformationButtons[index]
					.Configure(creatureButton.Display);

				// 
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnCreatureSelected(index));
			});
		}

		void ConfigureAvailableButtons() {
			availableButtons.Remove(RemoveButton);
			availableButtons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			availableButtons.Clear();

			//
			availableButtons.Add(RemoveButton);

			Do.ForEach(
				Engine.Profile.Creatures
					.Where(creature => !Engine.Profile.Party.Contains(creature.Id)),
				(creature, index) => {
					GameObject availableButtonGO = Instantiate(AvailableCreatureTemplate, AvailableCreatures.transform);
					availableButtonGO.SetActive(true);

					var creatureButton = availableButtonGO.GetComponent<CreatureNoHealthButton>();
					creatureButton.Configure(creature);

					//
					int j = index;
					availableButtonGO.GetComponent<InformationButton>()
						.Configure(() => {
							creatureButton.Display();
							AvailableCreaturesScrollView.UpdateVisibleButtonRange(availableButtons, j);
						});

					//
					Button button = availableButtonGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => OnAvailableCreatureSelected(creature));

					//
					availableButtons.Add(button);
				}
			);
		}

		// -------------------------------------------------------------------------

		void OnCreatureSelected(int index) {
			selectedPartyIndex = index;
			selectedAvailableIndex = 0;

			//
			ConfigureAvailableButtons();

			//
			Party.SetActive(false);
			AvailableCreatures.SetActive(true);

			//
			if (availableButtons.Count > 0) {
				Game.Focus.This(availableButtons[selectedAvailableIndex]);
			} else {
				AvailableCreaturesScrollView.UpdateVisibleButtonRange(availableButtons, 0);
				Game.Focus.Nothing();
			}

			//
			phase = FocusPhase.Swapping;
		}
		void OnAvailableCreatureSelected(Game.Creature creature) {
			Engine.Profile.SetPartyMember(selectedPartyIndex, creature.Id);

			ConfigurePartyButtons();
			GoBackToPartyList();
		}

		public void RemoveCreature() {
			Engine.Profile.RemovePartyMember(selectedPartyIndex);

			ConfigurePartyButtons();
			GoBackToPartyList();
		}

		// -------------------------------------------------------------------------

	}
}
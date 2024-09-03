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
		[SerializeField] GameObject Scrollbar;
		[SerializeField] RectTransform ScrollbarThumb;

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

		const int totalVisibleButtons = 8;
		int visibleButtonRangeMin = 0;
		int visibleButtonRangeMax = totalVisibleButtons;

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
			Game.Focus.This(buttons[selectedBodyPartIndex]);
			UpdateVisibleButtonRange(0);
		}

		void GoBack() {
			EditInitialMenu.gameObject.SetActive(true);
			EditInitialMenu.Configure(editing);

			gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------------

		public void Configure(EditingCreature editingCreature) {
			editing = editingCreature;

			//
			ConfigureBodyPartList();
			ConfigureBodyPartButtons();
			UpdateVisibleButtonRange(0);

			//
			Game.Focus.This(buttons[0]);
		}

		void ConfigureCancelAction() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureBodyPartList() {
			BodyParts[0].Configure(editing.Creature.Head);
			BodyParts[1].Configure(editing.Creature.Torso);
			BodyParts[2].Configure(editing.Creature.Tail);

			int numberOfAppendages = editing.Creature?.Torso?.BodyPart?.HowManyAppendages ?? 0;

			for (int i = 3; i < BodyParts.Count; i++) {
				var button = BodyParts[i];
				int j = i - 3;

				if (j >= numberOfAppendages) {
					button.gameObject.SetActive(false);
					continue;
				}

				//
				var appendage = editing.Creature.GetAppendage(j);

				button.Configure(appendage, editing.Creature.NameOfAppendage(j));
				button.gameObject.SetActive(true);
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
				Game.BodyPartEntry bodyPartEntryBase = button
					.GetComponent<BodyPartButton>()
					.BodyPartEntry;
				button.GetComponent<InformationButton>()
					.Configure(() => {
						selectedBodyPartIndex = j;
						DescribeBodyPart(bodyPartEntryBase);
						UpdateAvailableBodyPartList(bodyPartOrder[j]);
					});
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnBodyPartSelected);
			}
		}

		void DescribeBodyPart(Game.BodyPartEntry _) {
		}

		void UpdateAvailableBodyPartList(Game.PartOfBody newPartOfBody, bool forceRefresh = false) {
			if (
				!forceRefresh &&
				newPartOfBody == currentPartOfBody
			) {
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

			switch (newPartOfBody) {
				case Game.PartOfBody.Head:
					UpdateAvailableBodyParts(editing.AvailableHead);
					break;
				case Game.PartOfBody.Torso:
					UpdateAvailableBodyParts(editing.AvailableTorso);
					break;
				case Game.PartOfBody.Tail:
					UpdateAvailableBodyParts(editing.AvailableTail);
					break;
				case Game.PartOfBody.Appendage:
					UpdateAvailableBodyParts(editing.AvailableAppendage);
					break;
			}

			//
			UpdateAvailableButtons();
			UpdateVisibleButtonRange(0);

			//
			selectedAvailableBodyPartIndex = 0;
			currentPartOfBody = newPartOfBody;
		}

		void UpdateAvailableBodyParts(List<Game.HeadBodyPartEntry> available) {
			available
				.OrderByDescending(entry => entry.Experience)
				.ThenBy(entry => entry.BodyPart.Name)
				.ToList()
				.ForEach(bodyPartEntry => {
					var buttonGO = Instantiate(TemplateButton, AvailableBodyPartsList);
					buttonGO.SetActive(true);

					var bodyPartButton = buttonGO.GetComponent<BodyPartButton>();
					bodyPartButton.Configure(bodyPartEntry);

					//
					var button = buttonGO.GetComponent<Button>();
					availableButtons.Add(button);
				});
		}

		void UpdateAvailableBodyParts(List<Game.TorsoBodyPartEntry> available) {
			available
				.OrderByDescending(entry => entry.Experience)
				.ThenBy(entry => entry.BodyPart.Name)
				.ToList()
				.ForEach(bodyPartEntry => {
					var buttonGO = Instantiate(TemplateButton, AvailableBodyPartsList);
					buttonGO.SetActive(true);

					var bodyPartButton = buttonGO.GetComponent<BodyPartButton>();
					bodyPartButton.Configure(bodyPartEntry);

					//
					var button = buttonGO.GetComponent<Button>();
					availableButtons.Add(button);
				});
		}

		void UpdateAvailableBodyParts(List<Game.TailBodyPartEntry> available) {
			available
				.OrderByDescending(entry => entry.Experience)
				.ThenBy(entry => entry.BodyPart.Name)
				.ToList()
				.ForEach(bodyPartEntry => {
					var buttonGO = Instantiate(TemplateButton, AvailableBodyPartsList);
					buttonGO.SetActive(true);

					var bodyPartButton = buttonGO.GetComponent<BodyPartButton>();
					bodyPartButton.Configure(bodyPartEntry);

					//
					var button = buttonGO.GetComponent<Button>();
					availableButtons.Add(button);
				});
		}

		void UpdateAvailableBodyParts(List<Game.AppendageBodyPartEntry> available) {
			available
				.OrderByDescending(entry => entry.Experience)
				.ThenBy(entry => entry.BodyPart.Name)
				.ToList()
				.ForEach(bodyPartEntry => {
					var buttonGO = Instantiate(TemplateButton, AvailableBodyPartsList);
					buttonGO.SetActive(true);

					var bodyPartButton = buttonGO.GetComponent<BodyPartButton>();
					bodyPartButton.Configure(bodyPartEntry);

					//
					var button = buttonGO.GetComponent<Button>();
					availableButtons.Add(button);
				});
		}

		void UpdateAvailableButtons() {
			for (int i = 0; i < availableButtons.Count; i++) {
				int up = i == 0 ? availableButtons.Count - 1 : i - 1;
				int down = i == availableButtons.Count - 1 ? 0 : i + 1;

				//
				Button button = availableButtons[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = availableButtons[up];
				navigation.selectOnDown = availableButtons[down];

				button.navigation = navigation;

				//
				int j = i;
				var bodyPartEntry = button
						.GetComponent<BodyPartButton>()
						.BodyPartEntry;
				button.GetComponent<InformationButton>()
					.Configure(() => {
						selectedAvailableBodyPartIndex = j;
						DescribeBodyPart(bodyPartEntry);
						UpdateVisibleButtonRange(selectedAvailableBodyPartIndex);
					});

				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(
					(button == RemoveButton)
						? RemoveBodyPart
						: () => SetBodyPart(bodyPartEntry)
				);
			}
		}

		void OnBodyPartSelected() {
			phase = FocusPhase.AvailableBodyPart;
			Game.Focus.This(availableButtons[0]);
		}

		void SetBodyPart(Game.BodyPartEntry entry) {
			switch (selectedBodyPartIndex) {
				case 0:
					Game.HeadBodyPartEntry headEntry = (Game.HeadBodyPartEntry) entry;
					editing.AvailableHead.Remove(headEntry);
					if (editing.Creature.Head != null) {
						editing.AvailableHead.Add(editing.Creature.Head);
					}
					editing.Creature.Head = headEntry;
					break;
				case 1:
					Game.TorsoBodyPartEntry torsoEntry = (Game.TorsoBodyPartEntry) entry;
					editing.AvailableTorso.Remove(torsoEntry);
					if (editing.Creature.Torso != null) {
						editing.AvailableTorso.Add(editing.Creature.Torso);
					}
					editing.Creature.Torso = torsoEntry;

					//
					editing.Creature.Appendages.ForEach(
						entry => {
							if (entry != null) {
								editing.AvailableAppendage.Add(entry);
							}
						});
					editing.Creature.Appendages.Clear();
					Do.Times(
						torsoEntry.BodyPart.HowManyAppendages,
						() => editing.Creature.Appendages.Add(null)
					);
					break;
				case 2:
					Game.TailBodyPartEntry tailEntry = (Game.TailBodyPartEntry) entry;
					editing.AvailableTail.Remove(tailEntry);
					if (editing.Creature.Tail != null) {
						editing.AvailableTail.Add(editing.Creature.Tail);
					}
					editing.Creature.Tail = tailEntry;
					break;
				default:
					int appendageIndex = selectedBodyPartIndex - 3;
					Game.AppendageBodyPartEntry appendageEntry = (Game.AppendageBodyPartEntry) entry;
					editing.AvailableAppendage.Remove(appendageEntry);
					var currentEntry = editing.Creature.GetAppendage(appendageIndex);
					if (currentEntry != null) {
						editing.AvailableAppendage.Add(currentEntry);
					}
					editing.Creature.Appendages[appendageIndex] = appendageEntry;
					break;
			}

			//
			ConfigureBodyPartList();
			ConfigureBodyPartButtons();
			UpdateAvailableBodyPartList(currentPartOfBody, true);

			phase = FocusPhase.BodyPart;
			selectedAvailableBodyPartIndex = 0;
			Game.Focus.This(buttons[selectedBodyPartIndex]);
		}

		void RemoveBodyPart() {
			switch (selectedBodyPartIndex) {
				case 0:
					if (editing.Creature.Head != null) {
						editing.AvailableHead.Add(editing.Creature.Head);
						editing.Creature.Head = null;
					}
					break;
				case 1:
					if (editing.Creature.Torso != null) {
						editing.AvailableTorso.Add(editing.Creature.Torso);
						editing.Creature.Torso = null;

						//
						editing.Creature.Appendages.ForEach(
							entry => {
								if (entry != null) {
									editing.AvailableAppendage.Add(entry);
								}
							});
						editing.Creature.Appendages.Clear();
					}
					break;
				case 2:
					if (editing.Creature.Tail != null) {
						editing.AvailableTail.Add(editing.Creature.Tail);
						editing.Creature.Tail = null;
					}
					break;
				default:
					int appendageIndex = selectedBodyPartIndex - 3;
					var appendage = editing.Creature.GetAppendage(appendageIndex);
					if (appendage != null) {
						editing.AvailableAppendage.Add(appendage);
						editing.Creature.Appendages[appendageIndex] = null;
					}
					break;
			}

			//
			ConfigureBodyPartList();
			ConfigureBodyPartButtons();
			UpdateAvailableBodyPartList(currentPartOfBody, true);

			//
			phase = FocusPhase.BodyPart;
			Game.Focus.This(buttons[selectedBodyPartIndex]);
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
			for (int i = 0; i < availableButtons.Count; i++) {
				bool enabled = i >= visibleButtonRangeMin && i <= visibleButtonRangeMax;
				var button = availableButtons[i];
				var buttonGO = button.gameObject;
				if (buttonGO == null) {
					continue;
				}

				RectTransform rt = buttonGO.GetComponent<RectTransform>();

				Vector2 sizeDelta = rt.sizeDelta;
				sizeDelta.y = enabled
					? 10
					: 1;

				rt.sizeDelta = sizeDelta;

				foreach (Transform transform in buttonGO.transform) {
					transform.gameObject.SetActive(enabled);
				}
			}
		}

		void UpdateScrollbarThumb(int index) {
			ScrollbarThumb.gameObject.SetActive(availableButtons.Count > 0);

			if (availableButtons.Count > 0) {
				var parent = ScrollbarThumb.parent.GetComponent<RectTransform>();

				float parentHeight = Mathf.Ceil(parent.rect.height);
				float rawButtonHeight = availableButtons.Count > 1 ? parentHeight / availableButtons.Count : parentHeight;
				float buttonHeight = Mathf.Round(Mathf.Clamp(rawButtonHeight, 1f, parentHeight));
				float track = parentHeight - buttonHeight;
				float offset = buttons.Count > 1 ? Mathf.Ceil(track * ((float) index / ((float) (availableButtons.Count - 1)))) : 0;

				ScrollbarThumb.anchoredPosition = new Vector2(0, -offset);
				ScrollbarThumb.sizeDelta = new Vector2(4, buttonHeight);
			}
		}

		// -------------------------------------------------------------------------

	}
}
using System.Collections.Generic;
using System.Linq;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Menu {
	public class CompendiumMenu : MonoBehaviour {

		// -------------------------------------------------------------------------

		enum Phase {
			Normal,
			SubCategory,
			Expanded
		}

		// -------------------------------------------------------------------------

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] List<Button> Categories;
		[SerializeField] ScrollView ScrollView;
		[SerializeField] GameObject SubCategoryTemplate;
		[SerializeField] Transform SubCategoryParent;

		[SerializeField] TextMeshProUGUI ProgressLabel;
		[SerializeField] RectTransform ProgressRatio;

		[Header("Information Dialog")]
		[SerializeField] GameObject InformationDialog;
		[SerializeField] RectTransform InformationDialogRectTransform;

		[Header("Gameplay Information")]
		[SerializeField] GameObject TextInformationContainer;
		[SerializeField] TextMeshProUGUI TextInformation;

		[Header("Go Back")]
		[SerializeField] InitialMenu InitialMenu;

		// -------------------------------------------------------------------------

		InputAction Cancel;


		Phase phase;
		int selectedCategoryIndex;
		int selectedSubCategoryIndex;

		Dictionary<int, IEnumerable<Button>> subCategoryCache = new();
		readonly List<Button> subCategoryButtons = new();

		// --------------------------------------------------------------------------

		void OnEnable() {
			phase = Phase.Normal;
			selectedCategoryIndex = 0;
			selectedSubCategoryIndex = 0;

			//
			InformationDialog.SetActive(false);

			//
			ConfigureInput();
			ConfigureCategoryButtons();
			ScrollView.UpdateVisibleButtonRange(subCategoryButtons, 0);

			// 
			Game.Btn.Select(Categories[0]);
		}

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			subCategoryCache.Clear();

			//
			RemoveInputCallbacks();
		}

		void OnGoBack(InputAction.CallbackContext _) {
			switch (phase) {
				case Phase.Normal:
					GoBack();
					break;

				case Phase.SubCategory:
					GoBackToCategory();
					break;

				case Phase.Expanded:
					GoBackToSubCategory();
					break;
			}
		}

		void GoBack() {
			InitialMenu.gameObject.SetActive(true);

			//
			gameObject.SetActive(false);
		}

		void GoBackToCategory() {
			phase = Phase.Normal;
			InformationDialog.SetActive(false);

			//
			Game.Btn.Select(Categories[selectedCategoryIndex]);
		}

		void GoBackToSubCategory() {
			phase = Phase.SubCategory;
			InformationDialogRectTransform.sizeDelta = new Vector2(79, 67);

			//
			Game.Btn.Select(subCategoryButtons[selectedSubCategoryIndex]);
		}

		// ------------------------------------------------------------------------- 

		void ConfigureInput() {
			RemoveInputCallbacks();

			//
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureCategoryButtons() {
			for (int i = 0; i < Categories.Count; i++) {
				int up = i == 0 ? Categories.Count - 1 : i - 1;
				int down = i == Categories.Count - 1 ? 0 : i + 1;

				Button button = Categories[i];

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = Categories[up];
				navigation.selectOnDown = Categories[down];

				button.navigation = navigation;

				//
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => OnCategoryButtonHighlighted(j));

				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnCategoryButtonSelected);
			}
		}

		void ConfigureSubCategoryButtons() {
			foreach (var button in subCategoryButtons) {
				button.gameObject.SetActive(false);
			}

			//
			subCategoryButtons.Clear();

			//
			if (subCategoryCache.ContainsKey(selectedCategoryIndex)) {
				subCategoryButtons.AddRange(subCategoryCache[selectedCategoryIndex]);
				subCategoryButtons.ForEach(button => button.gameObject.SetActive(true));
			} else {
				switch (selectedCategoryIndex) {
					case 0:
						var bodyPartButtons = Engine.AllBodyParts
							.OrderBy(bodyPart => bodyPart.Name)
							.Select(bodyPart => {
								bool acquired = Engine.Profile.AcquiredBodyPart.Contains(bodyPart);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? bodyPart.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.BodyPart;
								compendiumButton.BodyPart = acquired ? bodyPart : null;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(bodyPartButtons);
						subCategoryCache.Add(selectedCategoryIndex, bodyPartButtons);
						break;

					case 1:
						var skillButtons = Engine.AllSkills
							.OrderBy(skill => skill.Name)
							.Select(skill => {
								bool acquired = Engine.Profile.AcquiredSkills.Contains(skill);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? skill.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.Skill;
								compendiumButton.Skill = acquired ? skill : null;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(skillButtons);
						subCategoryCache.Add(selectedCategoryIndex, skillButtons);
						break;

					case 2:
						var tagButtons = Engine.AllTags
							.OrderBy(tag => tag.Name)
							.Select(tag => {
								bool acquired = Engine.Profile.AcquiredTags.Contains(tag);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? tag.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.Tag;
								compendiumButton.Tag = acquired ? tag : null;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(tagButtons);
						subCategoryCache.Add(selectedCategoryIndex, tagButtons);
						break;

					case 3:
						var itemButtons = Engine.AllItems
							.OrderBy(item => item.Name)
							.Select(item => {
								bool acquired = Engine.Profile.AcquiredItem.Contains(item);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? item.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.Item;
								compendiumButton.Item = acquired ? item : null;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(itemButtons);
						subCategoryCache.Add(selectedCategoryIndex, itemButtons);
						break;

					case 4:
						var spiritWisdomButtons = Engine.AllSpiritWisdom
							.OrderBy(spiritWisdom => spiritWisdom.Name)
							.Select(spiritWisdom => {
								bool acquired = Engine.Profile.AcquiredSpiritWisdom.Contains(spiritWisdom);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? spiritWisdom.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.SpiritWisdom;
								compendiumButton.SpiritWisdom = spiritWisdom;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(spiritWisdomButtons);
						subCategoryCache.Add(selectedCategoryIndex, spiritWisdomButtons);
						break;

					case 5:
						var loreButtons = Engine.AllLore
							.OrderBy(lore => lore.Name)
							.Select(lore => {
								bool acquired = Engine.Profile.AcquiredLore.Contains(lore);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? lore.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.SpiritWisdom;
								compendiumButton.Lore = lore;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(loreButtons);
						subCategoryCache.Add(selectedCategoryIndex, loreButtons);
						break;

					case 6:
						var gameplayButtons = Engine.AllGameplay
							.OrderBy(gameplay => gameplay.Name)
							.Select(gameplay => {
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = gameplay.Name;

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.Gameplay;
								compendiumButton.Gameplay = gameplay;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(gameplayButtons);
						subCategoryCache.Add(selectedCategoryIndex, gameplayButtons);
						break;
				}
			}

			for (int i = 0; i < subCategoryButtons.Count; i++) {
				var button = subCategoryButtons[i];

				//
				int up = i == 0 ? subCategoryButtons.Count - 1 : i - 1;
				int down = i == subCategoryButtons.Count - 1 ? 0 : i + 1;

				Navigation navigation = button.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = subCategoryButtons[up];
				navigation.selectOnDown = subCategoryButtons[down];

				button.navigation = navigation;

				// 
				int j = i;
				button.GetComponent<InformationButton>()
					.Configure(() => OnSubCategoryButtonHighlighted(j));

				//
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnSubCategoryButtonSelected);

				//
				button.gameObject.SetActive(true);
			}
		}


		// -------------------------------------------------------------------------

		void OnCategoryButtonHighlighted(int index) {
			selectedCategoryIndex = index;

			//
			ConfigureSubCategoryButtons();
			UpdateProgress();
			ScrollView.UpdateVisibleButtonRange(subCategoryButtons, 0);
		}

		void OnCategoryButtonSelected() {
			if (subCategoryButtons.Count < 1) {
				return;
			}

			//
			phase = Phase.SubCategory;

			Game.Btn.Select(subCategoryButtons[0]);
		}

		void OnSubCategoryButtonHighlighted(int index) {
			selectedSubCategoryIndex = index;

			ScrollView.UpdateVisibleButtonRange(subCategoryButtons, selectedSubCategoryIndex);

			//
			TextInformationContainer.SetActive(false);

			//
			var button = subCategoryButtons[selectedSubCategoryIndex];
			var compendiumButton = button.GetComponent<CompendiumButton>();

			switch (compendiumButton.Type) {
				case CompendiumButtonType.BodyPart:
					if (!Engine.Profile.AcquiredBodyPart.Contains(compendiumButton.BodyPart)) {
						TextInformation.text = $"Body part not yet acquired.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.BodyPart.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.Skill:
					if (!Engine.Profile.AcquiredSkills.Contains(compendiumButton.Skill)) {
						TextInformation.text = $"Skill not yet observed.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.Skill.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.Item:
					if (!Engine.Profile.AcquiredItem.Contains(compendiumButton.Item)) {
						TextInformation.text = $"Item not yet acquired.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.Item.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.SpiritWisdom:
					if (!Engine.Profile.AcquiredSpiritWisdom.Contains(compendiumButton.SpiritWisdom)) {
						TextInformation.text = $"Spirit musing not yet heard.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = $"{compendiumButton.SpiritWisdom.Name}\n\n“{compendiumButton.SpiritWisdom.BattleStart}”\n\n“{compendiumButton.SpiritWisdom.BattleEnd}”";
						TextInformationContainer.SetActive(true);

					}
					break;
				case CompendiumButtonType.Lore:
					if (!Engine.Profile.AcquiredLore.Contains(compendiumButton.Lore)) {
						TextInformation.text = $"Lore not yet discovered.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = $"{compendiumButton.Lore.Name}\n\n{compendiumButton.Lore.Text}";
						TextInformationContainer.SetActive(true);

					}
					break;

				case CompendiumButtonType.Gameplay:
					TextInformation.text = $"{compendiumButton.Gameplay.Name}\n\n{compendiumButton.Gameplay.Information}";
					TextInformationContainer.SetActive(true);
					break;

				case CompendiumButtonType.Tag:
					if (!Engine.Profile.AcquiredTags.Contains(compendiumButton.Tag)) {
						TextInformation.text = $"Essense Tag not yet observed.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = $"{compendiumButton.Tag.Name}\n\n{compendiumButton.Tag.Description}";
						TextInformationContainer.SetActive(true);
					}
					break;
			}

			//
			InformationDialog.SetActive(true);
		}

		void OnSubCategoryButtonSelected() {
			phase = Phase.Expanded;

			//
			InformationDialogRectTransform.sizeDelta = new Vector2(160, 144);
		}

		void UpdateProgress() {
			int current = 0;
			int total = 0;

			//
			switch (selectedCategoryIndex) {
				case 0:
					current = Engine.Profile.AcquiredBodyPart.Count;
					total = Engine.AllBodyParts.Count;
					break;

				case 1:
					current = Engine.Profile.AcquiredItem.Count;
					total = Engine.AllSkills.Count;
					break;

				case 2:
					current = Engine.Profile.AcquiredTags.Count;
					total = Engine.AllTags.Count;
					break;

				case 3:
					current = Engine.Profile.AcquiredSkills.Count;
					total = Engine.AllItems.Count;
					break;

				case 4:
					current = Engine.Profile.AcquiredSpiritWisdom.Count;
					total = Engine.AllSpiritWisdom.Count;
					break;

				case 5:
					current = Engine.Profile.AcquiredLore.Count;
					total = Engine.AllLore.Count;
					break;

				case 6:
					current = Engine.AllGameplay.Count;
					total = current;
					break;
			}

			//
			float ratio = total > 0 ? (float) current / (float) total : 0;
			ProgressLabel.text = $"{current} / {total} ({Mathf.RoundToInt(ratio * 100):n0}%)";
			ProgressRatio.localScale = new Vector3(Mathf.Clamp01(ratio), 1, 1);
		}

		void RemoveInputCallbacks() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		// -------------------------------------------------------------------------
	}
}
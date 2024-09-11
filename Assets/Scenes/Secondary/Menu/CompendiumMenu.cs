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
			Game.Focus.This(Categories[0]);
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
			Game.Focus.This(Categories[selectedCategoryIndex]);
		}

		void GoBackToSubCategory() {
			phase = Phase.SubCategory;
			InformationDialogRectTransform.sizeDelta = new Vector2(79, 67);

			//
			Game.Focus.This(subCategoryButtons[selectedSubCategoryIndex]);
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
						var bodyPartButtons = Engine.GameData.BodyParts
							.OrderBy(bodyPart => bodyPart.Name)
							.Select(bodyPart => {
								bool acquired = Engine.Profile.Acquired.Has(bodyPart.Id);

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
						var skillButtons = Engine.GameData.Skills
							.OrderBy(skill => skill.Name)
							.Select(skill => {
								bool acquired = Engine.Profile.Acquired.Has(skill.Id);

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
						var tagButtons = Engine.GameData.Tags
							.OrderBy(tag => tag.Name)
							.Select(tag => {
								bool acquired = Engine.Profile.Acquired.Has(tag.Id);

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
						var itemButtons = Engine.GameData.Items
							.OrderBy(item => item.Name)
							.Select(item => {
								bool acquired = Engine.Profile.Acquired.Has(item.Id);

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
						var spiritWisdomButtons = Engine.GameData.SpiritWisdom
							.OrderBy(spiritWisdom => spiritWisdom.Name)
							.Select(spiritWisdom => {
								bool acquired = Engine.Profile.Acquired.Has(spiritWisdom.Id);

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
						var loreButtons = Engine.GameData.Lore
							.OrderBy(lore => lore.Name)
							.Select(lore => {
								bool acquired = Engine.Profile.Acquired.Has(lore.Id);

								//
								GameObject buttonGO = Instantiate(SubCategoryTemplate, SubCategoryParent);

								//
								TextMeshProUGUI label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
								label.text = acquired ? lore.Name : "--";

								//
								CompendiumButton compendiumButton = buttonGO.GetComponent<CompendiumButton>();
								compendiumButton.Type = CompendiumButtonType.Lore;
								compendiumButton.Lore = lore;

								//
								return buttonGO.GetComponent<Button>();
							});
						subCategoryButtons.AddRange(loreButtons);
						subCategoryCache.Add(selectedCategoryIndex, loreButtons);
						break;

					case 6:
						var gameplayButtons = Engine.GameData.Gameplay
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

			Game.Focus.This(subCategoryButtons[0]);
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
					if (!Engine.Profile.Acquired.Has(compendiumButton.BodyPart.Id)) {
						TextInformation.text = $"Body part not yet acquired.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.BodyPart.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.Skill:
					if (!Engine.Profile.Acquired.Has(compendiumButton.Skill.Id)) {
						TextInformation.text = $"Skill not yet observed.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.Skill.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.Item:
					if (!Engine.Profile.Acquired.Has(compendiumButton.Item.Id)) {
						TextInformation.text = $"Item not yet acquired.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = compendiumButton.Item.Name;
						TextInformationContainer.SetActive(true);
					}
					break;
				case CompendiumButtonType.SpiritWisdom:
					if (!Engine.Profile.Acquired.Has(compendiumButton.SpiritWisdom.Id)) {
						TextInformation.text = $"Spirit musing not yet heard.";
						TextInformationContainer.SetActive(true);
					} else {
						TextInformation.text = $"{compendiumButton.SpiritWisdom.Name}\n\n“{compendiumButton.SpiritWisdom.BattleStart}”\n\n“{compendiumButton.SpiritWisdom.BattleEnd}”";
						TextInformationContainer.SetActive(true);

					}
					break;
				case CompendiumButtonType.Lore:
					if (!Engine.Profile.Acquired.Has(compendiumButton.Lore.Id)) {
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
					if (!Engine.Profile.Acquired.Has(compendiumButton.Tag.Id)) {
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
					current = Engine.Profile.Acquired.BodyPart.Count;
					total = Engine.GameData.BodyParts.Count;
					break;

				case 1:
					current = Engine.Profile.Acquired.Skill.Count;
					total = Engine.GameData.Skills.Count;
					break;

				case 2:
					current = Engine.Profile.Acquired.Tag.Count;
					total = Engine.GameData.Tags.Count;
					break;

				case 3:
					current = Engine.Profile.Acquired.Item.Count;
					total = Engine.GameData.Items.Count;
					break;

				case 4:
					current = Engine.Profile.Acquired.SpiritWisdom.Count;
					total = Engine.GameData.SpiritWisdom.Count;
					break;

				case 5:
					current = Engine.Profile.Acquired.Lore.Count;
					total = Engine.GameData.Lore.Count;
					break;

				case 6:
					current = Engine.GameData.Gameplay.Count;
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
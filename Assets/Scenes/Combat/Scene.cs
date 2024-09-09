using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Combat {

	// ---------------------------------------------------------------------------

	public enum BattleResult {
		None = 0,
		Won = 1,
		Lost = 2,
		Fled = 3
	}

	public enum BattleActionType {
		None = 0,
		Item = 1,
		Move = 2
	}


	[Serializable]
	public class Battle {
		public Game.Creature Creature;
		public Game.SpiritId SpiritId;
		public Action<BattleResult> OnDone;
		public bool CantFlee;
	}

	public class BattleAction {
		public BattleActionType Type;
		public Game.ApplicableTarget Target;
		public Game.Creature Actor;
		public Game.Creature Receiver;

		public Item Item;
		public Game.SkillEntry SkillEntry;
	}

	// ---------------------------------------------------------------------------

	public class Scene : MonoBehaviour {

		// -------------------------------------------------------------------------

		static readonly public string Name = "Combat";
		static Battle Battle = null;

		static public IEnumerator Load(Battle battle) {
			Battle = battle;

			//
			yield return SceneManager.LoadSceneAsync(Name, LoadSceneMode.Additive);
		}

		static public IEnumerator Unload() {
			yield return SceneManager.UnloadSceneAsync(Name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}

		// -------------------------------------------------------------------------

#if UNITY_EDITOR
		[Header("Debug")]
		[SerializeField] Battle DebugBattle;
#endif

		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Locals")]
		[SerializeField] GameObject Spirit;
		[SerializeField] RectTransform PlayerCreatureContainer;
		[SerializeField] CanvasGroup PlayerCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen PlayerCombatant;
		[SerializeField] RectTransform EnemyCreatureContainer;
		[SerializeField] CanvasGroup EnemyCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen EnemyCombatant;

		[Header("Actions List")]
		[SerializeField] GameObject ActionListContainer;
		[SerializeField] List<Button> ActionButtons;

		[Header("Moves List")]
		[SerializeField] GameObject MoveListContainer;
		[SerializeField] Transform MoveButtonsParent;
		[SerializeField] GameObject MoveButtonsTemplate;
		[SerializeField] TextMeshProUGUI MoveMagicCost;
		[SerializeField] TextMeshProUGUI MoveGrade;
		[SerializeField] Transform MoveGradeProgress;
		[SerializeField] TextMeshProUGUI MoveTags;
		[SerializeField] TextMeshProUGUI MoveDescription;

		[Header("Items List")]
		[SerializeField] GameObject ItemListContainer;
		[SerializeField] ScrollView ItemListScrollView;
		[SerializeField] Transform ItemButtonsParent;
		[SerializeField] GameObject ItemButtonsTemplate;

		[Header("Creatures List")]
		[SerializeField] GameObject CreaturesList;
		[SerializeField] Transform CreatureButtonsParent;
		[SerializeField] GameObject CreatureButtonsTemplate;

		[Header("Confirm Flee Dialog")]
		[SerializeField] GameObject ConfirmFleeDialog;
		[SerializeField] Button ConfirmFleeCancelButton;

		[Header("Exit Cover")]
		[SerializeField] GameObject ExitCover;
		[SerializeField] CanvasGroup ExitCoverCanvasGroup;

		[Header("Targets List")]
		[SerializeField] GameObject TargetsListContainer;
		[SerializeField] GameObject TargetButtonTemplate;


		// -------------------------------------------------------------------------

		readonly Color BlackFaded = new(0, 0, 0, 0.5f);

		readonly List<Button> buttons = new();

		InputAction Cancel;
		Action onBack = () => Debug.Log("Nothing set");

		int selectedActionIndex;
		int selectedCreatureIndex;
		float dexterityPenalty;

		// ------------------------------------------------------------------------- 

		void OnDisable() {
			OnDestroy();
		}

		void OnDestroy() {
			if (Cancel != null) {
				Cancel.performed -= OnGoBack;
			}
		}

		void OnGoBack(InputAction.CallbackContext ctx) {
			GoBack();
		}

		void GoBack() {
			onBack?.Invoke();
		}

		// -------------------------------------------------------------------------


		IEnumerator Start() {
#if UNITY_EDITOR
			if (Battle == null) {
				Battle = DebugBattle;
			}
#endif

			//
			if (Spirit != null)
				Spirit.SetActive(false);
			PlayerCreatureContainer.gameObject.SetActive(false);
			EnemyCreatureContainer.gameObject.SetActive(false);

			//
			HideActionsList();
			HideItemsList();
			HideMovesList();
			HideCreaturesList();
			HideFleeConfirmationDialog();
			HideTargetsList();
			HideExitCover();

			//
			yield return Dialogue.Scene.Load();

			//
			selectedCreatureIndex = 0;

			//
			ConfigureActionList();
			ConfigureCombatant(
				PlayerCombatant,
				Engine.Profile.GetPartyCreature(selectedCreatureIndex)
			);
			ConfigureCombatant(EnemyCombatant, Battle.Creature);
			PlayerCombatant.HideBars();
			EnemyCombatant.HideBars();
#if UNITY_EDITOR
			yield return Wait.For(1f);
#endif
			yield return ShowAndThenPositionOpponent();
			yield return Wait.For(0.5f);
			ShowActionsList();

			//
			if (Cancel != null)
				Cancel.performed -= OnGoBack;
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		// -------------------------------------------------------------------------

		IEnumerator ShowAndThenPositionOpponent() {
			Vector3 enemyStart = Vector3.zero;
			Vector3 enemyEnd = EnemyCreatureContainer.anchoredPosition;

			EnemyCreatureCanvasGroup.alpha = 0;
			EnemyCreatureContainer.anchoredPosition = enemyStart;
			EnemyCreatureContainer.gameObject.SetActive(true);

			//
			yield return Do.For(0.25f, ratio => EnemyCreatureCanvasGroup.alpha = ratio);
			yield return Wait.For(1.25f);
			yield return Do.For(
				0.5f,
				ratio => EnemyCreatureContainer.anchoredPosition = Vector3.Lerp(enemyStart, enemyEnd, ratio),
				Easing.EaseOutSine01
			);
			yield return Wait.For(0.25f);

			//
			Vector3 playerEnd = PlayerCreatureContainer.anchoredPosition;
			Vector3 playerStart = playerEnd;
			playerStart.y -= 10;

			PlayerCreatureCanvasGroup.alpha = 0;
			PlayerCreatureContainer.anchoredPosition = playerStart;
			PlayerCreatureContainer.gameObject.SetActive(true);

			// 
			yield return Do.For(
				0.33f,
				ratio => {
					PlayerCreatureCanvasGroup.alpha = ratio;
					PlayerCreatureContainer.anchoredPosition = Vector3.Lerp(playerStart, playerEnd, ratio);
				},
				Easing.EaseOutSine01
			);
			yield return Wait.For(0.25f);

			// 
			EnemyCombatant.ShowBars();
			PlayerCombatant.ShowBars();

			yield return Do.For(2f, ratio => {
				PlayerCombatant.FancyFillUp(ratio, Engine.Profile.Magic, Engine.Profile.MagicTotal);
				EnemyCombatant.FancyFillUp(ratio);
			});
		}

		IEnumerator ExitBattle(BattleResult result) {
			showExitCover();
			yield return Do.For(0.25f, ratio => ExitCoverCanvasGroup.alpha = ratio);
			yield return Wait.For(1f);
			yield return Dialogue.Scene.Display("Lethia flees from the battlefield.");

			//
			Battle.OnDone?.Invoke(result);
		}

		// -------------------------------------------------------------------------

		void ConfigureCombatant(CombatantOnScreen combatantOnScreen, Game.Creature creature) {
			combatantOnScreen.Configure(creature);
		}

		void ConfigureActionList() {
			ActionButtons[1].interactable = false;
			ActionButtons[1].GetComponentInChildren<TextMeshProUGUI>()
				.color = !false
					? BlackFaded
					: Color.black;

			bool canSwap = Engine.Profile.Party.Count > 1;
			ActionButtons[3].interactable = canSwap;
			ActionButtons[3].GetComponentInChildren<TextMeshProUGUI>()
				.color = canSwap
					? Color.black
					: BlackFaded;

			ActionButtons[4].interactable = !Battle.CantFlee;
			ActionButtons[4].GetComponentInChildren<TextMeshProUGUI>()
				.color = Battle.CantFlee
					? BlackFaded
					: Color.black;
		}

		// -------------------------------------------------------------------------

		public void OnActionSelect(int action) {
			switch (action) {
				case 0:
					selectedActionIndex = 0;
					SelectMove();
					break;

				case 1:
					Debug.Log("Special");
					break;

				case 2:
					selectedActionIndex = 2;
					SelectItem();
					break;

				case 3:
					selectedActionIndex = 3;
					SwapCreature();
					break;

				case 4:
					selectedActionIndex = 4;
					FleeBattle();
					break;
			}
		}

		// -------------------------------------------------------------------------

		public void SelectMove() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Do.ForEach(
				playerCreature.Skills,
				(skillId, index) => {
					Game.SkillEntry skillEntry = Engine.Profile.Skills
						.Find(entry => entry.SkillId == skillId);
					if (skillEntry == null) {
						return;
					}

					//
					GameObject skillButtonGO = Instantiate(MoveButtonsTemplate, MoveButtonsParent);
					skillButtonGO.SetActive(true);

					TextMeshProUGUI label = skillButtonGO.GetComponentInChildren<TextMeshProUGUI>();
					label.text = skillEntry.Skill.Name;
					label.color = skillEntry.Skill.Cost <= Engine.Profile.Magic
						? Color.black
						: BlackFaded;

					skillButtonGO.GetComponent<InformationButton>()
						.Configure(() => {
							int experience = skillEntry.Experience;
							int toLevel = skillEntry.Skill.ExperienceToLearn;

							float rawLevel = Mathf.Clamp(3f * ((float) experience / (float) (toLevel * 3f)), 0, 3);
							int level = Mathf.FloorToInt(rawLevel);
							int nextLevel = level < 3 ? level + 1 : 3;
							float ratio = level < 3 ? (rawLevel - level) : 1;

							// 
							MoveMagicCost.text = $"{skillEntry.Skill.Cost}";
							MoveDescription.text = skillEntry.Skill.Description;
							MoveGradeProgress.localScale = new Vector3(Mathf.Clamp(ratio, 0, 1), 1, 1);
							MoveGrade.text = string.Join(
								"",
								Do.Times(
									3,
									i => experience >= toLevel * (i + 1) ? "★" : "☆"
								)
							);
							MoveTags.text = "(none)";
						});

					//
					Button button = skillButtonGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => OnMoveSelected(skillEntry));

					//
					buttons.Add(button);
				});

			//
			HideActionsList();
			showMovesList();
		}

		void OnMoveSelected(Game.SkillEntry entry) {
			HideMovesList();

			//
			if (entry.Skill.Targets.Count < 2) {
				BattleAction action = new() {
					Type = BattleActionType.Move,
					Target = entry.Skill.Targets[0],
					SkillEntry = entry
				};

				switch (entry.Skill.Targets[0]) {
					case Game.ApplicableTarget.Player:
						break;
					case Game.ApplicableTarget.Creature:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						break;

					case Game.ApplicableTarget.Enemy:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Battle.Creature;
						break;
				}

				ExecuteAction(action);
			} else {
			}
		}

		void OnMoveListCancel() {
			HideMovesList();
			ShowActionsList();
		}

		// -------------------------------------------------------------------------

		void SelectItem() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			Do.ForEach(
				Engine.Profile.Inventory.All
					.Where(
						entry =>
							entry.Item.Type == Game.ItemType.Consumable &&
							entry.Amount > 0
					)
					.OrderBy(entry => entry.Item.Name)
					.ToList(),
				(entry, index) => {
					GameObject itemGO = Instantiate(ItemButtonsTemplate, ItemButtonsParent);
					itemGO.SetActive(true);

					itemGO
						.GetComponent<ItemButton>()
						.Configure(entry);

					itemGO
						.GetComponent<InformationButton>()
						.Configure(() => {
							ItemListScrollView.UpdateVisibleButtonRange(buttons, index);
						});

					Button button = itemGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => OnItemSelected(entry));

					//
					buttons.Add(button);
				});

			//
			HideActionsList();
			ShowItemsList();
		}

		void OnItemSelected(Game.InventoryEntry entry) {
			HideItemsList();

			//
			if (entry.Item.Targets.Count < 2) {
				BattleAction action = new() {
					Type = BattleActionType.Item,
					Target = entry.Item.Targets[0],
					Item = entry.Item
				};

				switch (entry.Item.Targets[0]) {
					case Game.ApplicableTarget.Player:
						break;
					case Game.ApplicableTarget.Creature:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						break;

					case Game.ApplicableTarget.Enemy:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Battle.Creature;
						break;
				}

				ExecuteAction(action);
			} else {
			}
		}

		void OnItemListCanceled() {
			HideItemsList();
			ShowActionsList();
		}

		// -------------------------------------------------------------------------

		void SwapCreature() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			Do.Times(Engine.Profile.Party.Count, index => {
				Game.Creature creature = Engine.Profile.GetPartyCreature(index);
				if (creature == null) {
					return;
				}

				//
				GameObject creatureGO = Instantiate(CreatureButtonsTemplate, CreatureButtonsParent);
				creatureGO.SetActive(true);

				CreatureButton creatureButton = creatureGO
					.GetComponent<CreatureButton>();
				creatureButton.Configure(creature);

				creatureGO
					.GetComponent<InformationButton>()
					.Configure(creatureButton.Display);

				//
				Button button = creatureButton.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnCreatureSelected(index));

				//
				button.interactable = creature.Health > 0;

				//
				buttons.Add(button);
			});

			//
			HideActionsList();
			ShowCreaturesList();
		}

		public void OnCreatureSelected(int index) {
			dexterityPenalty = 0.5f;

			//
			HideCreaturesList();
			StartCoroutine(SwappingCreature(index));
		}

		void OnCreatureListCancel() {
			HideCreaturesList();
			ShowActionsList();
		}

		IEnumerator SwappingCreature(int newIndex) {
			int oldIndex = selectedCreatureIndex;
			selectedCreatureIndex = newIndex;

			//
			string oldName = Engine.Profile.GetPartyCreature(oldIndex).Name;
			string newName = Engine.Profile.GetPartyCreature(selectedCreatureIndex).Name;

			//
			yield return Dialogue.Scene.Display(
				$"Lethia recalls {oldName}!",
				$"She calls forth {newName}!"
			);
			ConfigureCombatant(
				PlayerCombatant,
				Engine.Profile.GetPartyCreature(selectedCreatureIndex)
			);
			yield return Do.For(2f, ratio => {
				PlayerCombatant.FancyFillUp(ratio, Engine.Profile.Magic, Engine.Profile.MagicTotal);
			});
			yield return Wait.For(0.33f);

			//
			OnCreatureListCancel();
		}

		// -------------------------------------------------------------------------

		public void FleeBattle() {
			HideActionsList();
			ShowFleeConfirmationDialog();
		}

		public void OnFleeSelected() {
			OnFleeSelected(0);
		}

		public void OnFleeSelected(int action) {
			HideFleeConfirmationDialog();
			if (action < 1) {
				ShowActionsList();
				return;
			}

			//
			StartCoroutine(ExitBattle(BattleResult.Fled));
		}

		// -------------------------------------------------------------------------


		void ShowActionsList() {
			ActionListContainer.gameObject.SetActive(true);
			Game.Focus.This(ActionButtons[selectedActionIndex]);
			onBack = null;
		}

		void HideActionsList() {
			ActionListContainer.SetActive(false);
			onBack = null;
		}

		void showMovesList() {
			MoveListContainer.SetActive(true);
			Game.Focus.This(buttons[0]);
			onBack = OnMoveListCancel;
		}

		void HideMovesList() {
			MoveListContainer.SetActive(false);
		}

		void ShowItemsList() {
			ItemListContainer.SetActive(true);
			if (buttons.Count > 0)
				Game.Focus.This(buttons[0]);
			onBack = OnItemListCanceled;
		}

		void HideItemsList() {
			ItemListContainer.SetActive(false);
			onBack = null;
		}

		void ShowCreaturesList() {
			CreaturesList.SetActive(true);
			Game.Focus.This(buttons[selectedCreatureIndex]);
			onBack = OnCreatureListCancel;
		}

		void HideCreaturesList() {
			CreaturesList.SetActive(false);
			onBack = null;
		}

		void ShowTargetsList(Action onBack) {
			TargetsListContainer.SetActive(true);
			Game.Focus.This(buttons[0]);
			this.onBack = onBack;
		}

		void HideTargetsList() {
			TargetsListContainer.SetActive(false);
			onBack = null;
		}

		void ShowFleeConfirmationDialog() {
			ConfirmFleeDialog.SetActive(true);
			Game.Focus.This(ConfirmFleeCancelButton);
			onBack = OnFleeSelected;
		}

		void HideFleeConfirmationDialog() {
			ConfirmFleeDialog.SetActive(false);
			onBack = null;
		}

		void showExitCover() {
			ExitCover.SetActive(true);
		}

		void HideExitCover() {
			ExitCover.SetActive(false);
		}

		// -------------------------------------------------------------------------

		void ExecuteAction(BattleAction action) {
			Debug.Log("Execute action!");


			//
			dexterityPenalty = 0;

			//
			ShowActionsList();
		}

		// -------------------------------------------------------------------------

	}
}
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
		public Animator ActorAnimator;
		public Animator ReceiverAnimator;

		public float Dexterity;

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

		[Header("Combatants")]
		[SerializeField] GameObject Combatants;
		[SerializeField] RectTransform PlayerCreatureContainer;
		[SerializeField] CanvasGroup PlayerCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen PlayerCombatant;
		[SerializeField] RectTransform EnemyCreatureContainer;
		[SerializeField] CanvasGroup EnemyCreatureCanvasGroup;
		[SerializeField] CombatantOnScreen EnemyCombatant;
		[SerializeField] Animator PlayerAnimator;
		[SerializeField] Animator EnemyAnimator;

		[Header("Actions List")]
		[SerializeField] GameObject ActionListContainer;
		[SerializeField] List<Button> ActionButtons;

		[Header("Moves List")]
		[SerializeField] GameObject MoveListContainer;
		[SerializeField] GameObject MoveButtonsTemplate;
		[SerializeField] TextMeshProUGUI MoveMagicCost;
		[SerializeField] TextMeshProUGUI MoveGrade;
		[SerializeField] Transform MoveGradeProgress;
		[SerializeField] TextMeshProUGUI MoveTags;
		[SerializeField] TextMeshProUGUI MoveDescription;

		[Header("Items List")]
		[SerializeField] GameObject ItemListContainer;
		[SerializeField] ScrollView ItemListScrollView;
		[SerializeField] GameObject ItemButtonsTemplate;
		[SerializeField] TextMeshProUGUI ItemDescription;
		[SerializeField] TextMeshProUGUI ItemFlavor;

		[Header("Creatures List")]
		[SerializeField] GameObject CreaturesList;
		[SerializeField] GameObject CreatureButtonsTemplate;

		[Header("Confirm Flee Dialog")]
		[SerializeField] GameObject ConfirmFleeDialog;
		[SerializeField] Button ConfirmFleeCancelButton;

		[Header("Targets List")]
		[SerializeField] GameObject TargetsListContainer;
		[SerializeField] GameObject TargetButtonTemplate;

		[Header("Exit Cover")]
		[SerializeField] GameObject ExitCover;
		[SerializeField] CanvasGroup ExitCoverCanvasGroup;

		// -------------------------------------------------------------------------

		readonly Color BlackFaded = new(0, 0, 0, 0.5f);

		readonly List<Button> buttons = new();

		InputAction Cancel;
		Action onBack = () => Debug.Log("Nothing set");

		int selectedActionIndex;
		int selectedCreatureIndex;
		float dexterityPenalty = 1;
		bool creatureSwapIsMandatory;

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
			HideCombatants();
			HideActionsList();
			HideItemsList();
			HideMovesList();
			HideCreaturesList();
			HideFleeConfirmationDialog();
			HideTargetsList();
			HideExitCover();
			yield return Dialogue.Scene.Load();
			SetSelectedCreatureIndexToFirstLiving();
			ConfigureActionList();
			ShowCombatants();
			ConfigureCombatant(
				PlayerCombatant,
				Engine.Profile.GetPartyCreature(selectedCreatureIndex)
			);
			ConfigureCombatant(EnemyCombatant, Battle.Creature);
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
			Game.Creature creature =
				Engine.Profile.GetPartyCreature(selectedCreatureIndex);

			Vector3 enemyStart = Vector3.zero;
			Vector3 enemyEnd = EnemyCreatureContainer.anchoredPosition;

			EnemyCreatureCanvasGroup.alpha = 0;
			EnemyCreatureContainer.anchoredPosition = enemyStart;
			EnemyCreatureContainer.gameObject.SetActive(true);

			//
			PlayerCombatant.HideBars();
			EnemyCombatant.HideBars();
			yield return Do.For(0.25f, ratio => EnemyCreatureCanvasGroup.alpha = ratio);
			yield return Wait.For(1.25f);
			yield return Dialogue.Scene.Display("An enraged spirit emerged!");
			yield return Wait.For(0.25f);
			yield return Do.For(
				0.5f,
				ratio => EnemyCreatureContainer.anchoredPosition = Vector3.Lerp(enemyStart, enemyEnd, ratio),
				Easing.EaseOutSine01
			);
			yield return Wait.For(0.25f);
			yield return Dialogue.Scene.Display(new string[1] { $"Come forth, {creature.Name}!" }, "Lethia");

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

			yield return Do.For(1.25f, ratio => {
				PlayerCombatant.FancyHealthFillUp(ratio);
				PlayerCombatant.FancyMagicFillUp(ratio, Engine.Profile.Magic, Engine.Profile.MagicTotal);

				EnemyCombatant.FancyHealthFillUp(ratio);
			});
		}

		IEnumerator ExitBattle(BattleResult result) {
			ShowExitCover();
			yield return Do.For(0.25f, ratio => ExitCoverCanvasGroup.alpha = ratio);
			yield return Wait.For(1f);
			yield return Dialogue.Scene.Display(new string[1] { "I'll retreat for now." }, "Lethia");

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
					GameObject skillButtonGO = Instantiate(
						MoveButtonsTemplate,
						MoveButtonsTemplate.transform.parent
					);
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
							MoveTags.text = "⌦ (none)";
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
			ShowMovesList();
		}

		void OnMoveSelected(Game.SkillEntry entry) {
			HideMovesList();

			//
			BattleAction action = new() {
				Type = BattleActionType.Move,
				SkillEntry = entry
			};

			//
			if (entry.Skill.Targets.Count < 2) {
				action.Target = entry.Skill.Targets[0];

				switch (entry.Skill.Targets[0]) {
					case Game.ApplicableTarget.Player:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = action.Actor;
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = PlayerAnimator;
						break;

					case Game.ApplicableTarget.Creature:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = PlayerAnimator;
						break;

					case Game.ApplicableTarget.Enemy:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Battle.Creature;
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = EnemyAnimator;
						break;
				}

				//
				ExecuteAction(action);
			} else {
				SelectTarget(action, SelectMove);
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
					GameObject itemGO = Instantiate(
						ItemButtonsTemplate,
						ItemButtonsTemplate.transform.parent
					);
					itemGO.SetActive(true);

					itemGO
						.GetComponent<ItemButton>()
						.Configure(entry);

					itemGO
						.GetComponent<InformationButton>()
						.Configure(() => {
							ItemDescription.text = entry.Item.Description;
							ItemFlavor.text = entry.Item.FlavorText;

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
			BattleAction action = new() {
				Type = BattleActionType.Item,
				Item = entry.Item
			};

			//
			if (entry.Item.Targets.Count < 2) {
				action.Target = entry.Item.Targets[0];

				switch (entry.Item.Targets[0]) {
					case Game.ApplicableTarget.Player:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = action.Actor;
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = PlayerAnimator;
						break;

					case Game.ApplicableTarget.Creature:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = PlayerAnimator;
						break;

					case Game.ApplicableTarget.Enemy:
						action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
						action.Receiver = Battle.Creature;
						action.ActorAnimator = PlayerAnimator;
						action.ReceiverAnimator = EnemyAnimator;
						break;
				}

				//
				ExecuteAction(action);
			} else {
				HideItemsList();
				SelectTarget(action, SelectItem);
			}
		}

		void OnItemListCanceled() {
			HideItemsList();
			ShowActionsList();
		}

		// -------------------------------------------------------------------------

		void SelectTarget(BattleAction action, Action newOnBack) {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			(
				action.Type == BattleActionType.Item
					? action.Item.Targets
					: action.SkillEntry.Skill.Targets
			).ForEach(applicableTarget => {
				GameObject targetButtonGO = Instantiate(
					TargetButtonTemplate,
					TargetButtonTemplate.transform.parent
				);
				targetButtonGO.SetActive(true);

				TextMeshProUGUI label = targetButtonGO
					.GetComponentInChildren<TextMeshProUGUI>();

				switch (applicableTarget) {
					case Game.ApplicableTarget.Player:
						label.text = "Lethia";
						break;

					case Game.ApplicableTarget.Creature:
						label.text = Engine.Profile.GetPartyCreature(selectedCreatureIndex).Name;
						break;

					case Game.ApplicableTarget.Enemy:
						label.text = Battle.Creature.Name;
						break;
				}

				Button button = targetButtonGO
					.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnSelectTarget(action, applicableTarget));

				//
				buttons.Add(button);
			});

			//
			ShowTargetsList(newOnBack);
		}

		void OnSelectTarget(BattleAction action, Game.ApplicableTarget applicableTarget) {
			action.Target = applicableTarget;

			//
			switch (action.Target) {
				case Game.ApplicableTarget.Player:
					action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
					action.Receiver = action.Actor;
					action.ActorAnimator = PlayerAnimator;
					action.ReceiverAnimator = PlayerAnimator;
					break;

				case Game.ApplicableTarget.Creature:
					action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
					action.Receiver = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
					action.ActorAnimator = PlayerAnimator;
					action.ReceiverAnimator = PlayerAnimator;
					break;

				case Game.ApplicableTarget.Enemy:
					action.Actor = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
					action.Receiver = Battle.Creature;
					action.ActorAnimator = PlayerAnimator;
					action.ReceiverAnimator = EnemyAnimator;
					break;
			}

			//
			HideTargetsList();
			ExecuteAction(action);
		}

		// -------------------------------------------------------------------------

		void SwapCreature() {
			buttons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			buttons.Clear();

			//
			Game.Creature currentCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);

			//
			Do.Times(Engine.Profile.Party.Count, index => {
				Game.Creature creature = Engine.Profile.GetPartyCreature(index);
				if (creature == null) {
					return;
				}

				//
				GameObject creatureGO = Instantiate(
					CreatureButtonsTemplate,
					CreatureButtonsTemplate.transform.parent
				);
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
				buttons.Add(button);
			});

			//
			HideActionsList();
			ShowCreaturesList();
		}

		public void OnCreatureSelected(int index) {
			if (index == selectedCreatureIndex) {
				return;
			}

			//
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
			Vector3 start = PlayerCreatureContainer.anchoredPosition;
			Vector3 end = start;
			end.y -= 10;

			if (!creatureSwapIsMandatory) {
				yield return Do.For(
					0.5f,
					ratio => {
						PlayerCreatureContainer.anchoredPosition = Vector3.Lerp(start, end, ratio);
						PlayerCreatureCanvasGroup.alpha = 1 - ratio * 2;
					},
					Easing.EaseOutSine01
				);
				yield return Dialogue.Scene.Display(
					$"Lethia recalls {oldName}!",
					$"She calls forth {newName}!"
				);
			} else {
				yield return Dialogue.Scene.Display(
					new string[1] { $"Come forth, {newName}!" },
					"Lethia"
				);
			}

			ConfigureCombatant(
				PlayerCombatant,
				Engine.Profile.GetPartyCreature(selectedCreatureIndex)
			);
			PlayerCombatant.HideBars();
			yield return Do.For(
				0.5f,
				ratio => {
					PlayerCreatureContainer.anchoredPosition = Vector3.Lerp(end, start, ratio);
					PlayerCreatureCanvasGroup.alpha = ratio * 2;
				},
				Easing.EaseOutSine01
			);
			PlayerCombatant.ShowBars();
			yield return Do.For(2f, ratio => {
				PlayerCombatant.FancyHealthFillUp(ratio);
				PlayerCombatant.FancyMagicFillUp(ratio, Engine.Profile.Magic, Engine.Profile.MagicTotal);
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

		void ShowCombatants() {
			Combatants.SetActive(true);
		}

		void HideCombatants() {
			Combatants.SetActive(false);
			PlayerCreatureContainer.gameObject.SetActive(false);
			EnemyCreatureContainer.gameObject.SetActive(false);
			if (Spirit != null)
				Spirit.SetActive(false);
		}


		void ShowActionsList() {
			ActionListContainer.gameObject.SetActive(true);
			Game.Focus.This(ActionButtons[selectedActionIndex]);
			onBack = null;

			creatureSwapIsMandatory = false;
		}

		void HideActionsList() {
			ActionListContainer.SetActive(false);
			onBack = null;
		}

		void ShowMovesList() {
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
			this.onBack = () => {
				HideTargetsList();
				onBack();
			};
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

		void ShowExitCover() {
			ExitCover.SetActive(true);
		}

		void HideExitCover() {
			ExitCover.SetActive(false);
		}

		// -------------------------------------------------------------------------

		void ExecuteAction(BattleAction action) {
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			action.Dexterity = playerCreature.Dexterity * dexterityPenalty;

			//
			StartCoroutine(PerformActions(action, GenerateEnemyAction()));
		}

		BattleAction GenerateEnemyAction() {
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Game.Creature enemyCreature = Battle.Creature;

			System.Random random = new System.Random();
			List<Game.SkillId> skillIds = new List<Game.SkillId>(enemyCreature.Skills)
				.OrderBy(x => random.Next())
				.ToList();

			//
			return new() {
				Type = BattleActionType.Move,
				Target = Game.ApplicableTarget.Creature,
				SkillEntry = new() {
					SkillId = skillIds[0]
				},

				Dexterity = enemyCreature.Dexterity,

				Actor = enemyCreature,
				ActorAnimator = EnemyAnimator,
				Receiver = playerCreature,
				ReceiverAnimator = PlayerAnimator
			};
		}

		IEnumerator PerformActions(BattleAction first, BattleAction second) {
			List<BattleAction> actions = new List<BattleAction>() {
				first,
				second
			}
			.OrderByDescending(action => action.Dexterity)
			.ToList();

			//
			yield return PerformAction(actions[0]);
			if (SomeoneDied()) {
				yield return HandleDeath();
				yield break;
			}

			yield return PerformAction(actions[1]);
			if (SomeoneDied()) {
				yield return HandleDeath();
				yield break;
			}

			//
			yield return Wait.For(0.33f);

			//
			dexterityPenalty = 1;
			ShowActionsList();
		}

		IEnumerator PerformAction(BattleAction action) {
			switch (action.Type) {
				case BattleActionType.Item:
					yield return UseItem(action.Item, action.Actor, action.Receiver, action.ActorAnimator, action.ReceiverAnimator);
					break;

				case BattleActionType.Move:
					yield return PerformMove(action.SkillEntry, action.Actor, action.Receiver, action.ActorAnimator, action.ReceiverAnimator);
					break;

				default:
					yield break;
			}
		}

		// -------------------------------------------------------------------------

		bool SomeoneDied() {
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Game.Creature enemyCreature = Battle.Creature;

			//
			return
				playerCreature.Health < 1 ||
				enemyCreature.Health < 1;
		}

		IEnumerator HandleDeath() {
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Game.Creature enemyCreature = Battle.Creature;

			if (enemyCreature.Health < 1) {
				Vector3 start = EnemyCreatureContainer.anchoredPosition;
				Vector3 end = start;
				end.y -= 10;

				yield return Do.For(
					0.5f,
					ratio => {
						EnemyCreatureContainer.anchoredPosition = Vector3.Lerp(start, end, ratio);
						EnemyCreatureCanvasGroup.alpha = 1 - ratio * 2;
					},
					Easing.EaseOutSine01
				);
				yield return Dialogue.Scene.Display("Enraged spirit collapsed!");
				yield return BattleEnd();
			} else if (playerCreature.Health < 1) {
				Vector3 start = PlayerCreatureContainer.anchoredPosition;
				Vector3 end = start;
				end.y -= 10;

				yield return Do.For(
					0.5f,
					ratio => {
						PlayerCreatureContainer.anchoredPosition = Vector3.Lerp(start, end, ratio);
						PlayerCreatureCanvasGroup.alpha = 1 - ratio * 2;
					},
					Easing.EaseOutSine01
				);
				yield return Dialogue.Scene.Display(
					$"{playerCreature.Name} collapsed!"
				);

				// 
				if (Engine.Profile.CreaturesAvailableToFight > 0) {
					yield return Dialogue.Scene.Display(
						new string[1] {
							"Who should I choose next?"
						},
						"Lethia"
					);

					//
					SwapCreature();
					creatureSwapIsMandatory = true;
					onBack = null;

					for (int i = 0; i < buttons.Count; i++) {
						Button button = buttons[i];
						Game.Creature creature = Engine.Profile.GetPartyCreature(i);
						if (creature.Health > 0) {
							Game.Focus.This(button);
							break;
						}
					}
				} else {
					yield return Dialogue.Scene.Display(
						new string[2] {
							"All my creatures have been defeated.",
							"I must return to the village."
						},
						"Lethia"
					);

					//
					Loader.Scene.Load(new Game.NextScene {
						Name = Village.Scene.Name,
						Destination = Village.Scene.Location_Tree
					});
					yield break;
				}
			} else {
				Debug.Assert(true, "No one actually died!");
				throw new System.Exception("No one actually died!");
			}
		}

		IEnumerator BattleEnd() {
			yield break;
		}

		// -------------------------------------------------------------------------

		IEnumerator UseItem(Item item, Game.Creature actor, Game.Creature receiver, Animator actorAnimator, Animator receiverAnimator) {
			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Game.Creature enemyCreature = Battle.Creature;

			float creatureHPA = playerCreature.Health;
			float creatureHPTA = playerCreature.HealthTotal;

			float enemyHPA = enemyCreature.Health;
			float enemyHPTA = enemyCreature.HealthTotal;

			float mpA = Engine.Profile.Magic;

			//
			yield return Dialogue.Scene.Display($"Lethia uses {item.Name}!");

			//
			ApplyEffects(item.Effects, actor, receiver);
			Engine.Profile.Inventory.AdjustItem(item, -1);

			// show skill fx(s)
			yield return Wait.For(0.33f);

			List<Animator> active = new();
			List<bool> done = new();

			foreach (Game.SkillFX fx in item.FX) {
				Animator animator = null;
				switch (fx.Target) {
					case Game.EffectTarget.Actor:
						animator = actorAnimator;
						break;

					case Game.EffectTarget.Recipient:
						animator = receiverAnimator;
						break;
				}

				if (active.Contains(animator)) {
					Debug.LogError($"Already have this {animator.name} animator in action!");
					continue;
				}

				active.Add(animator);
				done.Add(false);

				StartCoroutine(AnimateFX(fx.AnimationName, fx, animator, done, done.Count - 1));
			}

			yield return Wait.Until(() => done.All(isDone => isDone));
			yield return Wait.For(0.33f);

			// update health and statuses 
			float creatureHPB = playerCreature.Health;
			float creatureHPTB = playerCreature.HealthTotal;

			float enemyHPB = enemyCreature.Health;
			float enemyHPTB = enemyCreature.HealthTotal;

			float mpB = Engine.Profile.Magic;

			yield return Do.For(0.5f, ratio => {
				int creatureHP = Mathf.RoundToInt(Mathf.Lerp(creatureHPA, creatureHPB, ratio));
				int creatureHPT = Mathf.RoundToInt(Mathf.Lerp(creatureHPTA, creatureHPTB, ratio));
				int creatureMP = Mathf.RoundToInt(Mathf.Lerp(mpA, mpB, ratio));
				int enemyHP = Mathf.RoundToInt(Mathf.Lerp(enemyHPA, enemyHPB, ratio));
				int enemyHPT = Mathf.RoundToInt(Mathf.Lerp(enemyHPTA, enemyHPTB, ratio));

				PlayerCombatant.UpdateHealth(creatureHP, creatureHPT);
				PlayerCombatant.UpdateMagic(creatureMP, Engine.Profile.MagicTotal);

				EnemyCombatant.UpdateHealth(enemyHP, enemyHPT);
			}, Easing.EaseInOutSine01);
		}

		void ApplyEffects(List<Game.Effect> effects, Game.Creature actor, Game.Creature receiver) {
			effects.ForEach(effect => {
				Game.Creature effected = null;
				switch (effect.Target) {
					case Game.EffectTarget.Actor:
						effected = actor;
						break;

					case Game.EffectTarget.Recipient:
						effected = receiver;
						break;
				}

				switch (effect.Type) {
					case Game.EffectType.Health:
						effected.AdjustHealth(
							Mathf.RoundToInt(
								effect.Strength * UnityEngine.Random.Range(1 - effect.Variance, 1 + effect.Variance)
							)
						);
						break;

					case Game.EffectType.Status:
						effected.AddStatus(effect.Status, effect.Duration, effect.Strength);
						break;

					case Game.EffectType.Magic:
						Engine.Profile.AdjustMagic(effect.Strength);
						break;
				}
			});
		}

		IEnumerator PerformMove(Game.SkillEntry skillEntry, Game.Creature actor, Game.Creature receiver, Animator actorAnimator, Animator receiverAnimator) {
			Skill skill = skillEntry.Skill;

			Game.Creature playerCreature = Engine.Profile.GetPartyCreature(selectedCreatureIndex);
			Game.Creature enemyCreature = Battle.Creature;

			float creatureHPA = playerCreature.Health;
			float creatureHPTA = playerCreature.HealthTotal;

			float enemyHPA = enemyCreature.Health;
			float enemyHPTA = enemyCreature.HealthTotal;

			float mpA = Engine.Profile.Magic;

			//
			yield return Dialogue.Scene.Display($"{actor.Name} performs {skill.Name}!");

			//
			ApplyEffects(skill.Effect, actor, receiver);

			if (actor == playerCreature) {
				Engine.Profile.Magic = Mathf.Clamp(Engine.Profile.Magic - skill.Cost, 0, Engine.Profile.MagicTotal);
			}

			// show skill fx(s)
			yield return Wait.For(0.33f);

			List<Animator> active = new();
			List<bool> done = new();

			foreach (Game.SkillFX fx in skill.FX) {
				Animator animator = null;
				switch (fx.Target) {
					case Game.EffectTarget.Actor:
						animator = actorAnimator;
						break;

					case Game.EffectTarget.Recipient:
						animator = receiverAnimator;
						break;
				}

				if (active.Contains(animator)) {
					Debug.LogError($"Already have this {animator.name} animator in action!");
					continue;
				}

				active.Add(animator);
				done.Add(false);

				StartCoroutine(AnimateFX(fx.AnimationName, fx, animator, done, done.Count - 1));
			}

			yield return Wait.Until(() => done.All(isDone => isDone));
			yield return Wait.For(0.33f);

			// update health and statuses 
			float creatureHPB = playerCreature.Health;
			float creatureHPTB = playerCreature.HealthTotal;

			float enemyHPB = enemyCreature.Health;
			float enemyHPTB = enemyCreature.HealthTotal;

			float mpB = Engine.Profile.Magic;

			yield return Do.For(0.5f, ratio => {
				int creatureHP = Mathf.RoundToInt(Mathf.Lerp(creatureHPA, creatureHPB, ratio));
				int creatureHPT = Mathf.RoundToInt(Mathf.Lerp(creatureHPTA, creatureHPTB, ratio));
				int creatureMP = Mathf.RoundToInt(Mathf.Lerp(mpA, mpB, ratio));
				int enemyHP = Mathf.RoundToInt(Mathf.Lerp(enemyHPA, enemyHPB, ratio));
				int enemyHPT = Mathf.RoundToInt(Mathf.Lerp(enemyHPTA, enemyHPTB, ratio));

				PlayerCombatant.UpdateHealth(creatureHP, creatureHPT);
				PlayerCombatant.UpdateMagic(creatureMP, Engine.Profile.MagicTotal);

				EnemyCombatant.UpdateHealth(enemyHP, enemyHPT);
			}, Easing.EaseInOutSine01);
		}

		IEnumerator AnimateFX(string name, Game.SkillFX fx, Animator animator, List<bool> done, int doneIndex) {
			List<AnimationClip> clips = new(animator.runtimeAnimatorController.animationClips);
			if (!clips.Any(clip => clip.name == name)) {
				Debug.LogError($"No animation clip named: {name}");
				done[doneIndex] = true;
				yield break;
			}

			//
			var clip = clips.Find(clip => clip.name == name);
			if (fx.Delay > 0) {
				yield return Wait.For(fx.Delay);
			}

			//
			animator.Play(name);

			//
			yield return Wait.Until(() => animator.GetCurrentAnimatorStateInfo(0).IsName(name));
			yield return Wait.Until(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(name));

			//
			done[doneIndex] = true;
		}

		// -------------------------------------------------------------------------

		void SetSelectedCreatureIndexToFirstLiving() {
			string livingPartyId = Do.Select(
				Engine.Profile.Party,
				(_, index) => Engine.Profile.GetPartyCreature(index)
			)
				.Where(creature => creature.Health > 0)
				.ToList()
				[0]?.Id;
			selectedCreatureIndex = Engine.Profile.Party.IndexOf(livingPartyId);
		}

		// -------------------------------------------------------------------------

	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Game;

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

	[Serializable]
	public class Battle {
		public Game.Creature Creature;
		public Game.SpiritId SpiritId;
		public Action<BattleResult> OnDone;
		public bool CantFlee;
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
		[SerializeField] GameObject MoveListContainer;
		[SerializeField] GameObject TargetsListContainer;

		[Header("Actions List")]
		[SerializeField] GameObject ActionListContainer;
		[SerializeField] List<Button> ActionButtons;

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


		// -------------------------------------------------------------------------

		readonly Color BlackFaded = new(0, 0, 0, 0.5f);

		readonly List<Button> creatureButtons = new();
		readonly List<Button> itemButtons = new();

		InputAction Cancel;
		Action onBack = () => Debug.Log("Nothing set");

		int selectedActionIndex;

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
			yield return Wait.For(1f);
#endif

			//
			if (Spirit != null)
				Spirit.SetActive(false);
			PlayerCreatureContainer.gameObject.SetActive(false);
			EnemyCreatureContainer.gameObject.SetActive(false);

			HideActionsList();
			HideItemsList();
			MoveListContainer.SetActive(false);
			HideCreaturesList();
			ConfirmFleeDialog.SetActive(false);
			TargetsListContainer.SetActive(false);
			ExitCover.SetActive(false);

			//
			yield return Dialogue.Scene.Load();

			//
			ConfigureActionList();
			ConfigureCombatant(PlayerCombatant, Engine.Profile.GetPartyCreature(0));
			ConfigureCombatant(EnemyCombatant, Battle.Creature);
			PlayerCombatant.HideBars();
			EnemyCombatant.HideBars();
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
			ExitCover.SetActive(true);

			//
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
					Debug.Log("Move List");
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


		// -------------------------------------------------------------------------

		void SelectItem() {
			itemButtons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			itemButtons.Clear();

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
							ItemListScrollView.UpdateVisibleButtonRange(itemButtons, index);
						});

					Button button = itemGO.GetComponent<Button>();
					button.onClick.RemoveAllListeners();
					button.onClick.AddListener(() => OnItemSelected(entry));

					//
					itemButtons.Add(button);
				});

			//
			HideActionsList();
			ShowItemsList();
		}

		void OnItemSelected(InventoryEntry entry) {
			Debug.Log($"Item selected: {entry.Item.Name}");
		}

		void OnItemListCanceled() {
			HideItemsList();
			ShowActionsList();
		}

		// -------------------------------------------------------------------------

		void SwapCreature() {
			creatureButtons.ForEach(button => {
				button.gameObject.SetActive(false);
				Destroy(button.gameObject);
			});
			creatureButtons.Clear();

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
				button.onClick.AddListener(() => OnCreatureSelected(creature));

				//
				creatureButtons.Add(button);
			});

			//
			HideActionsList();
			ShowCreaturesList();
		}

		public void OnCreatureSelected(Game.Creature creature) {
			Debug.Log("swap!");
			Debug.Log("show dialogue about swap");
			Debug.Log("50% dexterity debuff on swap");

			//
			OnCreatureListCancel();
		}

		void OnCreatureListCancel() {
			HideCreaturesList();
			ShowActionsList();
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

		void ShowItemsList() {
			ItemListContainer.SetActive(true);
			if (itemButtons.Count > 0)
				Game.Focus.This(itemButtons[0]);
			onBack = OnItemListCanceled;
		}

		void HideItemsList() {
			ItemListContainer.SetActive(false);
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

		void ShowCreaturesList() {
			CreaturesList.SetActive(true);
			Game.Focus.This(creatureButtons[0]);
			onBack = OnCreatureListCancel;
		}

		void HideCreaturesList() {
			CreaturesList.SetActive(false);
			onBack = null;
		}

		// -------------------------------------------------------------------------

	}
}
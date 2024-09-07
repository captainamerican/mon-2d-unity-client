using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Combat {

	// ---------------------------------------------------------------------------

	[Serializable]
	public class Battle {
		public Game.Creature Creature;
		public Game.SpiritId SpiritId;
		public Action OnDone;
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
		[SerializeField] GameObject ActionListContainer;
		[SerializeField] List<Button> ActionButtons;
		[SerializeField] Button ActionListSpecialButton;
		[SerializeField] Button ActionListFleeButton;
		[SerializeField] GameObject ItemListContainer;
		[SerializeField] GameObject MoveListContainer;
		[SerializeField] GameObject ConfirmFleeDialog;
		[SerializeField] GameObject TargetsListContainer;

		[Header("Creatures List")]
		[SerializeField] GameObject CreaturesList;
		[SerializeField] Transform CreaturesButtonParent;
		[SerializeField] GameObject CreatureButtonTemplate;

		// -------------------------------------------------------------------------

		List<GameObject> creatureButtons = new();

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
#endif

			//
			if (Spirit != null)
				Spirit.SetActive(false);
			PlayerCreatureContainer.gameObject.SetActive(false);
			EnemyCreatureContainer.gameObject.SetActive(false);
			ActionListContainer.SetActive(false);
			ItemListContainer.SetActive(false);
			MoveListContainer.SetActive(false);
			CreaturesList.SetActive(false);
			ConfirmFleeDialog.SetActive(false);
			TargetsListContainer.SetActive(false);

			//
			yield return Dialogue.Scene.Load();

#if UNITY_EDITOR
			yield return Wait.For(1f);
#endif

			//
			ConfigureActionList();
			ConfigureCombatant(PlayerCombatant, Engine.Profile.GetPartyCreature(0));
			ConfigureCombatant(EnemyCombatant, Battle.Creature);
			PlayerCombatant.HideBars();
			EnemyCombatant.HideBars();
			yield return ShowAndThenPositionOpponent();
			yield return Wait.For(0.5f);
			ShowActionList();

			//
			if (Cancel != null)
				Cancel.performed -= OnGoBack;
			Cancel = PlayerInput.currentActionMap.FindAction("Cancel");
			Cancel.performed += OnGoBack;
		}

		void ConfigureCombatant(CombatantOnScreen combatantOnScreen, Game.Creature creature) {
			combatantOnScreen.Configure(creature);
		}

		void ConfigureActionList() {
			ActionListSpecialButton.interactable = false;
			ActionListSpecialButton.GetComponentInChildren<TextMeshProUGUI>()
				.color = !false
					? new Color(0, 0, 0, 0.5f)
					: Color.black;

			ActionListFleeButton.interactable = !Battle.CantFlee;
			ActionListFleeButton.GetComponentInChildren<TextMeshProUGUI>()
				.color = Battle.CantFlee
					? new Color(0, 0, 0, 0.5f)
					: Color.black;
		}

		void ShowActionList() {
			ActionListContainer.gameObject.SetActive(true);
			Game.Focus.This(ActionButtons[selectedActionIndex]);

			//
			onBack = OnActionCancel;
		}

		void OnActionCancel() {
		}

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
					Debug.Log("Item List");
					break;

				case 3:
					selectedActionIndex = 3;
					ShowCreaturesList();
					break;

				case 4:
					Debug.Log("Flee");
					break;
			}
		}

		// -------------------------------------------------------------------------

		void ShowCreaturesList() {
			creatureButtons.ForEach(go => {
				go.SetActive(false);
				Destroy(go);
			});
			creatureButtons.Clear();

			//
			Do.Times(Engine.Profile.Party.Count, index => {
				Game.Creature creature = Engine.Profile.GetPartyCreature(index);
				if (creature == null) {
					return;
				}

				//
				GameObject creatureGO = Instantiate(CreatureButtonTemplate, CreaturesButtonParent);
				creatureGO.SetActive(true);

				CreatureButton creatureButton = creatureGO.GetComponent<CreatureButton>();
				creatureButton.Configure(creature);

				InformationButton informationButton = creatureGO.GetComponent<InformationButton>();
				informationButton.Configure(creatureButton.Display);

				//
				Button button = creatureButton.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnCreatureSelected(creature));

				//
				creatureButtons.Add(creatureGO);
			});

			//
			ActionListContainer.SetActive(false);
			CreaturesList.SetActive(true);

			Game.Focus.This(creatureButtons[0].GetComponent<Button>());

			onBack = OnCreatureListCancel;
		}

		void OnCreatureListCancel() {
			CreaturesList.SetActive(false);

			ShowActionList();
		}

		public void OnCreatureSelected(Game.Creature creature) {
			Debug.Log("swap!");
			Debug.Log("show dialogue about swap");
			Debug.Log("50% dexterity debuff on swap");

			//
			OnCreatureListCancel();
		}

		// -------------------------------------------------------------------------

	}
}
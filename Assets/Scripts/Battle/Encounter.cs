using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Battle {
	enum Phase {
		NoBattle,
		BattleStart,
		BattleTurnStart,
		BattlePlayerActionSelection,
		BattleShowMoves,
		BattleShowItems,
		BattleShowCreatures,
		BattleShowConfirmRun,
		BattleAction,
		BattleTurnEnd,
		BattleEnd
	}

	public class Encounter : MonoBehaviour {
		[SerializeField]
		Engine Engine;

		[SerializeField]
		Player Player;

		[SerializeField]
		PlayerInput PlayerInput;

		[SerializeField]
		Image Creature;

		[SerializeField]
		Image Enemy;

		[SerializeField]
		GameObject Cover;

		[SerializeField]
		GameObject CoverMask;

		[SerializeField]
		GameObject Content;

		[SerializeField]
		GameObject ItemsMenu;

		[SerializeField]
		GameObject ActionMenu;

		[SerializeField]
		GameObject MovesMenu;

		[SerializeField]
		GameObject CreaturesMenu;

		[SerializeField]
		Dialogue Dialogue;

		[SerializeField]
		Button FirstAction;

		[SerializeField]
		GameObject CreatureQueue;

		[SerializeField]
		List<Button> MoveButtons;

		[SerializeField]
		TextMeshProUGUI MoveDescription;

		[SerializeField]
		TextMeshProUGUI MoveCost;

		[SerializeField]
		TextMeshProUGUI MoveUses;

		[SerializeField]
		GameObject Statistics;

		[SerializeField]
		TextMeshProUGUI EnemyNameLabel;

		[SerializeField]
		RectTransform EnemyHealthTransform;

		[SerializeField]
		TextMeshProUGUI CreatureNameLabel;

		[SerializeField]
		TextMeshProUGUI CreatureHealthLabel;

		[SerializeField]
		TextMeshProUGUI CreatureMagicLabel;

		[SerializeField]
		RectTransform CreatureHealthTransform;

		[SerializeField]
		RectTransform CreatureMagicTransform;

		static Encounter Self;

		InputAction SubmitAction;
		InputAction CancelAction;

		WorldEnemy.Enemy enemy;
		Creature enemyCreature;
		Creature currentCreature;

		Combatant enemyCombatant;
		List<Combatant> creatureCombatants = new();
		Combatant creatureCombatant;

		Phase phase;
		int currentCreatureIndex;
		int magic;
		int magicTotal;

		static public void Begin(WorldEnemy.Enemy enemy) {
			Self.StartBattle(enemy);
		}

		void Start() {
			Self = this;
			Cover.SetActive(false);
			Content.SetActive(false);

			SubmitAction = PlayerInput.currentActionMap.FindAction("Submit");
			CancelAction = PlayerInput.currentActionMap.FindAction("Cancel");
		}

		void Update() {
		}

		void StartBattle(WorldEnemy.Enemy enemy) {
			Engine.Mode = EngineMode.Battle;

			this.enemy = enemy;

			magic = Engine.Profile.Magic;
			magicTotal = Engine.Profile.MagicTotal;

			enemyCreature = enemy.RollAppearance();
			enemyCombatant = new Combatant { Health = 30, HealthTotal = 30 };

			currentCreatureIndex = 0;
			currentCreature = Engine.Profile.Party[currentCreatureIndex];

			creatureCombatants.Add(new Combatant { Health = 50, HealthTotal = 50 });

			creatureCombatant = creatureCombatants[currentCreatureIndex];

			EnemyNameLabel.text = enemyCreature.Name;
			CreatureNameLabel.text = currentCreature.Name;

			Player.Stop();

			StartCoroutine(StartingBatle());
		}

		IEnumerator StartingBatle() {
			Cover.SetActive(true);
			Cover.transform.position = Player.transform.position + new Vector3(0, 0, 20f);
			CoverMask.transform.localScale = Vector3.one * 2;

			Player.GetComponent<SpriteRenderer>().sortingOrder = 11;
			enemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = 11;

			yield return Wait.For(1f);
			yield return Do.For(1f, ratio => {
				CoverMask.transform.localScale = (1 - ratio) * 2 * Vector3.one;
			});
			yield return Wait.For(1f);

			Content.SetActive(true);
			ItemsMenu.SetActive(false);
			ActionMenu.SetActive(false);
			MovesMenu.SetActive(false);
			CreaturesMenu.SetActive(false);
			CreatureQueue.SetActive(true);
			Statistics.SetActive(false);

			Dialogue.Display("An angry spirit emerges.", $"Lethia summons {currentCreature.Name}!");
			yield return Wait.Until(Dialogue.IsDone);

			//
			CreatureQueue.SetActive(false);
			Statistics.SetActive(true);

			//
			Vector3 enemyHealthA = new(0, 1, 1);
			Vector3 enemyHealthB = new((float) enemyCombatant.Health / (float) enemyCombatant.HealthTotal, 1, 1);

			Vector3 creatureHealthA = new(0, 1, 1);
			Vector3 creatureHealthB = new((float) creatureCombatant.Health / (float) creatureCombatant.HealthTotal, 1, 1);

			Vector3 creatureMagicA = new(0, 1, 1);
			Vector3 creatureMagicB = new((float) magic / (float) magicTotal, 1, 1);

			float creatureHPA = 0;
			float creatureHPB = creatureCombatant.Health;

			float mpA = 0;
			float mpB = magic;

			string hpLabel = CreatureHealthLabel.text;
			string mpLabel = CreatureMagicLabel.text;
			yield return Do.For(1f, ratio => {
				EnemyHealthTransform.localScale = Vector3.Lerp(enemyHealthA, enemyHealthB, ratio);
				CreatureHealthTransform.localScale = Vector3.Lerp(creatureHealthA, creatureHealthB, ratio);
				CreatureMagicTransform.localScale = Vector3.Lerp(creatureMagicA, creatureMagicB, ratio);

				hpLabel = $"{Mathf.FloorToInt(Mathf.Lerp(creatureHPA, creatureHPB, ratio))}";
				mpLabel = $"{Mathf.FloorToInt(Mathf.Lerp(mpA, mpB, ratio))}";

				if (CreatureHealthLabel.text != hpLabel) {
					CreatureHealthLabel.text = hpLabel;
				}

				if (CreatureMagicLabel.text != mpLabel) {
					CreatureMagicLabel.text = mpLabel;
				}
			});

			//
			TurnStart();
		}

		void TurnStart() {
			ShowActions();
		}

		IEnumerator TurnAction(Skill skill, Combatant actor, Combatant receiver) {
			// TODO: who goes first?


			float creatureHPA = creatureCombatant.Health;
			float mpA = magic;

			Dialogue.Display($"Creature performs {skill.Name}!");
			yield return Wait.Until(Dialogue.IsDone);

			ApplyEffects(skill, actor, receiver);
			magic = Mathf.Clamp(magic - skill.Cost, 0, magicTotal);

			// TODO: update health and statuses
			Vector3 enemyHealthA = EnemyHealthTransform.localScale;
			Vector3 enemyHealthB = new((float) enemyCombatant.Health / (float) enemyCombatant.HealthTotal, 1, 1);

			Vector3 creatureHealthA = CreatureHealthTransform.localScale;
			Vector3 creatureHealthB = new((float) creatureCombatant.Health / (float) creatureCombatant.HealthTotal, 1, 1);

			Vector3 creatureMagicA = CreatureMagicTransform.localScale;
			Vector3 creatureMagicB = new((float) magic / (float) magicTotal, 1, 1);

			float creatureHPB = creatureCombatant.Health;
			float mpB = magic;

			string hpLabel = CreatureHealthLabel.text;
			string mpLabel = CreatureMagicLabel.text;

			yield return Do.For(1f, ratio => {
				EnemyHealthTransform.localScale = Vector3.Lerp(enemyHealthA, enemyHealthB, ratio);
				CreatureHealthTransform.localScale = Vector3.Lerp(creatureHealthA, creatureHealthB, ratio);
				CreatureMagicTransform.localScale = Vector3.Lerp(creatureMagicA, creatureMagicB, ratio);

				hpLabel = $"{Mathf.FloorToInt(Mathf.Lerp(creatureHPA, creatureHPB, ratio))}";
				mpLabel = $"{Mathf.FloorToInt(Mathf.Lerp(mpA, mpB, ratio))}";

				if (CreatureHealthLabel.text != hpLabel) {
					CreatureHealthLabel.text = hpLabel;
				}

				if (CreatureMagicLabel.text != mpLabel) {
					CreatureMagicLabel.text = mpLabel;
				}
			}, Easing.EaseInOutSine01);

			//
			yield return TurnEnd();
		}

		IEnumerator TurnEnd() {
			// TODO: anyone dead?


			yield return null;

			TurnStart();
		}

		void BattleEnd() {
		}

		void ShowActions() {
			ActionMenu.SetActive(true);
			MovesMenu.SetActive(false);

			FirstAction.Select();
			FirstAction.OnSelect(null);
		}

		public void ShowMoves() {
			phase = Phase.BattleShowMoves;

			//
			ActionMenu.SetActive(false);
			MovesMenu.SetActive(true);

			//
			List<Button> activeButtons = new();

			for (int i = 0; i < 4; i++) {
				Button button = MoveButtons[i];
				TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();

				if (i >= currentCreature.Skills.Count) {
					label.text = "-";
					continue;
				}

				//
				Skill skill = currentCreature.Skills[i];
				label.text = skill != null ? skill.Name : "-";
				button
					.GetComponent<InformationButton>()
					.Configure(() => {
						MoveDescription.text = skill.Description;
						MoveCost.text = $"Cost: {skill.Cost}";
						MoveUses.text = $"({Mathf.FloorToInt(Engine.Profile.Magic / skill.Cost)} Uses)";
					});

				//
				activeButtons.Add(button);
			}

			//
			for (int i = 0; i < activeButtons.Count; i++) {
				int previous = i <= 0 ? activeButtons.Count - 1 : i - 1;
				int next = i >= activeButtons.Count - 1 ? 0 : i + 1;

				Navigation navigation = activeButtons[i].navigation;
				navigation.selectOnUp = activeButtons[previous];
				navigation.selectOnDown = activeButtons[next];

				activeButtons[i].navigation = navigation;
			}

			//
			activeButtons[0].Select();
			activeButtons[0].OnSelect(null);

			//
			CancelAction.performed += ExitToActions;

		}

		void ExitToActions(InputAction.CallbackContext ctx) {
			CancelAction.performed -= ExitToActions;

			ShowActions();
		}

		public void UseMove(int moveIndex) {
			MovesMenu.SetActive(false);

			StartCoroutine(TurnAction(currentCreature.Skills[moveIndex], creatureCombatant, enemyCombatant));
		}

		public void ShowItems() {
			Debug.Log("Items");
		}

		public void ShowCreatures() {
			Debug.Log("Creatures");
		}

		public void Run() {
			Debug.Log("Run");
		}

		void ApplyEffects(Skill skill, Combatant actor, Combatant receiver) {
			skill.Effect.ForEach(effect => {
				Combatant effected = effect.ApplyToSelf ? actor : receiver;

				switch (effect.Type) {
					case EffectType.Health:
						effected.Health = Mathf.Clamp(effected.Health - effect.Value, 0, effected.HealthTotal);
						break;

					case EffectType.Status:
						effected.AddStatus(effect.Status, effect.Value);
						break;
				}
			});

		}
	}
}
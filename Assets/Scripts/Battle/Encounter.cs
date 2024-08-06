using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		[SerializeField]
		Animator EnemyAttacked;

		[SerializeField]
		Animator CreatureAttacked;

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

			enemyCreature = enemy.RollAppearance();
			enemyCombatant = Combatant.New(enemyCreature.Name, enemy.Level, 5, 5, 5, 5, 5, 5, 999);

			currentCreatureIndex = 0;
			currentCreature = Engine.Profile.Party[currentCreatureIndex];

			creatureCombatants.Add(
				Combatant.New(
					currentCreature.Name,
					Engine.Profile.Level,
					5, 5, 5, 5,
					Engine.Profile.Wisdom,
					5,
					999,
					Engine.Profile.Magic
				)
			);

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

			Dialogue.Display("An angry spirit emerges.", $"Lethia calls forth {currentCreature.Name}!");
			yield return Wait.Until(Dialogue.IsDone);

			//
			CreatureQueue.SetActive(false);
			Statistics.SetActive(true);

			//
			Vector3 enemyHealthA = new(0, 1, 1);
			Vector3 enemyHealthB = new((float) enemyCombatant.Health / enemyCombatant.HealthTotal, 1, 1);

			Vector3 creatureHealthA = new(0, 1, 1);
			Vector3 creatureHealthB = new((float) creatureCombatant.Health / creatureCombatant.HealthTotal, 1, 1);

			Vector3 creatureMagicA = new(0, 1, 1);
			Vector3 creatureMagicB = new((float) creatureCombatant.Magic / creatureCombatant.MagicTotal, 1, 1);

			float creatureHPA = 0;
			float creatureHPB = creatureCombatant.Health;

			float mpA = 0;
			float mpB = creatureCombatant.Magic;

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

		IEnumerator TurnAction(Skill skill, Combatant actor, Combatant receiver, Animator actorAnimator, Animator receiverAnimator) {
			float creatureHPA = creatureCombatant.Health;
			float mpA = creatureCombatant.Magic;

			//
			Dialogue.Display($"{actor.Name} performs {skill.Name}!");
			yield return Wait.Until(Dialogue.IsDone);

			//
			ApplyEffects(skill, actor, receiver);
			actor.AdjustMagic(-skill.Cost);

			// show skill fx(s)
			yield return Wait.For(0.33f);

			List<Animator> active = new();
			List<bool> done = new();

			foreach (SkillFX fx in skill.FX) {
				Animator animator = fx.Actor ? actorAnimator : receiverAnimator;
				if (active.Contains(animator)) {
					Debug.LogError($"Already have this {fx.Actor} animator in action!");
					continue;
				}

				active.Add(animator);
				done.Add(false);

				StartCoroutine(AnimateFX(skill, fx, animator, done, done.Count - 1));
			}

			yield return Wait.Until(() => done.All(isDone => isDone));
			yield return Wait.For(0.33f);

			// update health and statuses
			Vector3 enemyHealthA = EnemyHealthTransform.localScale;
			Vector3 enemyHealthB = new((float) enemyCombatant.Health / (float) enemyCombatant.HealthTotal, 1, 1);

			Vector3 creatureHealthA = CreatureHealthTransform.localScale;
			Vector3 creatureHealthB = new((float) creatureCombatant.Health / (float) creatureCombatant.HealthTotal, 1, 1);

			Vector3 creatureMagicA = CreatureMagicTransform.localScale;
			Vector3 creatureMagicB = new((float) creatureCombatant.Magic / (float) creatureCombatant.MagicTotal, 1, 1);

			float creatureHPB = creatureCombatant.Health;
			float mpB = creatureCombatant.Magic;

			string hpLabel = CreatureHealthLabel.text;
			string mpLabel = CreatureMagicLabel.text;

			yield return Do.For(0.5f, ratio => {
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

			IEnumerator creatureAction = TurnAction(currentCreature.Skills[moveIndex], creatureCombatant, enemyCombatant, CreatureAttacked, EnemyAttacked);
			IEnumerator enemyAction = TurnAction(enemyCreature.Skills[0], enemyCombatant, creatureCombatant, EnemyAttacked, CreatureAttacked);

			if (creatureCombatant.Dexterity >= enemyCombatant.Dexterity) {
				StartCoroutine(ExecuteActions(creatureAction, enemyAction));
			} else {
				StartCoroutine(ExecuteActions(enemyAction, creatureAction));
			}
		}

		IEnumerator ExecuteActions(IEnumerator first, IEnumerator second) {
			yield return first;
			yield return second;

			//
			yield return TurnEnd();
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

				Debug.Log($"{skill.Name} {effect.Type} {effect.Strength}");

				switch (effect.Type) {
					case EffectType.Health:
						effected.AdjustHealth(effect.Strength);

						Debug.Log(effected.Health);
						break;

					case EffectType.Status:
						effected.AddStatus(effect.Status, effect.Duration, effect.Strength);
						break;
				}
			});

		}

		IEnumerator AnimateFX(Skill skill, SkillFX fx, Animator animator, List<bool> done, int doneIndex) {
			AnimationClip clip = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == skill.Name);
			if (clip == null) {
				Debug.LogError($"No animation clip named: {skill.Name}");
				done[doneIndex] = true;
				yield break;
			}

			//
			if (fx.Delay > 0) {
				yield return Wait.For(fx.Delay);
			}

			//
			animator.Play(skill.Name);

			//
			yield return Wait.Until(() => animator.GetCurrentAnimatorStateInfo(0).IsName(skill.Name));
			yield return Wait.Until(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(skill.Name));

			//
			done[doneIndex] = true;
		}
	}
}
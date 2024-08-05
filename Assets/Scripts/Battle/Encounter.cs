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

		static Encounter Self;
		Creature CurrentCreature;

		WorldEnemy.Enemy enemy;

		Combatant enemyCombatant;
		List<Combatant> creatureCombatants = new();

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
		}

		void StartBattle(WorldEnemy.Enemy enemy) {
			Engine.Mode = EngineMode.Battle;

			this.enemy = enemy;

			magic = Engine.Profile.Magic;
			magicTotal = Engine.Profile.MagicTotal;

			enemyCombatant = new Combatant { Health = 30, HealthTotal = 30 };

			currentCreatureIndex = 0;
			CurrentCreature = Engine.Profile.Party[currentCreatureIndex];

			creatureCombatants.Add(new Combatant { Health = 50, HealthTotal = 50 });

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

			Dialogue.Display("An angry spirit emerges.");
			yield return Wait.Until(Dialogue.IsDone);

			TurnStart();
		}

		void TurnStart() {
			ShowActions();
		}

		IEnumerator TurnAction(Skill skill, Combatant actor, Combatant receiver) {
			// TODO: who goes first?

			Dialogue.Display($"Creature performs {skill.Name}!");
			yield return Wait.Until(Dialogue.IsDone);

			ApplyEffects(skill, actor, receiver);
			magic = Mathf.Clamp(magic - skill.Cost, 0, magicTotal);

			// TODO: update health and statuses

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
			CreatureQueue.SetActive(false);

			FirstAction.Select();
			FirstAction.OnSelect(null);
		}

		public void ShowMoves() {
			ActionMenu.SetActive(false);
			MovesMenu.SetActive(true);

			//
			List<Button> activeButtons = new();

			for (int i = 0; i < 4; i++) {
				Button button = MoveButtons[i];
				TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();

				if (i >= CurrentCreature.Skills.Count) {
					label.text = "-";
					continue;
				}

				//
				Skill skill = CurrentCreature.Skills[i];
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
		}

		public void UseMove(int moveIndex) {
			MovesMenu.SetActive(false);

			StartCoroutine(TurnAction(CurrentCreature.Skills[moveIndex], creatureCombatants[currentCreatureIndex], enemyCombatant));
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
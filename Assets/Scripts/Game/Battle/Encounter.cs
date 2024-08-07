using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Battle {
	public class Encounter : MonoBehaviour {
		#region Unity Connections
		[Header("Globals")]
		[SerializeField] Engine Engine;
		[SerializeField] Player Player;
		[SerializeField] PlayerInput PlayerInput;

		[Header("Actions")]
		[SerializeField] GameObject ActionMenu;
		[SerializeField] Button FirstAction;

		[Header("Items")]
		[SerializeField] GameObject ItemsMenu;
		[SerializeField] GameObject ItemMenuItemInfo;
		[SerializeField] TextMeshProUGUI ItemMenuItemDescription;
		[SerializeField] TextMeshProUGUI ItemMenuItemFlavor;
		[SerializeField] GameObject ItemMenuItemButtonTemplate;
		[SerializeField] Button ItemMenuCancelButton;

		[Header("Moves")]
		[SerializeField] GameObject MovesMenu;
		[SerializeField] TextMeshProUGUI MoveDescription;
		[SerializeField] TextMeshProUGUI MoveCost;
		[SerializeField] TextMeshProUGUI MoveUses;
		[SerializeField] List<Button> MoveButtons;

		[Header("Summons")]
		[SerializeField] GameObject CreaturesMenu;

		[Header("Transition")]
		[SerializeField] GameObject Cover;
		[SerializeField] GameObject CoverMask;

		[Header("Start View")]
		[SerializeField] GameObject Content;
		[SerializeField] GameObject CreatureQueue;

		[Header("Basic View")]
		[SerializeField] GameObject Statistics;
		[SerializeField] TextMeshProUGUI EnemyNameLabel;
		[SerializeField] RectTransform EnemyHealthTransform;
		[SerializeField] TextMeshProUGUI CreatureNameLabel;
		[SerializeField] TextMeshProUGUI CreatureHealthLabel;
		[SerializeField] TextMeshProUGUI CreatureMagicLabel;
		[SerializeField] RectTransform CreatureHealthTransform;
		[SerializeField] RectTransform CreatureMagicTransform;
		[SerializeField] Animator EnemyAttacked;
		[SerializeField] Animator CreatureAttacked;
		[SerializeField] Image Creature;
		[SerializeField] Image Enemy;
		[SerializeField] Dialogue Dialogue;
		#endregion

		#region Local Variables
		static Encounter Self;

		InputAction SubmitAction;
		InputAction CancelAction;

		WorldEnemy.Enemy enemy;
		Creature enemyCreature;
		Creature currentCreature;
		private Combatant enemyCombatant;
		List<Combatant> creatureCombatants = new();
		Combatant creatureCombatant;

		int currentCreatureIndex;
		#endregion

		#region MonoBehavior
		void Start() {
			Hide(Cover);
			Hide(Content);

			SubmitAction = PlayerInput.currentActionMap.FindAction("Submit");
			CancelAction = PlayerInput.currentActionMap.FindAction("Cancel");
		}
		#endregion

		#region Starting the Battle
		public void StartBattle(WorldEnemy.Enemy enemy) {
			Engine.Mode = EngineMode.Battle;

			this.enemy = enemy;

			enemyCreature = enemy.RollAppearance();
			enemyCombatant = Combatant.New(enemyCreature.Name, enemy.Level, 5, 5, 5, 5, 5, 5, 999);

			currentCreatureIndex = 0;
			currentCreature = Engine.Profile.Party.Creatures[currentCreatureIndex];

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

			enemy.Stop();
			Player.Stop();

			StartCoroutine(StartingBatle());
		}

		IEnumerator StartingBatle() {
			Show(Cover);
			Cover.transform.position = Player.transform.position + new Vector3(0, 0, 20f);
			CoverMask.transform.localScale = Vector3.one * 2;

			Player.GetComponent<SpriteRenderer>().sortingOrder = 11;
			enemy.GetComponentInChildren<SpriteRenderer>().sortingOrder = 11;

			yield return Wait.For(1f);
			yield return Do.For(1f, ratio => {
				CoverMask.transform.localScale = (1 - ratio) * 2 * Vector3.one;
			});
			yield return Wait.For(1f);

			Show(Content);
			Show(CreatureQueue);
			Hide(ItemsMenu);
			Hide(ActionMenu);
			Hide(MovesMenu);
			Hide(CreaturesMenu);
			Hide(Statistics);
			Show(Creature.gameObject);
			Show(Enemy.gameObject);


			yield return Dialogue.DisplayAndWait("An angry spirit emerges.", $"Lethia calls forth {currentCreature.Name}!");

			//
			Hide(CreatureQueue);
			Show(Statistics);

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
		#endregion

		#region Flow
		void TurnStart() {
			ShowActions();
		}

		IEnumerator TurnEnd() {
			if (DidSomeoneDie()) {
				yield return SomeoneDied();
				yield break;
			}

			TurnStart();
		}

		IEnumerator BattleEnd() {
			int experience = enemyCombatant.Experience();

			List<string> pages = new() {
				$"{enemyCombatant.Name} defeated!",
				$"Lethia gained {experience} experience."
			};

			Engine.Profile.Experience += experience;
			if (Engine.Profile.Experience > Engine.Profile.ExperienceForNextLevel) {
				Engine.Profile.Level += 1;
				Engine.Profile.Experience -= Engine.Profile.ExperienceForNextLevel;
				Engine.Profile.ExperienceForNextLevel = Mathf.RoundToInt(Mathf.Pow(50, 1 + (0.1f * (Engine.Profile.Level - 1))));

				pages.Add($"She's now level {Engine.Profile.Level}!");
			}

			Engine.Profile.Magic = creatureCombatant.Magic;

			yield return Dialogue.DisplayAndWait(pages.ToArray());

			Hide(Content);
			Hide(enemy.gameObject);
			Hide(Cover);

			Destroy(enemy);

			Engine.Mode = EngineMode.PlayerControl;
			enemy = null;
		}
		#endregion

		#region Actions
		void ShowActions() {
			Show(ActionMenu);
			Hide(MovesMenu);
			Hide(ItemsMenu);

			Select(FirstAction);
		}

		void ExitToActions() {
			CancelAction.performed -= CancelButtonPressed;

			ShowActions();
		}

		void CancelButtonPressed(InputAction.CallbackContext ctx) {
			ExitToActions();
		}
		#endregion

		#region Moves
		public void ShowMoves() {
			Hide(ActionMenu);
			Show(MovesMenu);

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
			Select(activeButtons[0]);

			//
			CancelAction.performed += CancelButtonPressed;
		}

		public void UseMove(int moveIndex) {
			Hide(MovesMenu);

			PerformActions(
				PerformMove(currentCreature.Skills[moveIndex], creatureCombatant, enemyCombatant, CreatureAttacked, EnemyAttacked),
				EnemyAction()
			);
		}

		IEnumerator PerformMove(Skill skill, Combatant actor, Combatant receiver, Animator actorAnimator, Animator receiverAnimator) {
			float creatureHPA = creatureCombatant.Health;
			float mpA = creatureCombatant.Magic;

			//
			yield return Dialogue.DisplayAndWait($"{actor.Name} performs {skill.Name}!");

			//
			ApplyEffects(skill.Effect, actor, receiver);
			actor.AdjustMagic(-skill.Cost);

			// show skill fx(s)
			yield return Wait.For(0.33f);

			List<Animator> active = new();
			List<bool> done = new();

			foreach (Game.SkillFX fx in skill.FX) {
				Animator animator = fx.Actor ? actorAnimator : receiverAnimator;
				if (active.Contains(animator)) {
					Debug.LogError($"Already have this {fx.Actor} animator in action!");
					continue;
				}

				active.Add(animator);
				done.Add(false);

				StartCoroutine(AnimateFX(skill.Name, fx, animator, done, done.Count - 1));
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

		IEnumerator EnemyAction() {
			return PerformMove(enemyCreature.Skills[0], enemyCombatant, creatureCombatant, EnemyAttacked, CreatureAttacked);
		}
		#endregion

		#region Items
		public void ShowItems() {
			Hide(ActionMenu);
			Hide(ItemMenuItemInfo);
			Show(ItemsMenu);

			//
			int offset = 0;
			while (ItemsMenu.transform.childCount > 3 || offset > ItemsMenu.transform.childCount) {
				Transform child = ItemsMenu.transform.GetChild(offset);
				GameObject go = child.gameObject;

				if (
						go == ItemMenuCancelButton.gameObject ||
						go == ItemMenuItemButtonTemplate ||
						go == ItemMenuItemInfo
					) {
					offset += 1;
					continue;
				}

				child.SetParent(null);
				Hide(child.gameObject);
				Destroy(child.gameObject);
			}


			// generate items 
			List<Button> buttons = new();
			Engine.Profile.Inventory
				.Where(entry =>
					entry.Item != null
					&& entry.Amount > 0
					&& entry.Item.Type == Game.ItemType.Consumable
					&& entry.Item.UseInBattle
				)
				.ToList()
				.ForEach(entry => {
					GameObject newItem = Instantiate(ItemMenuItemButtonTemplate, ItemsMenu.transform);

					TextMeshProUGUI[] labels = newItem.GetComponentsInChildren<TextMeshProUGUI>();

					labels[0].text = entry.Item.Name;
					labels[1].text = $"x{entry.Amount}";

					if (entry.Item.Type != Game.ItemType.Consumable) {
						Color color = labels[0].color;
						color.a = 0.5f;

						labels[0].color = color;
						labels[1].color = color;
					}

					Button button = newItem.GetComponent<Button>();
					button.onClick.AddListener(() => OnItemSelected(entry.Item));
					button
					.GetComponent<InformationButton>()
						.Configure(() => {
							Hide(ItemMenuItemInfo);
							ItemMenuItemDescription.text = entry.Item.Description;
							ItemMenuItemFlavor.text = entry.Item.FlavorText;
						});

					buttons.Add(button);

					Show(newItem);
				});

			// set cancel button last
			ItemMenuCancelButton.transform.SetAsLastSibling();
			ItemMenuCancelButton
			.GetComponent<InformationButton>()
				.Configure(() => Hide(ItemMenuItemInfo));

			buttons.Add(ItemMenuCancelButton);

			for (int i = 0; i < buttons.Count; i++) {
				int previous = i <= 0 ? buttons.Count - 1 : i - 1;
				int next = i >= buttons.Count - 1 ? 0 : i + 1;

				Navigation navigation = buttons[i].navigation;
				navigation.selectOnUp = buttons[previous];
				navigation.selectOnDown = buttons[next];

				buttons[i].navigation = navigation;
			}

			//
			Select(buttons[0]);
			buttons[0].GetComponent<InformationButton>().OnSelect(null);

			//
			CancelAction.performed += CancelButtonPressed;
		}

		void OnItemSelected(Item item) {
			Hide(ItemsMenu);
			PerformActions(
				UseItem(item, creatureCombatant, enemyCombatant, CreatureAttacked, EnemyAttacked),
				EnemyAction()
			);
		}

		IEnumerator UseItem(Item item, Combatant actor, Combatant receiver, Animator actorAnimator, Animator receiverAnimator) {
			Hide(ActionMenu);

			float creatureHPA = creatureCombatant.Health;
			float mpA = creatureCombatant.Magic;

			//
			yield return Dialogue.DisplayAndWait($"Lethia uses {item.Name}!");

			//
			ApplyEffects(item.Effects, actor, receiver);
			Engine.Profile.AdjustItem(item, -1);

			// show skill fx(s)
			yield return Wait.For(0.33f);

			List<Animator> active = new();
			List<bool> done = new();

			foreach (Game.SkillFX fx in item.FX) {
				Animator animator = fx.Actor ? actorAnimator : receiverAnimator;
				if (active.Contains(animator)) {
					Debug.LogError($"Already have this {fx.Actor} animator in action!");
					continue;
				}

				active.Add(animator);
				done.Add(false);

				StartCoroutine(AnimateFX($"Item: {item.Name}", fx, animator, done, done.Count - 1));
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

		public void OnItemsCancel() {
			ExitToActions();
		}
		#endregion

		void PerformActions(IEnumerator creatureAction, IEnumerator enemyAction) {
			if (creatureCombatant.Dexterity >= enemyCombatant.Dexterity) {
				StartCoroutine(ExecuteActions(creatureAction, enemyAction));
				return;
			}

			StartCoroutine(ExecuteActions(enemyAction, creatureAction));
		}

		IEnumerator ExecuteActions(IEnumerator first, IEnumerator second) {
			yield return first;

			if (!DidSomeoneDie()) {
				yield return second;
			}

			yield return TurnEnd();
		}

		bool DidSomeoneDie() {
			return enemyCombatant.IsDead || creatureCombatant.IsDead;
		}

		IEnumerator SomeoneDied() {
			List<string> pages = new();

			if (creatureCombatant.IsDead) {
				Hide(Creature.gameObject);

				pages.Add($"{creatureCombatant.Name} collapsed!");

				if (Engine.Profile.Party.AvailableToFight - 1 > 0) {
					Debug.Log("Choose another!");
				} else {
					pages.Add("No creatures left to fight...");
					pages.Add("Lethia returns to her village.");

					yield return Dialogue.DisplayAndWait(pages.ToArray());

					Engine.NextScene = new NextScene { Name = Village.Scene.Name, Destination = Village.Scene.Location_Tree };
					SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Additive);
					yield break;
				}
			} else if (enemyCombatant.IsDead) {
				Hide(Enemy.gameObject);
				pages.Add($"{enemyCombatant.Name} collapsed!");
				yield return Dialogue.DisplayAndWait(pages.ToArray());
				yield return BattleEnd();
			} else {
				Debug.Assert(true, "No one actually died!");
				throw new System.Exception("No one actually died!");
			}
		}

		public void ShowMagic() {
			Debug.Log("Magic");
		}

		public void ShowCreatures() {
			Debug.Log("Creatures");
		}

		public void Run() {
			Debug.Log("Run");
		}

		void ApplyEffects(List<Game.Effect> effects, Combatant actor, Combatant receiver) {
			effects.ForEach(effect => {
				Combatant effected = effect.ApplyToSelf ? actor : receiver;

				switch (effect.Type) {
					case Game.EffectType.Health:
						effected.AdjustHealth(
							Mathf.RoundToInt(
								effect.Strength * Random.Range(1 - effect.Variance, 1 + effect.Variance)
							)
						);
						break;

					case Game.EffectType.Status:
						effected.AddStatus(effect.Status, effect.Duration, effect.Strength);
						break;
				}
			});

		}

		IEnumerator AnimateFX(string name, Game.SkillFX fx, Animator animator, List<bool> done, int doneIndex) {
			AnimationClip clip = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == name);
			if (clip == null) {
				Debug.LogError($"No animation clip named: {name}");
				done[doneIndex] = true;
				yield break;
			}

			//
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

		void Show(GameObject gameObject) {
			gameObject.SetActive(true);
		}

		void Hide(GameObject gameObject) {
			gameObject.SetActive(false);
		}

		void Select(Button button) {
			button.Select();
			button.OnSelect(null);
		}
	}
}
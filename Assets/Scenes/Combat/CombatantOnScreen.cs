using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using S = UnityEngine.SerializeField;

// -----------------------------------------------------------------------------

namespace Combat {
	public class CombatantOnScreen : MonoBehaviour {

		// -------------------------------------------------------------------------

		[S] Slider Health;
		[S] TextMeshProUGUI Head;
		[S] TextMeshProUGUI Torso;
		[S] TextMeshProUGUI Tail;
		[S] List<TextMeshProUGUI> Appendages;

		[Header("Player")]
		[S] Slider Magic;
		[S] GameObject Labels;
		[S] TextMeshProUGUI HealthLabel;
		[S] TextMeshProUGUI MagicLabel;

		// -------------------------------------------------------------------------

		Game.Creature creature;

		// -------------------------------------------------------------------------

		public void HideBars() {
			Health.gameObject.SetActive(false);
			if (Magic != null)
				Magic.gameObject.SetActive(false);
			if (Labels != null)
				Labels.SetActive(false);
		}

		public void ShowBars() {
			Health.gameObject.SetActive(true);
			if (Magic != null)
				Magic.gameObject.SetActive(true);
			if (Labels != null)
				Labels.SetActive(true);
		}

		public void Configure(Game.Creature creature) {
			this.creature = creature;

			//
			Head.gameObject.SetActive(!creature.MissingHead);
			if (!creature.MissingHead)
				Head.text = creature.Head.BodyPart.Name;

			Torso.gameObject.SetActive(!creature.MissingTorso);
			if (!creature.MissingTorso)
				Torso.text = creature.Torso.BodyPart.Name;

			Tail.gameObject.SetActive(!creature.MissingTail);
			if (!creature.MissingTail)
				Tail.text = creature.Tail.BodyPart.Name;

			Do.Times(
				Appendages.Count,
				index => {
					bool missing = creature.MissingAppendage(index);
					Appendages[index].gameObject.SetActive(!missing);

					if (!missing) {
						Appendages[index].text = creature.GetAppendage(index).BodyPart.Name;
					}
				}
			);
		}

		public void UpdateMagic(int magic, int magicTotal) {
			if (Magic == null) {
				return;
			}

			//
			Magic.minValue = 0;
			Magic.maxValue = magicTotal;
			Magic.value = magic;

			//
			if (MagicLabel != null) {
				MagicLabel.text = $"MP {magic,3:d0}";
			}
		}

		public void UpdateHealth(int health, int healthTotal) {
			Health.minValue = 0;
			Health.maxValue = healthTotal;
			Health.value = health;

			//
			if (HealthLabel != null) {
				HealthLabel.text = $"HP {health,4:d0}";
			}
		}

		// -------------------------------------------------------------------------

		public void FancyHealthFillUp(float ratio) {
			UpdateHealth(
				Mathf.RoundToInt(creature.Health * ratio),
				creature.HealthTotal
			);
		}

		public void FancyMagicFillUp(float ratio, int magic, int magicTotal) {
			if (Magic == null) {
				return;
			}

			// 
			UpdateMagic(Mathf.RoundToInt(magic * ratio), magicTotal);
		}

		// -------------------------------------------------------------------------

	}
}
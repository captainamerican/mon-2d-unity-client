using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

namespace Combat {
	public class CombatantOnScreen : MonoBehaviour {

		// -------------------------------------------------------------------------

		[SerializeField] Slider Health;
		[SerializeField] TextMeshProUGUI Head;
		[SerializeField] TextMeshProUGUI Torso;
		[SerializeField] TextMeshProUGUI Tail;
		[SerializeField] List<TextMeshProUGUI> Appendages;

		[Header("Player")]
		[SerializeField] Slider Magic;
		[SerializeField] GameObject Labels;
		[SerializeField] TextMeshProUGUI HealthLabel;
		[SerializeField] TextMeshProUGUI MagicLabel;

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
			Torso.gameObject.SetActive(!creature.MissingTorso);
			Tail.gameObject.SetActive(!creature.MissingTail);
			Do.Times(
				Appendages.Count,
				index => Appendages[index].gameObject.SetActive(
					!creature.MissingAppendage(index)
				)
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

		public void FancyFillUp(float ratio, int magic = 0, int magicTotal = 0) {
			int healthNow = Mathf.RoundToInt(creature.Health * ratio);
			Debug.Log(healthNow + " " + creature.Health + " " + ratio + " " + (creature.Health * ratio));
			UpdateHealth(healthNow, creature.HealthTotal);

			if (Magic != null) {
				int magicNow = Mathf.RoundToInt(magic * ratio);
				UpdateMagic(magicNow, magicTotal);
			}
		}

		public void Refresh() {
		}

		public IEnumerator Refreshing() {
			yield return null;
		}

		// -------------------------------------------------------------------------

	}
}
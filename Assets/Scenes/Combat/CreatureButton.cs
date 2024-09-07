using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------

public class CreatureButton : MonoBehaviour {

	// ---------------------------------------------------------------------------

	[SerializeField] Engine Engine;

	[SerializeField] Button Button;
	[SerializeField] TextMeshProUGUI Name;
	[SerializeField] Slider Health;

	[SerializeField] BodyPartButton Head;
	[SerializeField] BodyPartButton Torso;
	[SerializeField] BodyPartButton Tail;
	[SerializeField] List<BodyPartButton> Appendages;

	[SerializeField] List<SkillButton> Skills;

	// ---------------------------------------------------------------------------

	Game.Creature creature;

	// ---------------------------------------------------------------------------

	public void Configure(Game.Creature creature) {
		this.creature = creature;

		//
		Name.text = creature.Name;

		Health.minValue = 0;
		Health.maxValue = creature.HealthTotal;
		Health.value = creature.Health;

		Button.interactable = creature.Health > 0;
	}

	public void Display() {
		Head.Configure(creature.Head);
		Torso.Configure(creature.Torso);

		Tail.gameObject.SetActive(creature.MissingTail);
		Tail.Configure(creature.Tail);

		Do.ForEach(Appendages, (button, index) => {
			var bodyPart = creature.GetAppendage(index);

			button.gameObject.SetActive(bodyPart != null);
			button.Configure(bodyPart);
		});

		Do.ForEach(Skills, (button, index) => {
			var skill = creature.GetSkill(index);
			if (Skill.Missing(skill)) {
				button.gameObject.SetActive(false);
				return;
			}

			var skillEntry = Engine.Profile.Skills.Find(se => se.SkillId == skill.Id);
			if (skillEntry == null) {
				button.gameObject.SetActive(false);
				return;
			}

			button.gameObject.SetActive(true);
			button.Configure(skillEntry);
		});
	}

	// ---------------------------------------------------------------------------

}

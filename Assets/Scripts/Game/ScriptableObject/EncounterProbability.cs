using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(fileName = "EncounterProbability", menuName = "MoN/EncounterProbability")]
public class EncounterProbability : ScriptableObject {
	[SerializeField] List<Game.EncounterPossibility> Possibilities = new();

	public Game.EncounterPossibility Roll() {
		int total = Possibilities.Select(x => x.Weight).Sum();
		int random = Random.Range(0, total);

		for (int j = 0; j < Possibilities.Count; j++) {
			Game.EncounterPossibility possibility = Possibilities[j];
			if (random < possibility.Weight) {
				return possibility;
			}

			random -= possibility.Weight;
		}

		//
		return Possibilities[0];
	}
}

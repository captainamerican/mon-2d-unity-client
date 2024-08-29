using System;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class Options {

		// -------------------------------------------------------------------------

		[Range(0, 1)]
		public float DialogueSpeed = 0.25f;

		[Range(0, 1)]
		public float MusicVolume = 0.66f;

		[Range(0, 1)]
		public float SFXVolume = 0.66f;

		public bool BattleAnimations = true;
		public bool CheatMenu = false;
		public bool Speedrunning = false;

		// -------------------------------------------------------------------------

	}
}
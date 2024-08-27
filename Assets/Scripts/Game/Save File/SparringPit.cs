using System;
using System.Collections.Generic;

using UnityEngine;

// -----------------------------------------------------------------------------

namespace Game {
	[Serializable]
	public class SparringPit {

		// -------------------------------------------------------------------------

		public HeadBodyPartEntry Head;
		public TorsoBodyPartEntry Torso;
		public TailBodyPartEntry Tail;
		public List<AppendageBodyPartEntry> Appendage = new(4);

		public InventoryEntry Enhancer;

		public SparringPit() {
			Appendage = new(4);
		}

		// -------------------------------------------------------------------------

		public AppendageBodyPartEntry GetAppendage(int index) {
			return index < Appendage.Count ? Appendage[index] : null;
		}

		public void SetAppendage(AppendageBodyPartEntry appendage, int index) {
			Debug.Assert(index > -1 && index < 4, $"Index ({index}) wrong");

			//
			if (Appendage.Count < 4) {
				Do.Times(4, i => {
					if (Appendage.Count < 4) {
						Appendage.Add(null);
					}
				});
			}

			//
			Appendage[index] = appendage;
		}

		public void Set(BodyPartEntryBase entry) {
			if (entry is HeadBodyPartEntry head) {
				Head = head;
			} else if (entry is TorsoBodyPartEntry torso) {
				Torso = torso;
			} else if (entry is TailBodyPartEntry tail) {
				Tail = tail;
			}
		}

		public void Set(AppendageBodyPartEntry appendage, int index) {
			SetAppendage(appendage, index);
		}

		public void Set(InventoryEntry entry) {
			Enhancer = entry;
		}

		public void Train() {
			int experience = 5;
			float qualityDecrease = 0;

			if (Enhancer?.Item != null && (Enhancer?.Amount ?? 0) > 0) {
				Enhancer.Amount -= 1;

				//
				List<Effect> effects = Enhancer.Item.Effects;
				Debug.Assert(effects.Count == 2, "Enhancer doesn't have right effects count");

				Effect boost = effects[0];
				Effect downside = effects[1];
				Debug.Assert(boost.Type == EffectType.Experience, "boost isn't of type experiece");
				Debug.Assert(downside.Type == EffectType.BodyPartQuality, "boost isn't of type body part quality");

				float xpBoost = (float) boost.Strength / (float) 100;
				float bqRatio = (float) downside.Strength / (float) 100;

				experience = (int) Mathf.Round((float) experience * xpBoost);
				qualityDecrease = bqRatio;

				//
				if (Enhancer.Amount < 1) {
					Enhancer = null;
				}
			}

			//
			if (Head?.BodyPart != null && Head.Quality > 0) {
				Head.Experience += experience;
				Head.Quality -= qualityDecrease;
			}

			if (Torso?.BodyPart != null && Torso.Quality > 0) {
				Torso.Experience += experience;
				Torso.Quality -= qualityDecrease;
			}

			if (Tail?.BodyPart != null && Tail.Quality > 0) {
				Tail.Experience += experience;
				Tail.Quality -= qualityDecrease;
			}

			Appendage.ForEach(appendage => {
				if (appendage?.BodyPart == null && appendage.Quality > 0) {
					return;
				}

				appendage.Experience += experience;
				appendage.Quality -= qualityDecrease;
			});
		}

		// -------------------------------------------------------------------------
	}
}
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
			int baseExperience = 5;
			int experience = baseExperience;
			float qualityDecrease = 0;
			bool enhancerExists = false;

			//
			if (Enhancer?.Item != null && (Enhancer?.Amount ?? 0) > 0) {
				enhancerExists = true;

				//
				List<Effect> effects = Enhancer.Item.Effects;
				Debug.Assert(effects.Count == 2, "Enhancer doesn't have right effects count");

				Effect boost = effects[0];
				Effect downside = effects[1];
				Debug.Assert(boost.Type == EffectType.Experience, "boost isn't of type experiece");
				Debug.Assert(downside.Type == EffectType.BodyPartQuality, "boost isn't of type body part quality");

				float xpBoost = (float) boost.Strength / (float) 100;
				float bqRatio = (float) downside.Strength / (float) 100;

				experience = Mathf.CeilToInt((float) experience * xpBoost);
				qualityDecrease = Mathf.Clamp(bqRatio, 0.01f, 1f);
			}

			//
			bool usedEnhancer = false;
			if (Head?.BodyPart != null) {
				if (Head.Quality > 0) {
					usedEnhancer = true;

					//
					Head.Experience += experience;
					Head.Quality = Mathf.Clamp01(Head.Quality - qualityDecrease);
				} else {
					Head.Experience += 1;
				}
			}

			if (Torso?.BodyPart != null) {
				if (Torso.Quality > 0) {
					usedEnhancer = true;

					//
					Torso.Experience += experience;
					Torso.Quality = Mathf.Clamp01(Torso.Quality - qualityDecrease);
				} else {
					Torso.Experience += 1;
				}
			}

			if (Tail?.BodyPart != null) {
				if (Tail.Quality > 0) {
					usedEnhancer = true;

					//
					Tail.Experience += experience;
					Tail.Quality = Mathf.Clamp01(Tail.Quality - qualityDecrease);
				} else {
					Tail.Experience += 1;
				}
			}

			Appendage.ForEach(appendage => {
				if (appendage?.BodyPart != null) {
					if (appendage.Quality > 0) {
						usedEnhancer = true;

						//
						appendage.Experience += experience;
						appendage.Quality = Mathf.Clamp01(appendage.Quality - qualityDecrease);
					} else {
						appendage.Experience += 1;
					}
				}
			});

			//
			if (enhancerExists && usedEnhancer) {
				Enhancer.Amount -= 1;

				//
				if (Enhancer.Amount < 1) {
					Enhancer = null;
				}
			}
		}

		// -------------------------------------------------------------------------
	}
}
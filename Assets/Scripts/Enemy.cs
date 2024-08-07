using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;

using UnityEditor;

using UnityEngine;
using UnityEngine.AI;

namespace WorldEnemy {

	public enum Personality {
		Passive,
		Aggressive,
		ImmediateBattle,
		Fearful,
		ImmediateDisappear
	}

	public enum Alertness {
		Unaware,
		Alert,
		Fleeing,
		Chasing,
		InBattle,
		GaveUp
	}

	[Serializable]
	public class Possibility {
		public Creature Creature;
		public int Weight = 100;
	}

	[SelectionBase]
	public class Enemy : MonoBehaviour {
		[SerializeField]
		Personality Personality;

		[SerializeField, ReadOnly]
		public Alertness Alertness;

		[SerializeField]
		float DomainRadius;

		[SerializeField]
		public float AwarenessRadius;

		[SerializeField]
		float ActionRadius;

		[SerializeField]
		public Transform BodyTransform;

		[SerializeField]
		Animator BodyAnimator;

		[SerializeField]
		Rigidbody2D Body;

		[SerializeField]
		NavMeshAgent Agent;

		[SerializeField]
		GameObject Alert;

		[SerializeField]
		List<Possibility> Possibilities = new();

		[SerializeField]
		public int Level = 1;

		[Header("Unaware Settings")]
		[SerializeField]
		float Speed;

		[SerializeField]
		float Acceleration;

		[Header("Action Settings")]
		[SerializeField]
		float ActionDelay;

		[SerializeField]
		float ActionSpeed;

		[SerializeField]
		float ActionAcceleration;

		Vector3 destination;
		Vector3 alertedPosition;
		Transform target;

		void Start() {
			Agent.updateRotation = false;
			Agent.updateUpAxis = false;
			Agent.speed = Speed;
			Agent.acceleration = Acceleration;

			Alert.SetActive(false);

			ChooseNextDestination();
			StartCoroutine(MoveToDestination());
		}

		void LateUpdate() {
			if (Alertness != Alertness.Fleeing && Alertness != Alertness.Chasing) {
				return;
			}

			if (Vector3.Distance(BodyTransform.position, alertedPosition) >= ActionRadius) {
				Debug.Log("stop!");
				StopActionAndReturn();
				StartCoroutine(StopActionAndReturn());
				return;
			}

			switch (Alertness) {
				case Alertness.Chasing:
					Agent.SetDestination(target.position);
					break;
			}
		}

		void ChooseNextDestination() {
			float radians = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
			float distance = UnityEngine.Random.Range(0, DomainRadius);

			destination.x = transform.position.x + Mathf.Cos(radians) * distance;
			destination.y = transform.position.y + Mathf.Sin(radians) * distance;

			Agent.SetDestination(destination);
			StartCoroutine(MoveToDestination());
		}

		IEnumerator MoveToDestination() {
			float elapsed = 0;

			yield return Wait.Until(() => {
				elapsed += Time.deltaTime;
				if (elapsed > 5f) {
					return true;
				}

				if (Agent.pathPending) {
					return false;
				}

				switch (Agent.pathStatus) {
					case NavMeshPathStatus.PathInvalid:
					case NavMeshPathStatus.PathPartial:
						return true;

					default:
						break;
				}

				return Vector3.Distance(BodyTransform.position, destination) < 1.5f;
			});
			ReachedDestination();
		}

		void ReachedDestination() {
			StartCoroutine(WaitingToMove());
		}

		IEnumerator WaitingToMove() {
			if (UnityEngine.Random.value > 0.8f) {
				yield return Wait.For(UnityEngine.Random.Range(0.25f, 4f));
			}
			ChooseNextDestination();
		}

		private void OnDrawGizmos() {
			if (!EditorApplication.isPlaying) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(transform.position, DomainRadius);

				Gizmos.color = Color.black;
				Gizmos.DrawSphere(destination, 0.5f);

				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(BodyTransform.position, ActionRadius);

				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(BodyTransform.position, AwarenessRadius);
			} else {
				switch (Alertness) {
					case Alertness.Unaware:
						Gizmos.color = Color.magenta;
						Gizmos.DrawWireSphere(transform.position, DomainRadius);

						Gizmos.color = Color.red;
						Gizmos.DrawWireSphere(BodyTransform.position, AwarenessRadius);

						Gizmos.color = Color.black;
						Gizmos.DrawSphere(destination, 0.5f);
						break;

					case Alertness.Alert:
						break;

					case Alertness.Chasing:
					case Alertness.Fleeing:
						Gizmos.color = Color.blue;
						Gizmos.DrawWireSphere(alertedPosition, ActionRadius);
						break;
				}
			}
		}

		public void Stop() {
			Agent.isStopped = true;
		}

		public void TargetEnteredAwarenessRange(Transform target) {
			StopAllCoroutines();

			this.target = target;

			Alertness = Alertness.Alert;
			Agent.isStopped = true;
			Agent.ResetPath();

			alertedPosition = BodyTransform.position;

			StartCoroutine(ShockThenAction());
		}

		IEnumerator ShockThenAction() {
			Alert.SetActive(true);
			yield return Wait.For(ActionDelay);

			Alert.SetActive(false);

			switch (Personality) {
				case Personality.Passive:
					StartCoroutine(Do.After(2f, () => {
						Alertness = Alertness.Unaware;
						Agent.speed = Speed;
						Agent.acceleration = Acceleration;
						ChooseNextDestination();
					}));
					break;

				case Personality.Aggressive:
					Alertness = Alertness.Chasing;
					Agent.isStopped = false;
					Agent.speed = ActionSpeed;
					Agent.acceleration = ActionAcceleration;
					break;

				case Personality.Fearful:
					Alertness = Alertness.Fleeing;
					Agent.isStopped = false;
					Agent.speed = ActionSpeed;
					Agent.acceleration = ActionAcceleration;
					break;

				case Personality.ImmediateDisappear:
					gameObject.SetActive(false);
					break;

				case Personality.ImmediateBattle:
					gameObject.SetActive(false);
					Debug.Log("BATTLE!");
					break;
			}
		}

		IEnumerator StopActionAndReturn() {
			Alertness = Alertness.GaveUp;

			Agent.isStopped = true;
			Agent.ResetPath();

			alertedPosition = Vector3.zero;

			yield return Wait.For(2f);

			Agent.SetDestination(transform.position);
			Agent.isStopped = false;
			Agent.speed = ActionSpeed / 2f;
			Agent.acceleration = ActionAcceleration / 2f;

			Alertness = Alertness.Unaware;

			yield return MoveToDestination();

			Agent.speed = Speed;
			Agent.acceleration = Acceleration;
		}

		public Creature RollAppearance() {
			int total = Possibilities.Select(x => x.Weight).Sum();
			int random = UnityEngine.Random.Range(0, total);

			for (int j = 0; j < Possibilities.Count; j++) {
				Possibility possibility = Possibilities[j];
				if (random < possibility.Weight) {
					return possibility.Creature;
				}

				random -= possibility.Weight;
			}

			//
			return Possibilities[0].Creature;
		}
	}
}
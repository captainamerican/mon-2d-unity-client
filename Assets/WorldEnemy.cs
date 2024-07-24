using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WorldEnemy {

	public enum Personality {
		Passive,
		Aggressive,
		Fearful
	}

	public enum Alertness {
		Unaware,
		Alert,
		Fleeing,
		Chasing
	}

	public enum Id {
		Wolf
	}

	public class Possibility {
		public Id Id = Id.Wolf;
		public int Weight = 100;
		public int Level = 1;
	}

	public class WorldEnemy : MonoBehaviour {
		[SerializeField]
		Personality Personality;

		[SerializeField]
		Alertness Alertness;

		[SerializeField]
		float DomainRadius;

		[SerializeField]
		float AwarenessRadius;

		[SerializeField]
		float ChaseRadius;

		[SerializeField]
		float FleeRadius;

		[SerializeField]
		float Speed;

		[SerializeField]
		Transform BodyTransform;

		[SerializeField]
		Animator BodyAnimator;

		[SerializeField]
		Rigidbody2D Body;

		[SerializeField]
		NavMeshAgent Agent;

		[SerializeField]
		List<Possibility> Possibilities = new();

		Vector3 destination;

		Vector3 alertedPosition;

		// Start is called before the first frame update
		void Start() {
			ChooseNextDestination();
			StartCoroutine(MoveToDestination());

			Agent.updateRotation = false;
			Agent.updateUpAxis = false;
		}

		// Update is called once per frame
		void Update() {

		}

		void ChooseNextDestination() {
			Debug.Log("ChooseNextDestination");

			float radians = Random.Range(0f, 360f) * Mathf.Deg2Rad;
			float distance = Random.Range(0, DomainRadius);

			destination.x = transform.position.x + Mathf.Cos(radians) * distance;
			destination.y = transform.position.y + Mathf.Sin(radians) * distance;

			Agent.SetDestination(destination);
			StartCoroutine(MoveToDestination());
		}

		IEnumerator MoveToDestination() {
			float elapsed = 0;

			Debug.Log("MoveToDestination");
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
			Debug.Log("ReachedDestination");
			StartCoroutine(WaitingToMove());
		}

		IEnumerator WaitingToMove() {
			Debug.Log("WaitingToMove");
			if (Random.value > 0.8f) {
				yield return Wait.For(Random.Range(0.25f, 4f));
			}
			ChooseNextDestination();
		}

		void PlayerSighted() {
			StopAllCoroutines();
		}



		private void OnDrawGizmos() {
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, DomainRadius);

			Gizmos.color = Color.black;
			Gizmos.DrawSphere(destination, 0.5f);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(BodyTransform.position, ChaseRadius);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(BodyTransform.position, AwarenessRadius);
		}
	}
}
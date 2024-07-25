using System.Collections.Generic;

using UnityEngine;

enum PlayerDirection {
	None,
	Up,
	Down,
	Left,
	Right,
}

public class Player : MonoBehaviour {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	float MovementSpeed = 5f;

	[SerializeField]
	Rigidbody2D rb;

	[SerializeField]
	Animator animator;

	Vector2 movement;
	PlayerDirection direction = PlayerDirection.None;
	PlayerDirection nextDirection = PlayerDirection.None;

	Dictionary<PlayerDirection, string> DirectionMap = new Dictionary<PlayerDirection, string>() {
		{ PlayerDirection.Up, "Up" },
		{ PlayerDirection.Down, "Down" },
		{ PlayerDirection.Left, "Left" },
		{ PlayerDirection.Right, "Right" }
	};

	private void Start() {
		Engine.ModeChanged += ModeChanged;
	}

	private void OnDestroy() {
		Engine.ModeChanged -= ModeChanged;
	}

	void Update() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		if (movement.x > 0 && movement.y == 0) {
			nextDirection = PlayerDirection.Right;
		} else if (movement.x < 0 && movement.y == 0) {
			nextDirection = PlayerDirection.Left;
		} else if (movement.y > 0 && movement.x == 0) {
			nextDirection = PlayerDirection.Up;
		} else if (movement.y < 0 && movement.x == 0) {
			nextDirection = PlayerDirection.Down;
		}

		if (nextDirection != direction) {
			direction = nextDirection;
			animator.Play(DirectionMap[direction]);
			animator.speed = 1;
		}

		if (direction != PlayerDirection.None && movement.x == 0 && movement.y == 0) {
			Stop();
		}
	}

	void FixedUpdate() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		rb.MovePosition(rb.position + MovementSpeed * Time.fixedDeltaTime * movement.normalized);
	}

	void ModeChanged(EngineMode mode) {
		switch (mode) {
			case EngineMode.Menu:
				direction = PlayerDirection.Down;
				nextDirection = PlayerDirection.Down;
				animator.Play(DirectionMap[direction]);
				Stop();
				break;

			case EngineMode.PlayerControl:
				break;

			default:
				break;
		}
	}

	void Stop() {
		animator.Play(DirectionMap[direction], -1, 0.33f);
		animator.speed = 0;
		movement = Vector2.zero;
		direction = PlayerDirection.None;
		nextDirection = PlayerDirection.None;
	}
}
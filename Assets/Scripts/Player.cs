using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	[SerializeField]
	Engine Engine;

	[SerializeField]
	float MovementSpeed = 5f;

	[SerializeField]
	Rigidbody2D rb;

	[SerializeField]
	Animator animator;

	[SerializeField]
	PlayerInput PlayerInput;

	Vector2 movement;
	Game.PlayerDirection direction = Game.PlayerDirection.None;
	Game.PlayerDirection nextDirection = Game.PlayerDirection.None;

	InputAction Move;

	Dictionary<Game.PlayerDirection, string> DirectionMap = new() {
		{
			Game.PlayerDirection.Up,
			"Up"
		},
		{
			Game.PlayerDirection.Down,
			"Down"
		},
		{
			Game.PlayerDirection.Left,
			"Left"
		},
		{
			Game.PlayerDirection.Right,
			"Right"
		}
	};

	private void Start() {
		Engine.ModeChanged += ModeChanged;

		Move = PlayerInput.currentActionMap.FindAction("Move");
	}

	private void OnDestroy() {
		Engine.ModeChanged -= ModeChanged;
	}

	void Update() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		movement = Move.ReadValue<Vector2>();

		if (movement.x > 0 && movement.y == 0) {
			nextDirection = Game.PlayerDirection.Right;
		} else if (movement.x < 0 && movement.y == 0) {
			nextDirection = Game.PlayerDirection.Left;
		} else if (movement.y > 0 && movement.x == 0) {
			nextDirection = Game.PlayerDirection.Up;
		} else if (movement.y < 0 && movement.x == 0) {
			nextDirection = Game.PlayerDirection.Down;
		}

		if (nextDirection != direction) {
			direction = nextDirection;
			animator.Play(DirectionMap[direction]);
			animator.speed = 1;
		}

		if (direction != Game.PlayerDirection.None && movement.x == 0 && movement.y == 0) {
			Stop();
		}
	}

	void FixedUpdate() {
		if (Engine.Mode != EngineMode.PlayerControl) {
			return;
		}

		rb.MovePosition(rb.position + MovementSpeed * Time.fixedDeltaTime * movement.normalized);
		Engine.Profile.CurrentLocation = transform.position;
	}

	void ModeChanged(EngineMode mode) {
		switch (mode) {
			case EngineMode.Store:
				direction = Game.PlayerDirection.Down;
				nextDirection = Game.PlayerDirection.Down;
				animator.Play(DirectionMap[direction]);
				Stop();
				break;

			case EngineMode.PlayerControl:
				break;

			default:
				break;
		}
	}

	public void Stop() {
		if (direction == Game.PlayerDirection.None) {
			return;
		}

		animator.Play(DirectionMap[direction], -1, 0.33f);
		animator.speed = 0;
		movement = Vector2.zero;
		direction = Game.PlayerDirection.None;
		nextDirection = Game.PlayerDirection.None;
	}
}

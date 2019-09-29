using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputListener))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerPath))]
public class PlayerController : MonoBehaviour
{
	protected InputListener inputs;
	protected PlayerMovement movement;
	protected PlayerPath path;

	private void Awake()
	{
		inputs = GetComponent<InputListener>();
		movement = GetComponent<PlayerMovement>();
		path = GetComponent<PlayerPath>();
	}

	public void UpdateInputs()
	{
		movement.Move(inputs);
	}

	public void UpdatePosition(GravityBody[] obstacles, float dt)
	{
		movement.UpdatePosition(obstacles, dt);
	}

	public void UpdatePath(GravityBody[] obstacles, float dt)
	{
		path.UpdatePath(movement, obstacles, dt);
	}
}

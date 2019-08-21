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

	private void Start()
	{
		movement.ApplyConfig(path.Simulator);
	}

	public void UpdateInputs()
	{
		movement.Move(inputs);
	}

	public void UpdatePosition(ObstacleBody[] obstacles, float dt)
	{
		movement.UpdatePosition(obstacles, dt);
	}

	public void UpdatePath(ObstacleBody[] obstacles, float dt)
	{
		path.UpdatePath(movement.Velocity, obstacles, dt);
	}
}

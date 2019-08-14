using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputListener))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerPath))]
public class PlayerController : MonoBehaviour
{
	public Rigidbody player;

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
		player.constraints =
			RigidbodyConstraints.FreezePositionZ |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;

		movement.rb = player;
	}

	private void Update()
	{
		movement.move(inputs);
	}
}

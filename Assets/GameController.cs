using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public InputController inputs;
	public PlayerController player;
	public ObstacleController obstacles;

	private void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;

		player.UpdatePosition(obstacles.GetObstacles(), dt);
	}

	void Update()
	{
		float dt = Time.deltaTime;

		player.UpdateInputs();
		player.UpdatePath(obstacles.GetObstacles(), dt);
	}
}

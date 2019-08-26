using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static int LAYER_HIDE = 9;

	public InputController inputs;
	public PlayerController player;
	public ObstacleController obstacles;

	void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;
		GravityBody[] visibleObstacles = obstacles.GetObstacles();

		player.UpdatePosition(visibleObstacles, dt);
		player.UpdatePath(visibleObstacles, dt);
	}

	void Update()
	{
		float dt = Time.deltaTime;

		player.UpdateInputs();
	}
}

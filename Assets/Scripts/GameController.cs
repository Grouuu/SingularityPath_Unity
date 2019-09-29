using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	public Camera sceneCamera;
	public InputController inputs;
	public PlayerController player;
	public ObstacleController obstacles;

	void Awake()
	{
		if (!instance)
			instance = this;
	}

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
		obstacles.UpdateObstacle(new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, 0));
	}

	public void PlayerHitObstacle(ContactPoint contact)
	{
		Debug.Log("CRASH!");
	}
}

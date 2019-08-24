using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	public GravityBody[] GetObstacles()
	{
		// TEST
		GravityBody[] obstacles = FindObjectsOfType<GravityBody>();

		return obstacles;
	}
}

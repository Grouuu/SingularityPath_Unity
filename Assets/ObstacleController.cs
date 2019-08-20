using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	protected ObstacleBody[] obstacles;

	public ObstacleBody[] GetObstacles()
	{
		return obstacles;
	}
}

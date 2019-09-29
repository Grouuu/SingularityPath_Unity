using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerPath : MonoBehaviour
{
	public LineRenderer line;
	public int pathLenght = 50;
	public int multiplierStep = 5;

	/**
	 * Update the simulated visual trajectory of the player
	 */
	public void UpdatePath(PlayerMovement player, GravityBody[] obstacles, float dt)
	{
		int nbStep = pathLenght + 1; // +1 for initial position

		List<Vector3> points = new List<Vector3>();

		Vector3 position = player.Position;
		Vector3 velocity = player.Velocity;

		points.Add(position);

		for (int step = 1; step < nbStep * multiplierStep; step++)
		{
			Vector3 oldPosition = position;

			velocity = player.UpdateVelocity(velocity, position, obstacles, dt);
			position += velocity * dt;

			Vector3 direction = position - oldPosition;
			RaycastHit hit;

			// if hit a GravityBody
			if (Physics.Raycast(position, direction.normalized, out hit, direction.magnitude)
				&& hit.collider.GetComponent<GravityBody>())
			{
				points.Add(hit.point);
				break; // break the path
			}

			if (step % multiplierStep == 1)
				points.Add(position); // add a step each X simulations
		}

		line.positionCount = points.Count;
		line.SetPositions(points.ToArray());
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{
	public LineRenderer line;
	public int pathLenght = 50;
	public int multiplierStep = 5;

	protected Gravity gravity = new Gravity();

	/**
	 * Update the simulated visual trajectory of the player
	 * The trajectory is calculated with same player velocity and updated gravity force
	 */
	public void UpdatePath(PlayerMovement player, GravityBody[] obstacles, float dt)
	{
		int nbStep = pathLenght + 1; // +1 for initial position

		Vector3[] points = new Vector3[nbStep];
		Vector3 position = player.Position;

		points[0] = position;

		for (int step = 1; step < nbStep * multiplierStep; step++)
		{
			position += player.Velocity * dt;

			Vector3 gravityForce = gravity.GetGravity(obstacles, position) * dt;
			if (gravityForce.magnitude > player.gravityMax)
				gravityForce = gravityForce.normalized * player.gravityMax;

			position += gravityForce;

			if (step % multiplierStep == 1)
				points[step / multiplierStep] = position; // add a step each X simulations
		}

		line.positionCount = points.Length;
		line.SetPositions(points);
	}
}

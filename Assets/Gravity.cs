using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	private const float G = 667.4f;
	private const int gravityDivider = 500;

	public Vector3 GetGravity(GravityBody[] obstacles, Vector3 subjectPosition, float subjectMass)
	{
		Vector3 gravity = Vector3.zero;

		foreach (GravityBody body in obstacles)
		{
			Vector3 dir = body.position - subjectPosition;
			float dist = dir.magnitude;

			// TODO : make the gravity effect :
			// - far : visible at low speed
			// - near : visible at max speed
			// - close : avoid by speed max with tangent trajectory or boost

			if (dist > 0 && dist > body.radius)
			{
				float force = G * (body.mass * subjectMass) / Mathf.Pow(dist, 2); // F = ((m1 * m2) / d²) * G
				force /= gravityDivider; // clamp
				gravity += dir.normalized * force;
			}
		}

		return gravity;
	}
}

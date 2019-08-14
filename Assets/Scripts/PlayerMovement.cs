using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Vector3 startVelocity = Vector3.zero;
	public Quaternion startAngle = Quaternion.identity;
	public float speedAcc = 0.5f;
	public float speedDec = 0.5f;
	public float speedMin = 0.5f;
	public float speedMax = 15f;
	public float angleSpeed = 1f;
	public float angleInertiaDec = 8f;
	public float boostForce = 1f;

	[HideInInspector] public Rigidbody rb;

	private Vector3 velocity;
	private Quaternion angle;
	private Quaternion angleInertia;

	private void Start()
	{
		velocity = startVelocity;
		angle = startAngle;
		angleInertia = Quaternion.identity;
	}

	private void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;

		Debug.DrawLine(rb.position, rb.position + angle * velocity * dt * 100);

		rb.rotation = angle;
		rb.position += angle * velocity * dt;

		// inertia

		if (angleInertia != Quaternion.identity)
		{
			angleInertia = Quaternion.Slerp(angleInertia, Quaternion.identity, dt * angleInertiaDec);
			angle *= angleInertia;
		}
	}

	public void AddGravity(Vector3 gravity)
	{
		// no rotation if the player is stopped
		Quaternion rotation = Quaternion.FromToRotation(velocity, velocity + gravity);

		// when the player is stopped (velocity == Vector3.zero) or if gravity is null
		if (rotation == Quaternion.identity)
		{
			// care, if we use Vector3.right before the velocity/angle changes and before the render, Vector3.right may be not good
			rotation = Quaternion.FromToRotation(Vector3.right, velocity + gravity);
		}

		angle *= rotation;
	}

	public void Accelerate(float speed)
	{
		velocity += transform.right * speedAcc * speed;

		if (velocity.magnitude > speedMax)
			velocity = Vector3.ClampMagnitude(velocity, speedMax);
	}

	public void Decelerate(float speed)
	{
		Vector3 force = -transform.right * speedDec;

		if (force.magnitude + speedMin > velocity.magnitude) // no backward + avoid stop
			force = Vector3.zero;

		velocity += force;
	}

	public void Turn(float speed)
	{
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, -speed * angleSpeed));
		angle *= rotation;
		angleInertia *= rotation;
	}

	public void Boost()
	{
		velocity += velocity.normalized * boostForce;

		// TODO : temporary increase speedMax by boostForce then reduce it progressively to the normal
	}

	public void move(InputListener input)
	{
		if (input.top > 0 && input.bot == 0)
			Accelerate(input.top);
		if (input.bot < 0 && input.top == 0)
			Decelerate(input.bot);

		if (input.right > 0 && input.left == 0)
			Turn(input.right);
		if (input.left < 0 && input.right == 0)
			Turn(input.left);

		if (input.space)
			Boost();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Tooltip("Started velocity")]
	public Vector3 startVelocity = Vector3.zero;
	[Tooltip("Started angle")]
	public Quaternion startAngle = Quaternion.identity;
	public float speedAcc = 0.5f;
	public float speedDec = 0.5f;
	public float speedMax = 15f;
	public float speedMin = 0.1f;
	private bool lockSpeedMin = true;
	public float angleSpeed = 1f;
	public float angleInertiaDec = 8f;
	public float gravityAngleMax = 0.1f;
	public Vector3 boostForce = Vector3.zero;
	public float boostDuration = 2f;

	[HideInInspector] public Rigidbody rb;

	private Quaternion angle;
	private Vector3 velocity;

	private Quaternion angleInertia = Quaternion.identity;
	private float boostTime = 0f;

	private void Start()
	{
		angle = startAngle;
		velocity = angle * startVelocity;
	}

	private void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;

		// boost

		boostTime = boostTime - dt > 0 ? boostTime - dt : 0;

		// clamp

		if (lockSpeedMin && velocity.magnitude >= speedMin)
			lockSpeedMin = false; // lock the speed to min when exceeded

		float velocityBoost = boostForce.magnitude;
		float velocityMax = boostTime == 0 ? speedMax : speedMax + velocityBoost;
		float velocityMin = lockSpeedMin ? 0 : speedMin;

		if (velocity.magnitude > velocityMax)
			velocity = Vector3.ClampMagnitude(velocity, velocityMax); // clamp max

		if (velocity.magnitude < velocityMin)
			velocity = velocity.normalized * velocityMin; // clamp min

		// apply

		Debug.DrawLine(rb.position, rb.position + angle * velocity * dt * 100);

		rb.rotation = angle;
		rb.position += angle * velocity * dt;

		// inertia
		
		if (angleInertia != Quaternion.identity)
		{
			angleInertia = Quaternion.Slerp(angleInertia, Quaternion.identity, angleInertiaDec * dt);
			angle *= angleInertia; // rotation inertia (on input)
		}
	}

	public void AddGravity(Vector3 gravity)
	{
		Vector3 fromVelocity = (velocity != Vector3.zero) ? velocity : transform.right;
		Vector3 targetVelocity = fromVelocity + gravity;
		Vector3 clampedGravity = Vector3.RotateTowards(fromVelocity, targetVelocity, gravityAngleMax, targetVelocity.magnitude); // clamp

		float degree = Vector3.Angle(fromVelocity, clampedGravity);
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, degree));

		angle *= rotation;
	}

	public void Accelerate(float intensity)
	{
		velocity += transform.right * speedAcc * intensity;
	}

	public void Decelerate(float intensity)
	{
		Vector3 force = -transform.right * speedDec;

		if (force.magnitude + speedMin > velocity.magnitude)
			force = Vector3.zero; // no backward + avoid stop

		velocity += force;
	}

	public void Turn(float intensity)
	{
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleSpeed * -intensity));
		angle *= rotation;
		angleInertia *= rotation;
	}

	public void Boost()
	{
		velocity += boostForce; // TODO : increase/decrease with ease
		boostTime = boostDuration;
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

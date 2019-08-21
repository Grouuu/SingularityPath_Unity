using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody playerBody;
	public Vector3 startVelocity = Vector3.zero;
	public float speedAcc = 0.2f;
	public float speedDec = 0.1f;
	public float speedMax = 5f;
	[Range(0.2f, 5.0f)] public float speedMin = 0.2f; // if < 0.2, trigger === Vector3.zero
	public float angleSpeed = 1f;
	public float boostForce = 2f;
	public float boostDuration = 2f;

	protected Vector3 velocity;
	protected Gravity gravity = new Gravity();

	private float boostTime = 0f;
	private float speedCap;

	public void ApplyConfig(PlayerMovement clone)
	{
		// TODO : observable system to update simulator if the config changed
		// or externalize the parameters and applies on all instances
		clone.startVelocity = startVelocity;
		clone.speedAcc = speedAcc;
		clone.speedDec = speedDec;
		clone.speedMax = speedMax;
		clone.speedMin = speedMin;
		clone.angleSpeed = angleSpeed;
		clone.boostForce = boostForce;
		clone.boostDuration = boostDuration;
	}

	private void Start()
	{
		playerBody.constraints =
			RigidbodyConstraints.FreezePositionZ |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY;

		velocity = startVelocity;
		
		UpdatePosition(null, 1f); // apply start setup
	}

	public void UpdatePosition(ObstacleBody[] obstacles, float dt)
	{
		speedCap = speedMax;

		if (boostTime > 0)
		{
			speedCap += boostForce;
			//boostTime = boostTime - dt < 0 ? 0 : boostTime - dt;
			boostTime = Mathf.Lerp(boostTime, 0, dt);
			// TODO : Mathf.Lerp(boostTime, 0, dt); ?
		}

		if (obstacles != null && obstacles.Length > 0)
			AddGravity(gravity.GetGravity(obstacles));

		Debug.DrawLine(playerBody.position, playerBody.position + velocity * dt * 100); // DEBUG

		if (velocity.magnitude > 0)
		{
			playerBody.position += velocity * dt;
			playerBody.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
		}
	}

	public void Accelerate(float intensity) // intensity > 0
	{
		velocity += getVelocity().normalized * speedAcc * intensity;
		ClampMax(); // here to avoid unexpected velocity value in other methods
	}

	public void Decelerate(float intensity) // intensity < 0
	{
		velocity += getVelocity().normalized * speedDec * intensity;
		ClampMin(); // here to avoid unexpected velocity value in other methods
	}

	public void Turn(float intensity)
	{
		float magnitude = velocity.magnitude; // don't use transform.right

		velocity = Quaternion.Euler(new Vector3(0, 0, angleSpeed * -intensity)) * getVelocity();

		if (magnitude == 0)
		{
			// apply the rotation if the player don't move
			playerBody.rotation *= Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
		}

		velocity = velocity.normalized * magnitude; // avoid to speed up
	}

	public void Boost()
	{
		velocity += getVelocity().normalized * boostForce;
		ClampMax();
		boostTime = boostDuration;
	}

	public void AddGravity(Vector3 gravity)
	{
		velocity += gravity;
	}

	public void Move(InputListener input)
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

	protected void ClampMax()
	{
		if (velocity.magnitude > speedCap)
			velocity = getVelocity().normalized * speedMax;
	}

	protected void ClampMin()
	{
		if (velocity.magnitude < speedMin)
			velocity = getVelocity().normalized * speedMin;
	}

	private Vector3 getVelocity()
	{
		return velocity == Vector3.zero ? transform.right : velocity;
	}

	public Vector3 Velocity => velocity;
}

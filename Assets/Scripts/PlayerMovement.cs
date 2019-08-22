using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody playerBody;
	public Vector3 startVelocity = Vector3.zero;
	public float speedAcc = 0.5f;
	public float speedDec = 0.1f;
	public float speedMax = 10f;
	[Range(0.2f, 5.0f)] public float speedMin = 0.2f; // if < 0.2, trigger === Vector3.zero
	public float angleSpeed = 1f;
	public float boostForce = 2f;
	public float boostDuration = 1f;

	protected Vector3 velocity;
	protected Vector3 direction; // should not == Vector.zero
	protected Gravity gravity = new Gravity();

	private float boostTime = 0f;
	private float speedCap;

	/**
	 * Apply the same configuration to an another PlayerMovement
	 */
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

	/**
	 * Initialize
	 */
	private void Start()
	{
		playerBody.constraints =
			RigidbodyConstraints.FreezePositionZ |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY;

		velocity = startVelocity;
		direction = startVelocity.magnitude > 0 ? startVelocity : transform.right;
		speedCap = speedMax;

		UpdatePosition(null, 1f); // apply start setup
	}

	/**
	 * Move and rotate the rigidbody with the gravity
	 */
	public void UpdatePosition(ObstacleBody[] obstacles, float dt)
	{
		if (boostTime > 0)
		{
			// TODO : change the boost system
			// . keep space pressed : boost
			// . release space : gradually slowdown to speedMax
			// . add boost time / fuel quantity limit

			speedCap = speedMax + boostForce;
			boostTime = boostTime - dt < 0 ? 0 : boostTime - dt;
		}
		else
		{
			speedCap = Mathf.Lerp(speedCap, speedMax, dt * 10);
		}

		if (obstacles != null && obstacles.Length > 0)
			AddExternalForce(gravity.GetGravity(obstacles));

		Debug.DrawLine(playerBody.position, playerBody.position + velocity * dt * 100); // DEBUG

		if (direction.magnitude > 0)
		{
			playerBody.position += velocity * dt;
			playerBody.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
		}
	}

	/**
	 * Increase the speed on the forward direction
	 * Cap the speed to the max
	 */
	public void Accelerate(float intensity) // intensity > 0
	{
		velocity += getVelocity().normalized * speedAcc * intensity;
		ClampMax(); // here to avoid unexpected velocity value in other methods
	}

	/**
	 * Decrease the speed on the backward
	 * Cap the speed to the min (prevent to stop)
	 */
	public void Decelerate(float intensity) // intensity < 0
	{
		if(velocity.magnitude != 0)
		{
			velocity += getVelocity().normalized * speedDec * intensity;
			ClampMin(); // here to avoid unexpected velocity value in other methods
		}
	}

	/**
	 * Rotate the direction and the velocity without changes the speed
	 */
	public void Turn(float intensity)
	{
		float previousMagnitude = velocity.magnitude; // don't use transform.right
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleSpeed * -intensity));

		velocity = rotation * getVelocity();
		direction = rotation * direction;

		velocity = velocity.normalized * previousMagnitude; // avoid to speed up
	}

	/**
	 * Apply a boost force on the forward
	 * Increase the speed max temporally
	 */
	public void Boost()
	{
		velocity += getVelocity().normalized * boostForce;
		ClampMax();
		boostTime = boostDuration;
	}

	/**
	 * Modify the velocity and the direction with external force (eg. gravity)
	 */
	public void AddExternalForce(Vector3 force)
	{
		velocity += force;
		direction = Quaternion.Euler(force) * direction;
	}

	/**
	 * Use the current inputs to modifiy the velocity and the direction
	 * Listen :
	 * - left/right (rotation CCW/CW)
	 * - up/down (accelerate/decelerate)
	 * - space key (boost)
	 */
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

	/**
	 * Limit the magnitude to the magnitude max
	 */
	protected void ClampMax()
	{
		if (velocity.magnitude > speedCap)
			velocity = getVelocity().normalized * speedMax;
	}

	/**
	 * Limit the magnitude to the magnitude min
	 */
	protected void ClampMin()
	{
		if (velocity.magnitude < speedMin)
			velocity = getVelocity().normalized * speedMin;
	}

	/**
	 * Return the current velocity or the direction if the velocity magnitude is equal to 0
	 */
	private Vector3 getVelocity()
	{
		return velocity == Vector3.zero ? direction : velocity;
	}

	/** The velocity of the rigid body */
	public Vector3 Velocity => velocity;
	/** The direction of the rigid body */
	public Vector3 Direction => direction;
}

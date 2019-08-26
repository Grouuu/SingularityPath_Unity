using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody playerBody;
	public Vector3 startVelocity = Vector3.zero;
	public float speedAcc = 0.5f;
	public float speedDec = 0.1f;
	public float speedMax = 10f;
	public float speedMin = 0.2f;
	public float angleSpeed = 1f;
	public float boostForce = 2f;
	public float boostDecDuration = 1f;
	public float gravityMax = 10f;

	protected Gravity gravity = new Gravity();

	protected Vector3 velocity; // velocity based only on inputs (w/o gravity)
	protected Vector3 direction; // forward direction (should not == Vector.zero)

	private float boostTime = 0f; // duration remains on stopped boost
	private float speedCap; // max speed of the player

	/**
	 * Initialize
	 */
	void Start()
	{
		playerBody.constraints =
			RigidbodyConstraints.FreezePositionZ |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY;

		velocity = startVelocity;
		direction = startVelocity.magnitude > 0 ? startVelocity : transform.right; // default direction = →
		speedCap = speedMax;

		UpdatePosition(null, 1f); // apply start setup
	}

	/**
	 * Validate the parameter values (on editor)
	 */
	void OnValidate()
	{
		speedMin = Mathf.Max(speedMin, 0.2f); // if < 0.2, trigger === Vector3.zero
		gravityMax = Mathf.Max(gravityMax, speedAcc + 0.1f); // must be > speedMax
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
		if (input.space)
			Boost(); // first apply the speed cap

		if (input.top > 0 && input.bot == 0)
			Accelerate(input.top);
		if (input.bot < 0 && input.top == 0)
			Decelerate(input.bot);

		if (input.right > 0 && input.left == 0)
			Turn(input.right);
		if (input.left < 0 && input.right == 0)
			Turn(input.left);
	}

	/**
	 * Move and rotate the rigidbody with the gravity
	 */
	public void UpdatePosition(GravityBody[] obstacles, float dt)
	{
		// Gravity

		if (obstacles != null && obstacles.Length > 0)
			AddGravity(gravity.GetGravity(obstacles, playerBody.position) * dt);

		// Move

		ClampSpeedMax(); // apply speedCap decrease

		Debug.DrawLine(playerBody.position, playerBody.position + velocity * dt * 100); // DEBUG

		if (direction.magnitude > 0)
		{
			playerBody.position += velocity * dt;
			playerBody.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
		}

		// Boost

		boostTime = boostTime - dt < 0 ? 0 : boostTime - dt;

		if (boostTime == 0)
			speedCap = speedMax;
		else
			speedCap = Mathf.Lerp(speedMax + boostForce, speedMax, 1 - boostTime / boostDecDuration); // decrease the speed cap if no more boost
	}

	/**
	 * Increase the speed on the forward direction
	 * Cap the speed to the max
	 */
	public void Accelerate(float intensity) // intensity > 0
	{
		velocity += getVelocity().normalized * speedAcc * intensity;
		ClampSpeedMax();
	}

	/**
	 * Decrease the speed on the backward
	 * Cap the speed to the min (prevent to stop)
	 */
	public void Decelerate(float intensity) // intensity < 0
	{
		Vector3 decelerate = getVelocity().normalized * speedDec * intensity;

		if (velocity.magnitude > decelerate.magnitude)
		{
			velocity += decelerate;
			ClampSpeedMin();
		}
	}

	/**
	 * Rotate the direction and the velocity without changes the speed
	 */
	public void Turn(float intensity)
	{
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleSpeed * -intensity));
		float previousMagnitude = velocity.magnitude;

		velocity = rotation * getVelocity();
		velocity = velocity.normalized * previousMagnitude; // avoid to speed up
		direction = rotation * direction;
	}

	/**
	 * Apply a boost force on the forward
	 * Increase the speed max temporally
	 */
	public void Boost()
	{
		velocity += direction.normalized * boostForce;
		speedCap = speedMax + boostForce;
		boostTime = boostDecDuration;
		ClampSpeedMax();
	}

	/**
	 * Modify the velocity and the direction with gravity
	 */
	protected void AddGravity(Vector3 gravity)
	{
		gravity = ClampGravity(gravity);
		playerBody.position += gravity;
	}

	/**
	 * Limit the speed to the speed max
	 */
	protected void ClampSpeedMax()
	{
		if (velocity.magnitude > speedCap)
			velocity = getVelocity().normalized * speedCap;
	}

	/**
	 * Limit the speed to the speed min
	 */
	protected void ClampSpeedMin()
	{
		if (velocity.magnitude < speedMin)
			velocity = getVelocity().normalized * speedMin;
	}

	/**
	 * Limit the gravity force
	 */
	protected Vector3 ClampGravity(Vector3 gravity)
	{
		return (gravity.magnitude > gravityMax) ? gravity.normalized * gravityMax : gravity;
	}

	/**
	 * Return the current velocity or the direction if the velocity magnitude is equal to 0
	 */
	protected Vector3 getVelocity()
	{
		return velocity.magnitude == 0 ? direction : velocity;
	}

	/** The velocity of the rigid body */
	public Vector3 Velocity => velocity;
	/** The direction of the rigid body */
	public Vector3 Direction => direction;
	/** The position of the player body */
	public Vector3 Position => playerBody.position;
}

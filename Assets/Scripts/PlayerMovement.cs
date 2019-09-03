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

	public bool ignoreGravity = false;

	private Gravity gravity = new Gravity(); // gravity simulator

	private Vector3 velocity; // propulsion force
	private float boostTime = 0f; // duration remains on stopped boost
	private float speedCap; // max speed of the player

	/**
	 * Validate the parameter values (on editor)
	 */
	void OnValidate()
	{
		speedMin = Mathf.Max(speedMin, 0.2f); // if < 0.2, trigger === Vector3.zero
	}

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
		speedCap = speedMax;

		UpdatePosition(null, 1f); // apply start setup
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
			Decelerate(-input.bot);

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
		// Position

		velocity = UpdateVelocity(velocity, playerBody.position,  obstacles, dt); // add gravity + clamp

		Debug.DrawLine(playerBody.position, playerBody.position + velocity * dt * 100); // DEBUG

		playerBody.position += velocity * dt;

		// Rotation

		if (velocity.magnitude > 0)
			playerBody.rotation = Quaternion.Lerp(playerBody.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg), dt);

		// Boost

		if (boostTime > 0)
			speedCap = Mathf.Lerp(speedMax + boostForce, speedMax, 1 - boostTime / boostDecDuration);

		boostTime = boostTime - dt < 0 ? 0 : boostTime - dt;
	}

	/**
	 * Update the velocity with gravity and clamp speed max
	 * Used by PlayerPath to simulate the path
	 */ 
	public Vector3 UpdateVelocity(Vector3 vel, Vector3 pos, GravityBody[] obstacles, float dt)
	{
		if (!ignoreGravity)
			vel += gravity.GetGravity(obstacles, pos) * dt; // add gravity

		if (vel.magnitude > speedCap)
			vel = vel.normalized * speedCap; // clamp

		return vel;
	}

	/**
	 * Increase the speed on the forward direction
	 * Cap the speed to the max
	 */
	private void Accelerate(float intensity) // intensity > 0
	{
		velocity += playerBody.transform.right * speedAcc * intensity;

		if (velocity.magnitude > speedCap)
			velocity = velocity.normalized * speedCap; // clamp
	}

	/**
	 * Decrease the speed on the forward direction
	 * Cap the speed to the min (prevent to stop and backward)
	 */
	private void Decelerate(float intensity) // intensity > 0
	{
		Vector3 force = -velocity * speedDec * intensity;

		if (Vector3.Dot(playerBody.transform.right, velocity + force) > 0) // if the velocity is not opposite to the forward
		{
			velocity += force;

			if (velocity.magnitude < speedMin)
				velocity = velocity.normalized * speedMin; // clamp
		}
	}

	/**
	 * Rotate the direction and the velocity without changes the speed
	 */
	private void Turn(float intensity) // intensity 0 → 1
	{
		Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleSpeed * -intensity));
		playerBody.rotation = rotation * playerBody.rotation;

		float magnitude = velocity.magnitude;
		velocity = rotation * velocity;
		velocity = velocity.normalized * magnitude; // avoid to change the speed
	}

	/**
	 * Apply a boost force on the velocity
	 * Increase the speed max temporally
	 */
	private void Boost()
	{
		velocity += velocity.normalized * boostForce;
		speedCap = speedMax + boostForce;
		boostTime = boostDecDuration;
	}

	/** The velocity of the rigid body */
	public Vector3 Velocity => velocity;
	/** The position of the player body */
	public Vector3 Position => playerBody.position;
}

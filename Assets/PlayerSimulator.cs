using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimulator : PlayerMovement
{
	public void SetVelocity(Vector3 velocity)
	{
		this.velocity = velocity;
	}
}

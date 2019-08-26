using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimulator : PlayerMovement
{
	public void SetSimulatorPosition(Vector3 position)
	{
		playerBody.position = position;
	}
}

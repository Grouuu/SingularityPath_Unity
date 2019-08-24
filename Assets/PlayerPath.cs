using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{
	protected PlayerSimulator simulator = new PlayerSimulator();

	public void UpdatePath(Vector3 playerVelocity, GravityBody[] obstacles, float dt)
	{
		simulator.SetVelocity(playerVelocity);
	}

	public PlayerSimulator Simulator => simulator;
}

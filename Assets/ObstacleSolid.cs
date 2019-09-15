using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSolid : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Player")
			GameController.instance.PlayerHitObstacle(collision.contacts[0]); // return the first contact
	}
}

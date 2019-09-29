using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Obstacle : MonoBehaviour
{

	void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Player")
			GameController.instance.PlayerHitObstacle(collision.contacts[0]); // return the first contact
	}
}

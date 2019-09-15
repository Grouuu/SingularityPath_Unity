using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
	public float mass = 10f;
	public float radius = 5f;

	[HideInInspector]
	public Vector3 position;

	private void Awake() {
		position = transform.position;
	}
}

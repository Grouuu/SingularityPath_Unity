using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
	public float mass = 10f;

	[HideInInspector] public float radius = 5f;
	[HideInInspector] public float radiusGravity = 5f;
	[HideInInspector] public Vector3 position;

	void Start()
	{
		position = transform.position;
		radius = transform.localScale.x / 2;
		radiusGravity = radius + mass * 1000; // TODO : temp
	}

	public void Highlight(bool enable)
	{
		if (enable)
			transform.localScale = new Vector3(2, 2, 2);
		else
			transform.localScale = new Vector3(1, 1, 1);
	}
}

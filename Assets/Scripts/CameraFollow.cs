using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform player;
	public float smoothSpeed = 0.125f;

	private Vector3 camOffset;

	void Start()
	{
		camOffset = transform.position - player.position;
	}

	void Update()
	{
		if (player != null)
			transform.position = player.position + camOffset;
	}
}

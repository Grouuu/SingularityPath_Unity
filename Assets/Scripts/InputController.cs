using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public InputListener[] listeners;

    void Update()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		float top = vertical > 0 ? vertical : 0;
		float right = horizontal > 0 ? horizontal : 0;
		float bot = vertical < 0 ? vertical : 0;
		float left = horizontal < 0 ? horizontal : 0;

		List<KeyCode> pressedKeys = new List<KeyCode>();

		if (Input.GetKey(KeyCode.Space))
			pressedKeys.Add(KeyCode.Space);

		for (int i = 0; i < listeners.Length; i++)
		{
			listeners[i].Input(top, right, bot, left, pressedKeys);
		}
    }
}

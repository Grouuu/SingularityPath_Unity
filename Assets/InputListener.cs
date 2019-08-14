using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
	[HideInInspector] public float top { get; private set; } = 0;
	[HideInInspector] public float right { get; private set; } = 0;
	[HideInInspector] public float bot { get; private set; } = 0;
	[HideInInspector] public float left { get; private set; } = 0;

	[HideInInspector] public bool space { get; private set; } = false;

	public void Input(float top, float right, float bot, float left, List<KeyCode> pressedKeys)
	{
		this.top = top;
		this.right = right;
		this.bot = bot;
		this.left = left;

		space = pressedKeys.IndexOf(KeyCode.Space) != -1;
	}
}

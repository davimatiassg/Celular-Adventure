using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBindings : ScriptableObject 
{
	public KeyCode right, left, up, down, jump, attack, spec, pause;

	public KeyCode CheckKey(string key)
	{
		switch(key)
		{
			case "right":
				return right;
			case "left":
				return left;
			case "up":
				return up;
			case "down":
				return down;
			case "Jump":
				return jump;
			case "Attack":
				return attack;
			case "Spec":
				return spec;
			case "pause":
				return pause;

			default:
				return KeyCode.None;
				break;
		}
	}

	public void SetKey(KeyCode code, string key)
	{
		switch(key)
		{
			case "right":
				right = code;
				break;
			case "left":
				left = code;
				break;
			case "up":
				up = code;
				break;
			case "down":
				down = code;
				break;
			case "Jump":
				jump = code;
				break;
			case "Attack":
				attack = code;
				break;
			case "Spec":
				spec = code;
				break;
			case "pause":
				pause = code;
				break;
			default:
				break;
		}
		Debug.Log("KeySet");
	}


}

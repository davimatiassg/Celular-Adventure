using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
	public static InputManager instance;

	public int Vaxis;

	public int Haxis;

	public KeyBindings keybindings;
	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(instance != this)
		{
			Destroy(this.gameObject);	
		}
	}

	// Update is called once per frame
	public bool GetButton(string key)
	{
		if(Input.GetKey(keybindings.CheckKey(key)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool GetButtonDown(string key)
	{
		if(Input.GetKeyDown(keybindings.CheckKey(key)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool GetButtonUp(string key)
	{
		if(Input.GetKeyUp(keybindings.CheckKey(key)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public float GetAxisRaw(string axis)
	{	
		
		
		if(axis == "Horizontal")
		{	
			float x = 0;
			if(Input.GetKey(keybindings.CheckKey("right")))
			{
				x++;
				Debug.Log(x);
				return x;
			}
			if (Input.GetKey(keybindings.CheckKey("left")))
			{
				x--;
				Debug.Log(x);
				return x;
			}
			
		}
		if(axis == "Vertical")
		{	
			float y = 0;
			if(Input.GetKey(keybindings.CheckKey("up")))
			{
				y++;
				Debug.Log(y*2);
				return y;
			}
			if (Input.GetKey(keybindings.CheckKey("down")))
			{
				y--;
				Debug.Log(y*2);
				return y;
			}
			
		}
		return 0;

	}
}

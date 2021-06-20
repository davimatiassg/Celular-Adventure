using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputMenu : MonoBehaviour
{

	public bool selected = false;

	public string innerName;

	public KeyCode thiscode;

	public Text keytext;

	public Event keyEvent;

	public AudioInterface a;

	void OnEnable()
	{
		GameEvents.StartListening("selectKeyButton", Unselect);

		thiscode = InputManager.instance.keybindings.CheckKey(innerName);
		keytext.text = thiscode.ToString();
		Unselect();
		a = this.gameObject.GetComponent<AudioInterface>();

	}
	void OnDisable()
	{
		GameEvents.StopListening("selectKeyButton", Unselect);
	}
	

	public void Unselect()
	{	
		selected = false;
	}
	public void Select()
	{	
		if(a != null)
		{
			a.PlaySound("click");
			GameEvents.ScreamEvent("selectKeyButton");
			selected = true;
		}

	}

	void OnGUI()
	{
		keyEvent = Event.current;

		if(keyEvent.isKey && selected)
		{	
			Debug.Log("selected");
			GameEvents.ScreamEvent("selectKeyButton");
			thiscode = keyEvent.keyCode;
			SetKeyBinding();
		}
		else if(keyEvent.type == EventType.MouseDown && selected)
		{
			GameEvents.ScreamEvent("selectKeyButton");
		}
	}
	public void SetKeyBinding()
	{
		InputManager.instance.keybindings.SetKey(thiscode, innerName);
		keytext.text = thiscode.ToString();
	}
}

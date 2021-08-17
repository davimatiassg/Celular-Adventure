using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{

	private Transform trs;
	private Vector3 mouse;
	public CameraBehavior cam;
	[SerializeField] private GameObject Pannel;

	public bool Attached = false;

	public bool LvlSelec = true;


	void OnEnable()
	{	
		GameEvents.StartListening("ToogleCursor", ToggleVisible);
	}
	void OnDisable()
	{
		GameEvents.StopListening("ToogleCursor", ToggleVisible);
	}
	void Start()
	{
		trs = this.gameObject.GetComponent<Transform>();
		if(LvlSelec)
		{
			CameraBehavior.target = trs;
			ToggleVisible();
		}

		
		
	}

	void Update()
	{	
		if(LvlSelec)
		{
			if(!Attached && !Pannel.activeSelf)
			{	
				mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
				trs.position = Camera.main.ScreenToWorldPoint(mouse);
			}
			else if(Pannel.activeSelf)
			{
				UnityEngine.Cursor.visible = true;
			}

			cam.enabled = !Attached;
		}
		else
		{
			mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
			trs.position = Camera.main.ScreenToWorldPoint(mouse);
		}


	}

	void ToggleVisible()
	{
		Debug.Log("toogled cur");
		UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
		this.gameObject.GetComponent<SpriteRenderer>().enabled = !UnityEngine.Cursor.visible;
	}
}

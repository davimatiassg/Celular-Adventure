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

	void Start()
	{
		trs = GetComponent<Transform>();
		UnityEngine.Cursor.visible = false;
	}

	void Update()
	{	

		if(!Attached && !Pannel.activeSelf)
		{	
			mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
			UnityEngine.Cursor.visible = false;
			trs.position = Camera.main.ScreenToWorldPoint(mouse);
		}
		else if(Pannel.activeSelf)
		{
			UnityEngine.Cursor.visible = true;
		}

		cam.enabled = !Attached;

	}
}

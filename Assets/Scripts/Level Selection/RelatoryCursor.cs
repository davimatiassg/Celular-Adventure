using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelatoryCursor : MonoBehaviour
{

	private Transform trs;
	private Vector3 mouse;
	public CameraBehavior cam;


	void Start()
	{
		trs = this.gameObject.GetComponent<Transform>();
		CameraBehavior.target = trs;
		ToggleVisible(true);
	
	}

	void Update()
	{	
		mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 70f);
		trs.position = Camera.main.ScreenToWorldPoint(mouse);
	}

	void ToggleVisible(bool state)
	{
		UnityEngine.Cursor.visible = !state;
		this.gameObject.GetComponent<SpriteRenderer>().enabled = state;
	}
}

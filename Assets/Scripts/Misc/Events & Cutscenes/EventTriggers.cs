using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggers : MonoBehaviour {

	private GameObject Player;
	[SerializeField] private string eventname;
	[SerializeField] private GameObject Activable_obj;
	[SerializeField] private Vector3 CameraPosition;


	void FixedUpdate()
	{	
		Player = GameObject.FindWithTag("Player");
		if(Player.transform.position.x > transform.position.x)
		{
			GameEvents.ScreamEvent(eventname);
			if(Activable_obj)
			{
				Activable_obj.SetActive(!Activable_obj.activeSelf);
			}
			if(CameraPosition != Vector3.zero)
			{
				CameraBehavior cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>();
				cam.posx = CameraPosition.x;
				cam.posy = CameraPosition.y;
				cam.camscale = CameraPosition.z;
			}
			
			Destroy(gameObject);

		} 
	}

}

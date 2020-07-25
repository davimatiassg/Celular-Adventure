using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggers : MonoBehaviour {

	public GameObject Player;
	public GameObject Boss;


	void FixedUpdate()
	{	
		Player = GameObject.FindWithTag("Player");
		if(Player.transform.position.x > transform.position.x)
		{
			GameEvents.ScreamEvent("BossAreaEntered");
			Boss.SetActive(true);
			Destroy(gameObject);

		} 
	}
}

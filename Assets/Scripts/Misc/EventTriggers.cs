using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggers : MonoBehaviour {

	public Transform Player;
	public GameObject Boss;


	void FixedUpdate()
	{	
		Player = GameObject.FindWithTag("Player").transform;
		if(Player.position.x > transform.position.x)
		{
			GameEvents.current.BossAreaEnter();
			Boss.SetActive(true);
			Destroy(gameObject);

		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staph3Behavior : MonoBehaviour {



	private Transform Player;
	private Transform trs;
	private float PlayerDistance;
	public float range;
	public float speed;
	private float dir;
	private Vector2 PdMod;
	private Vector2 POriginal;


	// Use this for initialization
	void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		trs = GetComponent<Transform> ();
		POriginal = trs.position;

		
	}
	
	// Update is called once per frame
	void Update () {


		PlayerDistance = Vector2.Distance(Player.position, trs.position);

		dir = PlayerDistance/Mathf.Abs(PlayerDistance);
		


		if(PlayerDistance < range)
		{
			trs.position = Vector2.MoveTowards(trs.position, Player.position, speed* Time.deltaTime);
		}
		else 
		{
			trs.position = Vector2.MoveTowards(trs.position, POriginal, speed* Time.deltaTime);			
		}
			


		

		trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
		
		}
	


	
}

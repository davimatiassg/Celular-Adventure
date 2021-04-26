using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPoint : MonoBehaviour {

	public int lifeheal;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag.Equals("Player"))
		{
			var player = other.gameObject.GetComponent<MasterController>();
			this.gameObject.GetComponent<AudioInterface>().PlaySound("heal");
			player.gainlife(lifeheal);

			this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		}
	}
}

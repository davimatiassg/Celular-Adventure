
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDmgApplyerBehavior : MonoBehaviour {
	public int dmg;
	public bool abyss;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player"))
		{
			var hited = other.gameObject.GetComponent<PlayerBehavior>();
			if(abyss)
			{
				hited.fall(dmg);
			}
			else
			{
				hited.takedamage(dmg);
			}
		}
		else if(other.gameObject.tag.Equals("Hitable") && abyss)
		{
			Destroy(other.gameObject);
		}
	}
}

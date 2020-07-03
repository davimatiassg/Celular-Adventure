using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour {
	private SpriteRenderer spr;
	private Transform trs;
	public int dmg;
	public float length;
	public float yflipct;
	public bool FlipY;

	// Use this for initialization
	void Start () {
		spr = GetComponent<SpriteRenderer> ();
		trs = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		spr.size = new Vector2(spr.size.x, length*2);
		if(FlipY)
		{
			trs.position = new Vector3 (trs.position.x, yflipct, trs.position.z);
			spr.flipY = FlipY;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player"))
		{
			var hited = other.gameObject.GetComponent<PlayerBehavior>();
			hited.takedamage(dmg);
		}

	}
}

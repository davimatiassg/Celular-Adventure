using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehavior : MonoBehaviour {
	public int dmg;
	public float speed;
	private Rigidbody2D rigb;
	public GameObject HitEffect;

	// Use this for initialization
	void Start () {
		rigb = GetComponent<Rigidbody2D> ();
		rigb.velocity = transform.right * speed;
	}

	void Update () {
		rigb.velocity = transform.right * speed;
		if (Mathf.Abs(rigb.velocity.x) <= 0.3f){Destroy(gameObject);}
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player"))
		{
			var hited = other.gameObject.GetComponent<PlayerBehavior>();
			hited.takedamage(dmg);


		}
		Destroy(gameObject);
		Instantiate(HitEffect);
	}
}

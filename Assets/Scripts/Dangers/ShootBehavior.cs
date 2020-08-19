using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehavior : MonoBehaviour {

	//declaração das variáveis da inst
	public int dmg;
	public float speed;
	private Rigidbody2D rigb;
	public GameObject HitEffect;

	public bool enemie = true;

	// Use this for initialization
	void Start () {
		rigb = GetComponent<Rigidbody2D> ();
		rigb.velocity = transform.right * speed;
	}

	void Update () {
		rigb.velocity = transform.right * speed;
		if (Mathf.Abs(rigb.velocity.x) <= 0.3f)
		{
			Destroy(gameObject);
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && enemie)
		{
			var hited = other.gameObject.GetComponent<MasterController>();
			hited.takedamage(dmg);
			Destroy(gameObject);
		}
		else if(other.gameObject.tag.Equals("hitable") && !enemie)
		{	
			var hited = other.gameObject.GetComponent<CombatEnemy>();
			hited.takedamage(dmg, new Vector2(rigb.velocity.x/Mathf.Abs(rigb.velocity.x)*6f, 5f));
			Destroy(gameObject);
			Instantiate(HitEffect);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{	
		if(!other.gameObject.tag.Equals("Player"))
		{
			Instantiate(HitEffect);
			Destroy(this.gameObject);
		}
		
		
	}
}

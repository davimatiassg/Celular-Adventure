using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehavior : MonoBehaviour {

	//declaração das variáveis da inst
	public int dmg;
	public float speed;
	private Rigidbody2D rigb;
	public GameObject HitEffect;
	public Vector2 knockback = new Vector2(0f, 0f);

	public bool enemie = true;

	// Use this for initialization
	void Start () {
		rigb = GetComponent<Rigidbody2D> ();
		rigb.velocity = transform.right * speed;
	}

	void Update () {
		rigb.velocity = transform.right * speed;
		if (!enemie)
		{
			Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), GameObject.FindWithTag("Player").GetComponent<Collider2D>(), true);
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
			Debug.Log(rigb.rotation/45);
			hited.takedamage(dmg, new Vector2(Mathf.Sign(90f - transform.eulerAngles.y)*knockback.x, knockback.y*rigb.rotation/45));

			if(speed != 0f)
			{	
				Destroy(gameObject);

			}
			
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
	public void end()
	{
		Destroy(gameObject);
	}
}

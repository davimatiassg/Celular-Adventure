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

	public bool destroyOnContact = true;

	public bool enemie = true;

	// Use this for initialization
	void Start () 
	{	
		if(this.gameObject.GetComponent<Rigidbody2D>() != null)
		{
			rigb = GetComponent<Rigidbody2D>();
			rigb.velocity = transform.right * speed;
		}
		
		
	}

	void Update () 
	{	
		if(this.gameObject.GetComponent<Rigidbody2D>() != null)
		{
			rigb.velocity = transform.right * speed;
		}
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
			if(destroyOnContact)
			{
				Destroy(gameObject);
			}
			
		}
		else if(other.gameObject.tag.Equals("hitable") && !enemie)
		{	
			var hited = other.gameObject.GetComponent<CombatEnemy>();
			hited.takedamage(dmg, new Vector2(Mathf.Sign(transform.right.x)*knockback.x, knockback.y*transform.right.y));
			Debug.Log(transform.right);

			if(speed != 0f)
			{	
				Destroy(gameObject);

			}
			
			Instantiate(HitEffect);
		}
		else if(other.gameObject.tag.Equals("Boss") && !enemie)
		{
			var hited = other.gameObject.GetComponent<BossCore>();
			if(!hited)
			{
				other.gameObject.GetComponent<HitableParts>();
			}
			hited.takedamage(dmg);

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

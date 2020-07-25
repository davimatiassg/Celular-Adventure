using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEnemy : MonoBehaviour {

	public string name;
	private Rigidbody2D rigb;
	public int life;
	public int attackdmg;
	public float hitstun = 2f;
	private float stun;
	private bool stuned;
	public GameObject hiteffect;
	public ParticleSystem hitparticles;
	private SpriteRenderer HitF;




	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && !stuned)
		{
			var hited = other.gameObject.GetComponent<PlayerBehavior>();
			hited.takedamage(attackdmg);
		}

	}
	public void takedamage(int dmg, Vector2 knockback)
	{	
		if(life > 0)
		{
			life -= dmg;
			stun = hitstun;
			if(life > 0)
			{
				Instantiate(hiteffect, transform.position, transform.rotation);
			}
			else
			{
				HitF = Instantiate(hiteffect, transform.position, transform.rotation).GetComponent<SpriteRenderer>();
				HitF.color = new Color(1f, 0, 0, 1f);
				HitF.gameObject.transform.localScale = new Vector3(14f, 14f, 1f);
			}
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
		}

	}


	public bool stuncheck()
	{
		return stuned;

	}

	void BestiaryAdd()
    {
    	if(GameObject.FindGameObjectWithTag("Bestiary").GetComponent<BestiaryElements>().Bestiary.ContainsKey(name))
    	{
    		GameObject.FindGameObjectWithTag("Bestiary").GetComponent<BestiaryElements>().Bestiary[name] ++;
    	}
    	else
    	{
    		GameObject.FindGameObjectWithTag("Bestiary").GetComponent<BestiaryElements>().Bestiary.Add(name, 1);

    	}
    	
    	BestiaryRemove();
    }
    void BestiaryRemove()
    {
    	BestiaryElements.onKillEnemy -= BestiaryAdd;

    }

	void Update()
	{
		if (life <= 0)
		{
			if(!stuned)
			{
				Destroy(this.gameObject);
				
				BestiaryElements.onKillEnemy += BestiaryAdd;

				GameEvents.ScreamEvent("EnemyKilled");

			}
			else
			{
			HitF = Instantiate(hiteffect, transform.position, transform.rotation).GetComponent<SpriteRenderer>();
			HitF.color = new Color(1f, stun, stun, 1f);
			HitF.gameObject.transform.localScale = new Vector3(1/(stun+0.1f)+6f, 1/(stun+0.1f)+6f, 1f);
			}
		}

		if(stun > 0.0f)
		{
			stun -= Time.deltaTime;
			stuned = true;
		}
		else
		{
			stuned = false;
		}
	}
}

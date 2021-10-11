
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableParts : MonoBehaviour {

	public SpriteRenderer spr;
	public Animator anim;


	public bool isinvencible;
	public bool isdangerous;

	public Collider2D h1, h2, h3, h4;
	public bool hit1, hit2, hit3, hit4;

	public int damage;

	public bool hasmultipleparts;

	public int life;
	public int maxlife;
	public float dmgtime;
	private float blinktime;

	//caso estejamos tratando do objeto contendo a vida do chefe
	public bool isBossCore;
	public Texture hplevel, frame, bottom;

	
	void Start()
	{
		life = maxlife;
		if(isBossCore)
		{
			anim = GetComponent<Animator>();	
		}
		
		spr = GetComponent<SpriteRenderer>();
	}
	void Update() 
	{	
		if(hasmultipleparts)
		{
			h1.enabled = hit1;
			h2.enabled = hit2;
			h3.enabled = hit3;
			h4.enabled = hit4;
		}
		if(blinktime > 0)
		{
			blinktime -= Time.deltaTime;
			if(isBossCore){anim.Play("BlinkDamage");}
			
			spr.color = new Color(Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*10f, 1f));
		}
		else
		{
			if(isBossCore){anim.Play("Idle");}
		}


		if(isdangerous)
		{
			spr.color  = new Color(1f, 1f - Mathf.PingPong(Time.time*2/3, 0.67f) ,1f - Mathf.PingPong(Time.time, 1f));
		}
		else
		{
			spr.color = Color.white;
		}


	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && isdangerous)
		{
			var hited = other.gameObject.GetComponent<MasterController>();
			hited.takedamage(damage);
		}

	}
	public void takedamage(int dmg)
	{

		if(!isinvencible && blinktime <= 0)
		{
			life -= dmg;

			blinktime = dmgtime;
			if (life <= 0)
			{	
				if (!isBossCore)
				{
					Destroy(this.gameObject);
				}
				else
				{
					GameEvents.ScreamEvent("BossDie");
				}
				
			}

		}

	}
	void OnGUI ()
 	{	
 		if(isBossCore)
 		{
 			GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), bottom);
 			if(life > 0)
 			{
 				GUI.DrawTexture (new Rect (Screen.width*0.405f, Screen.height*0.874f, Screen.width*0.587f*life/maxlife, Screen.height*0.117f), hplevel);
 			}
 			
 			GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), frame);
 		}
 		
 	}

}

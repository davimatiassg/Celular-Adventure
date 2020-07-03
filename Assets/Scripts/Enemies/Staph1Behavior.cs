using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Staph1Behavior : MonoBehaviour {



	private Transform Player;
	private Transform trs;
	private Rigidbody2D rigb;
	private Animator anim;
	private Vector3 PlayerDistance;
	public float rangex;
	public float rangey;
	public float speed;
	private float dir;
	private Vector2 PdMod;
	public bool Charge = false;
	private float loadtime;
	public float load;
	private bool alratack = false;
	public bool stuned;
	public bool dash;




	// Use this for initialization
	void Start () {

		Player = GameObject.FindGameObjectWithTag("Player").transform;
		trs = GetComponent<Transform> ();
		rigb = GetComponent<Rigidbody2D> ();
		alratack = false;
		Charge = false;
		loadtime = load;
		anim = GetComponent<Animator> ();


		
	}
	
	// Update is called once per frame
	void Update () {
		dash = (Mathf.Abs(rigb.velocity.x) > speed);
		var combat = GetComponent<CombatEnemy>();
		stuned = combat.stuncheck();
		anim.SetBool("Stunned", stuned);
		anim.SetBool("Dash", dash);
		if(stuned)
		{
			loadtime = load;	
			anim.SetBool("Charge", false);
			Charge = false;
			anim.Play("takedmg");
		}





		PlayerDistance = Player.transform.position - trs.position;

		PdMod = new Vector2 (Mathf.Abs(PlayerDistance.x), PlayerDistance.y);
		if (!Charge){dir = PlayerDistance.x/PdMod.x;}
		


		if(PdMod.x < rangex && PdMod.y < rangey && !stuned)
		{
			if(!Charge){rigb.velocity = new Vector2(speed*dir, rigb.velocity.y); anim.SetBool("Charge", true);}
			if(PdMod.x < rangex/2 && PdMod.y < rangey/4){Attack();}
			else{
				alratack = false; 
				loadtime = load;
				anim.SetBool("Charge", false);
				}
			
		}
		if(Mathf.Abs(rigb.velocity.x) == 0){Charge = false;}
		

		trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*-dir, trs.localScale.y);
		
		}
	

	void Attack(){
			Charge = true;
			if (!alratack)
			{
				loadtime -= Time.deltaTime;
				anim.SetBool("Charge", true);
				if(loadtime > 0.0f){rigb.velocity = new Vector2(0.0f, rigb.velocity.y);}
			}

			if(loadtime <= 0.0f)
			{
				rigb.AddForce(new Vector2(speed*4*dir, 0.0f), ForceMode2D.Impulse);
				anim.SetBool("Charge", false);
				loadtime = load;
				alratack = true;
			}

	}
	
}

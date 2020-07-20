using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Staph2Behavior : MonoBehaviour {



	private Transform Player;
	public Transform ShootLocal;
	private Transform trs;
	private Animator anim;
	private Vector3 PlayerDistance;
	public float range;
	private float dir;
	private Vector2 PdMod;
	public bool Charge = false;
	private float loadtime;
	public float load;
	public float reload;
	private float reloadtime;
	public bool stuned;
	public GameObject shoot;





	// Use this for initialization
	void Start () {

		
		trs = GetComponent<Transform> ();
		loadtime = load;
		reloadtime = 0f;
		anim = GetComponent<Animator> ();


		
	}
	
	// Update is called once per frame
	void Update () {
		Player = GameObject.FindGameObjectWithTag("Player").transform;
		var combat = GetComponent<CombatEnemy>();
		stuned = combat.stuncheck();
		anim.SetBool("Stunned", stuned);

		PlayerDistance = Player.transform.position - trs.position;

		PdMod = new Vector2 (Mathf.Abs(PlayerDistance.x), PlayerDistance.y);
		if (!Charge){dir = PlayerDistance.x/PdMod.x;}

		if(reloadtime > 0f){reloadtime -= Time.deltaTime;}
		


		if(PdMod.x < range && !stuned)
		{
			if(PdMod.x < range && PdMod.y < range/4 && reloadtime <= 0f){Attack();}
			else{
				Charge = false;
				loadtime = load;
				anim.SetBool("Charge", false);
				}
			
		}
		

		trs.rotation = new Quaternion(trs.rotation.x, 90f-(dir*90), 0, trs.rotation.w);
		
		}
	

	void Attack(){
			Charge = true;
			loadtime -= Time.deltaTime;
			anim.SetBool("Charge", true);
			if(stuned){loadtime = load;	anim.SetBool("Charge", false); }
			if(loadtime <= 0.0f)
			{
				anim.SetBool("Charge", false);
				loadtime = load;
				reloadtime = reload;
				Instantiate(shoot, ShootLocal.position, ShootLocal.rotation);

			}

	}
	
}

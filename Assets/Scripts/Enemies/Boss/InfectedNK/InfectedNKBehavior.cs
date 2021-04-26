using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class InfectedNKBehavior : MonoBehaviour
{	
    public ParticleSystem Particles;

	public float speedFault = 1;

	public Color R_Clone_Color;

	public Color L_Clone_Color;

	public bool isMain = false;

	public GameObject MainClones;

	public BossCore bossCore;

	[SerializeField] private GameObject Axe;

	private TrailRenderer trail;

	[SerializeField] private float activeTime;

	[SerializeField] private Animator anim;

	[SerializeField] private Transform trs;

	[SerializeField] private MasterController player;

	[SerializeField] private float timebtwattacks;

	public bool isIdle, isEntry, isPinch, isAtk;

	[SerializeField] private Vector2 pPosition = Vector2.zero;

	[SerializeField] private Vector2 destination = Vector2.zero;

	[SerializeField] private float pd;

	[SerializeField] private Vector2 startpos;

	[SerializeField] private bool atkAfterTP = false;

	[SerializeField] private bool Whatatk; //verdadeiro para dash, falso para jogar machado

    void OnEnable()
	{
		GameEvents.StartListening("BossDie", Die);
        GameEvents.StartListening("BossDamaged", PlayParticles);
	}
	void OnDisable()
	{
		GameEvents.StopListening("BossDie", Die);
        GameEvents.StopListening("BossDamaged", PlayParticles);
	}

    void Start()
    {	

    	trail = this.gameObject.GetComponent<TrailRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>();
        trs = GetComponent<Transform>();
        startpos = transform.position;
        isIdle = true;
        isAtk = false;
        pPosition = Vector2.zero;
        bossCore = trs.parent.gameObject.GetComponent<BossCore>();
        this.GetComponent<SpriteRenderer>().material.color = this.gameObject.GetComponent<SpriteRenderer>().color;
        Particles.startColor = this.gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {	
    	if(!isEntry)
    	{
	    	activeTime += Time.deltaTime;
	    	if(bossCore.life <= bossCore.maxlife*2/5 && isMain && !isPinch)
	    	{	
	    		isPinch = true;
	    		Debug.Log(this.gameObject);
	    		SummonClones();	
	    	}
	    	int i = 1;
	        pd = PlayerXDistanceX(out pPosition);

	        if(isIdle)
	        {	

	        	destination = new Vector2(startpos.x + (Mathf.Sin(activeTime*2.5f)*10f)/speedFault, startpos.y + (Mathf.Sin(activeTime*5)*3 -2.5f)/speedFault);
	        	if(!isAtk)
	        	{
	        		trs.position = destination;
		        	if(Mathf.Abs(pd) > 7.5f)
		        	{
		        		i = Mathf.RoundToInt(Mathf.Sign(pd)) + 1;
		        	}
		        	anim.Play("Idle" + i);
		        	if(Mathf.Repeat(Time.time, timebtwattacks) < 0.03f)
		        	{
		        		Attack();
	        		}
	        	}
	        }    		
    	}

    }




    public void Attack()
    {	
    	isAtk = true;
    	Whatatk = Convert.ToBoolean(UnityEngine.Random.Range(0, 2)); //verdadeiro para dash, falso para jogar machado
    	bool willTp = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));

    	if(willTp)
    	{	
    		atkAfterTP = true;
    		Teleport();
    		isIdle = true;
    	}
    	else
    	{	
    		atkAfterTP = false;
			Atkcheck();
    	}
    	
    }


    public void Atkcheck()
    {
    	if(Whatatk)
	    {	
	    	isIdle = false;
	    	Dash();	
	    }
	    else
	    {
	    	PrepareAxe();
	    }
    }


    public void Dash()
    {	
    	
    	if(Mathf.Sign(pd) > 0)
	    {	
	    	if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash1"))
    		{
		    	anim.Play("Dash0");
		    	trs.eulerAngles = new Vector3(0, 0, -Vector2.Angle((new Vector2(trs.position.x, trs.position.y) - pPosition).normalized, Vector2.left));
		    }
	    }
	    else
	    {	
	    	if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash0"))
    		{
	    		anim.Play("Dash1");
	    		trs.eulerAngles = new Vector3(0, 180, -Vector2.Angle((new Vector2(trs.position.x, trs.position.y) - pPosition).normalized, Vector2.right));
	    		trs.localScale = Vector3.left + Vector3.up + Vector3.forward;
	    	}
	    }

    }

    public void StopDash()
    {	
    	Vector2 stp = trs.position;

    	Vector2 finalpos = trs.right.normalized*20 + trs.position;
    	trs.position = finalpos;
    	RaycastHit2D hit = Physics2D.Raycast(stp, trs.right.normalized, 20);
    	if(hit)
    	{	
    		Debug.Log(hit.transform.gameObject);
    		if(hit.transform.gameObject.tag.Equals("Player"))
    		{	
    			Debug.Log("hit");
    			var hited = hit.transform.gameObject.GetComponent<MasterController>();
				if(hited)
				{
					hited.takedamage(bossCore.damage);
				}
    		}
    	}
    	trs.localScale = new Vector3(1, 1, 1);
    	trs.eulerAngles = Vector3.zero;
    	isIdle = false;
    	activeTime = 0.03f;
    	atkAfterTP = false;

    	

    }

    public void TPBack()
    {
    	Teleport(new Vector2(startpos.x + Mathf.Sin(0.03f*2.5f)*10f, startpos.y + Mathf.Sin(0.03f*5)*2 -1.5f));
    }
    public void Teleport()
    {	
    	isIdle = false;
    	
    	anim.Play("Teleport");
    	destination = startpos + UnityEngine.Random.insideUnitCircle*4.5f;
    }
    public void Teleport(Vector2 dest)
    {	
    	isIdle = false;
    	anim.Play("Teleport");
    	destination = dest;
    }

    public void InstanteTranslocate()
    {	
    	trail.Clear();
    	activeTime = 0.03f;
    	trs.position = destination;
    	trail.Clear();
    }


    public void CheckTeleportAtk()
    {	
    	
    	if(atkAfterTP)
    	{	
    		isIdle = false;
			Atkcheck();
			isAtk = true;

    	}
    	else
    	{	
    		isAtk = false;
    		isIdle = true;
    		activeTime = 0.03f;
    	}	
    }


    public void PrepareAxe()
    {
    	anim.Play("Trow");
    }

    public void TrowAxe()
    {
    	AxeBehavior axe = Instantiate(Axe, trs.position, trs.rotation).GetComponent<AxeBehavior>();
    	axe.Owner = this.gameObject;
    	axe.bossPosition = trs.position;
    	axe.target = pPosition;
    	anim.speed = 0;
    	axe.GetComponent<ShootBehavior>().dmg = bossCore.damage;
    }

    public void CatchAxe()
    {	
    	anim.speed = 1;
    	isIdle = false;
    	activeTime = 0.03f;
    	atkAfterTP = false;
    }


    private float PlayerXDistanceX(out Vector2 pos)
    {	
    	pos = player.gameObject.transform.position;
    	float distance = player.gameObject.transform.position.x - trs.position.x;
    	return distance;
    }

    public void SummonClones()
    {	
    	timebtwattacks = timebtwattacks*3/2;
    	isPinch = true;
    	GameObject RightClone = Instantiate(MainClones, startpos + (Vector2.right*10 + Vector2.down*4), new Quaternion(0,0,0,0), bossCore.gameObject.transform);
    	InfectedNKBehavior R_code = RightClone.GetComponent<InfectedNKBehavior>();
    	R_code.isPinch = true;
    	R_code.isEntry = true;
        R_code.isIdle = false;
    	RightClone.GetComponent<Animator>().Play("Spawn");
    	R_code.startpos = RightClone.transform.position;
    	RightClone.GetComponent<SpriteRenderer>().material.color = R_Clone_Color;
    	RightClone.GetComponent<TrailRenderer>().startColor = R_Clone_Color;
    	RightClone.GetComponent<TrailRenderer>().endColor = R_Clone_Color;
        RightClone.GetComponent<SpriteRenderer>().color = R_Clone_Color;
        R_code.Particles.startColor = R_Clone_Color;



    	GameObject LeftClone = Instantiate(MainClones, startpos + (Vector2.left*10 + Vector2.down*4), new Quaternion(0,0,0,0), bossCore.gameObject.transform);
    	InfectedNKBehavior L_code = LeftClone.GetComponent<InfectedNKBehavior>();
    	L_code.isPinch = true;
    	L_code.isEntry = true;
        L_code.isIdle = false;
    	LeftClone.GetComponent<Animator>().Play("Spawn");
    	L_code.startpos = LeftClone.transform.position;
    	LeftClone.GetComponent<SpriteRenderer>().material.color = L_Clone_Color;
    	LeftClone.GetComponent<TrailRenderer>().startColor = L_Clone_Color;
    	LeftClone.GetComponent<TrailRenderer>().endColor = L_Clone_Color;
        LeftClone.GetComponent<SpriteRenderer>().color = L_Clone_Color;
        L_code.Particles.startColor = L_Clone_Color;
    }


	private void Die()
	{	
		anim.Play("Die");
		player.TogglePlayable(false);
        this.isEntry = true;
        KillClones();
	}

	private void Dead()
	{	
		this.gameObject.SetActive(false);
		GameEvents.ScreamEvent("BossDead");
		Destroy(this.gameObject);

	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && bossCore.isdangerous)
		{
			var hited = other.gameObject.GetComponent<MasterController>();
			hited.takedamage(bossCore.damage);
		}

	}

	public void ToggleLifebar(int state)
	{	
		if(state > 0)
		{
			bossCore.showlifebar = true;
		}
		else
		{
			bossCore.showlifebar = false;
		}
		
	}
	
	public void TransferCorePositionx(float pox)
 	{
 		bossCore.trs.position = new Vector2(pox, bossCore.trs.position.y);
 	}
	public void TransferCorePositiony(float poy)
 	{
 		bossCore.trs.position = new Vector2(bossCore.trs.position.x, poy);
 	}

 	public void StartAttack()
 	{
 		this.isEntry = false;
 	}

    public void PlayParticles()
    {
        Particles.Play();
    }

    public void KillClones()
    {
        if(!this.isMain)
        {   
            anim.Play("KillClones");
        }
    }

    public void disapear()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        anim.speed = 0;
    }
}

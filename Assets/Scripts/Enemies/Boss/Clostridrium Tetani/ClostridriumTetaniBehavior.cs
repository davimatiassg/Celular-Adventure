using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClostridriumTetaniBehavior : MonoBehaviour {

	private Animator anim;

	public GameObject P;

	private MasterController player;

	private float timebtwattacks;

	public bool isIdle, isEntry, isPinch;

	private List<string> atks = new List<string>{"atk1", "atk2", "atk3"};

	public HitableParts Core;

	private bool paralized;

	private float starty;
	public GameObject FinalScreen;

	void OnEnable()
	{
		GameEvents.StartListening("BossDie", Die);
	}
	void OnDisable()
	{
		GameEvents.StopListening("BossDie", Die);
	}

	void Awake () 
	{
		
		gameObject.SetActive(false);
		timebtwattacks = 4f;

	}

	void Start()
	{
		anim = GetComponent<Animator> ();
		player = GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController> ();
		starty = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		player = GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController> ();
			if(Core.life <= Core.maxlife*2/5 && Core.life > 0)
			{
				isPinch = true;
			}
			else
			{
				isPinch = false;
			}
			isIdle = anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
			if(isIdle || anim.GetCurrentAnimatorStateInfo(0).IsTag("Pinch"))
			{
				timebtwattacks -= Time.deltaTime;
				if(timebtwattacks <= 0)
				{	
					if (isPinch)
					{
						timebtwattacks = 2f;
						anim.speed = 1.7f;
					}
					else
					{
						timebtwattacks = 4f;
					}

					int atkchosen = Random.Range(0, atks.Count);
					Debug.Log(atkchosen, gameObject);
					anim.SetTrigger(atks[atkchosen]);
					
				}
			}
			anim.SetBool("IsPinch",isPinch);
	}

	private void Die()
	{	
		
		anim.SetTrigger("Die");
		player.TogglePlayable(false);	
	}

	private void Dead()
	{
		FinalScreen.SetActive(true);
		Time.timeScale = 0.0f;
		anim.speed = 0f;

	}



}

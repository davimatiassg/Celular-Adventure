using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTaeniaBehavior : MonoBehaviour
{	
    [Header("Detalhes do Boss")]

    [Tooltip("Identificador")]
    [SerializeField] public int enemyID; 

    [Tooltip("Nome científico")]
    [SerializeField] public string enemyName;

    [Tooltip("Local de Encontro")]
    [SerializeField] public string encounterLocal; 

    [TextArea]
    [Tooltip("Comportamento In-Game")]
    [SerializeField] public string enemyBehavior;

    [TextArea]
    [Tooltip("Informações científicas")]
    [SerializeField] public string realInfo; 

    [SerializeField] public Sprite inGameImg;

    [SerializeField] public Sprite realImg;

    [Header("Atributos do Inimigo")]

	[SerializeField] private GameObject Boulders;
	[SerializeField] private GameObject Glues;
	[SerializeField] private ParticleSystem Fire;

	[SerializeField] private int FireDamage;

	private Transform Player;
	public float atkCD;
    private float Cd;

    private GameObject bitedColun;

    private Vector3 originalposition;


    private Transform trs;	
    private Animator anim;
    private float life;
    private Vector3 destination = Vector3.zero;
    [SerializeField] private float speed = 50f;
	[SerializeField] private List<string> Attacks = new List<string>{"Bite","GlueSpit", "RockTrow", "Flame"};

    // Start is called before the first frame update
    void Start()
    {	
    	Fire = Fire.GetComponent<ParticleSystem>();
    	Fire.Stop();
        Player = GameObject.FindWithTag("Player").gameObject.GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
        anim = this.gameObject.GetComponent<Animator>();
        destination = new Vector3(724, -56);
        trs.position = new Vector3(724, -90);
        originalposition = destination;
        anim.Play("EntryAnimation");
    }

    // Update is called once per frame
    void Update()
    {	
    	if(this.gameObject.GetComponent<BossCore>().life <= 0)
    	{
    		Fire.Stop();
    		anim.Play("Die");
            Die();
    	}
    	if(Fire.emission.rateOverTimeMultiplier > 18.0f && Fire.isPlaying)
    	{		
    		Burner();
    	}
    	if(destination != trs.position)
    	{	

    		trs.position = Vector3.MoveTowards(trs.position, destination, Time.deltaTime*speed);
    	}
        if(Cd < atkCD)
        {	
        	if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
        	{
        		Cd += Time.deltaTime;
        	}
        	
        }
        else
        {	
        	Cd = 0;

        	anim.Play(Attacks[Mathf.RoundToInt(Random.Range(0, Attacks.Count))]);
        }
    }

    public void Spawn_boulders()
    {
    	for (int i = 0; i < 3; i++ )
        {   
            Vector3 spawnlocal = new Vector3(Random.Range(688f, 760f), -9, 0f); 
            if(i == 0)
            {
                spawnlocal = new Vector3(Player.position.x, -9, 0f);
            }
            Instantiate(Boulders, spawnlocal, new Quaternion(0,0,0,0));
        }
    }
    public void SpitGlue()
    {
    	for (int i = 0; i < 5; i++ )
        {   
            Vector3 spawnlocal = new Vector3(688+12*Mathf.RoundToInt(Random.Range(0f, 7f)), 0, 0f); 
            if(i == 3)
            {
                spawnlocal = new Vector3(Player.position.x, 0, 0f);
            }
            var glue = Instantiate(Glues, spawnlocal, new Quaternion(0,0,0,0));
        }
    }

    public void Positionate(float P_Y)
    {
    	int startposition = Mathf.RoundToInt(Mathf.Sign(Random.Range(-1, 1)));
    	speed = 25;
    	destination = new Vector3(Mathf.RoundToInt((Player.position.x - 688)/12)*12 + 688, -66.6f + P_Y, 0f);

    }
    public void OriginReturn()
    {
    	destination = originalposition;
    }
    public void Inflamate()
    {	
    	if(Fire.isPlaying)
    	{
    		Fire.Stop();
    	}
    	else
    	{
    		Fire.Play();
    	}
    }
    public void BurstFire(float spd)
    {	
    	speed = spd;
    	int dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - (trs.position.x -3.5f)));
    	destination = new Vector3(destination.x + (30*dir), destination.y, 0f);
    	if(destination.x > 760)
    	{
    		destination = new Vector3(760, destination.y, 0f);
    	}
    	else if(destination.x < 696)
    	{
    		destination = new Vector3(696, destination.y, 0f);
    	}
    }


    public void Bite()
    {
    	bitedColun = Physics2D.Raycast(trs.position, Vector2.down, 50f).collider.gameObject;
    	if(bitedColun != null)
    	{
    		if(bitedColun.tag.Equals("Player"))
    		{
    			bitedColun.GetComponent<MasterController>().takedamage(20);
    		}
    	}
    	
    }
    public void Pull(float Intesity)
    {
    	if(bitedColun != null)
    	{
    		if(bitedColun.tag.Equals("Scenary"))
    		{
    			bitedColun.GetComponent<BossMovableTerrain>().GetPulled(Intesity);
    		}
    	}
    }
    public void Drop()
    {
    	if(bitedColun != null)
    	{
    		bitedColun = null;
    	}
    }
    public void Burner()
    {	

    	var FireHit = Physics2D.Raycast(Fire.gameObject.GetComponent<Transform>().position + Vector3.down, Vector3.down, 50f).collider.gameObject;
    	Debug.DrawLine(Fire.gameObject.GetComponent<Transform>().position + Vector3.down, Fire.gameObject.GetComponent<Transform>().position + Vector3.down*5, Color.red);
    	if(FireHit != null)
    	{
    		if(FireHit.tag.Equals("Player"))
    		{
    			FireHit.GetComponent<MasterController>().takedamage(FireDamage);
    		}
    	}
    }
    public void Die()
    {	
    	Fire.Play();
    	destination += Vector3.down*50;
    	speed = 12;
        CardIndex enemyCard = new CardIndex(enemyID, realImg, inGameImg, encounterLocal, enemyBehavior, enemyName, realInfo);
        BestiaryElements.AddCardEnemy(enemyCard);
        PontuationCounter.AddScore(3000);
    }
    public void Dead()
    {
    	GameEvents.ScreamEvent("FinalBossIsDead");
        GameEvents.ScreamEvent("BossDead");
        Debug.Log("BossDead");
    	Destroy(this.gameObject);
    }
}

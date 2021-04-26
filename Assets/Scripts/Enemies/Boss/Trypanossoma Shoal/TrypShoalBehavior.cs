using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrypShoalBehavior : MonoBehaviour
{	
    [SerializeField] private BossCore core;
	[SerializeField] private float atkCoolDown;
	[SerializeField] private float cd;
	private Animator anim;
	private Transform trs;
	private AnimatorStateInfo currentAnimation;
	[SerializeField] private List<GameObject> piranhas = new List<GameObject>();
	public Vector2 destination;
	private Vector2 StartPos;
	[SerializeField] private float mainSpeed = 45;
	[SerializeField] private float range;
	[SerializeField] private float speed;

	[SerializeField] private GameObject LazerBeam;

    [SerializeField] private ParticleSystem particles;


	private Transform Player;
    // Start is called before the first frame update
    void OnEnable()
    {
        GameEvents.StartListening("BossDie", Die);
    }
    void OnDisable()
    {
        GameEvents.StopListening("BossDie", Die);
    }
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        trs = this.gameObject.GetComponent<Transform>();
        StartPos = trs.position;
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        speed = mainSpeed;
        core = this.gameObject.GetComponent<BossCore>();
    }

    // Update is called once per frame
    void Update()
    {	
    	currentAnimation = anim.GetCurrentAnimatorStateInfo(0);
        
        if(!currentAnimation.IsTag("Cutscene") && core.life > 0)
        {
            if(currentAnimation.IsTag("Idle"))
            {   
                destination = StartPos + (range*Mathf.Sin(Time.time))*Vector2.right;
                if(cd > 0)
                {
                    cd -= Time.deltaTime;
                }
                else
                {
                    Attack();
                }
            }
            else if(currentAnimation.IsTag("atk"))
            {
                
                if(currentAnimation.IsName("atk1"))
                {
                    destination = StartPos + (range*Mathf.Sin(Time.time))*Vector2.right + Vector2.up*10;
                }
            }
            trs.position = Vector2.MoveTowards(trs.position, destination, speed*Time.deltaTime);
        }
        else if(!currentAnimation.IsName("Die"))
        {
            Die();
        }

    }

    public void Attack()
    {
    	anim.Play("atk"+(Random.Range(0, 3)+1));
    }

    public void ReturnIdle()
    {
    	anim.Play("Idle");
    	cd = atkCoolDown;
    	trs.localScale = Mathf.Abs(trs.localScale.x)*Vector2.one;
    	speed = mainSpeed;
    }

    private float FindOpositeSide()
    {
    	return Mathf.Sign(StartPos.x-Player.position.x);
    }

    public void AxeAttack()
    {	
    	float u = FindOpositeSide();
    	destination = StartPos + 10*Vector2.up + 15*Vector2.right*u;
    	trs.localScale = Vector2.up*Mathf.Abs(trs.localScale.x)*-u + Vector2.right*Mathf.Abs(trs.localScale.x)*-u;

    }
    public void TornadoAttack()
    {
    	speed = speed*3/2;
    }
    public void LazerAttack()
    {
    	float u = FindOpositeSide();
    	destination = StartPos + 11.5f*Vector2.up + 15*Vector2.right*u;
    	trs.localScale = Vector2.up*Mathf.Abs(trs.localScale.x) + Vector2.right*Mathf.Abs(trs.localScale.x)*-u;
    }
    public void SpawnLazer()
    {
    	GameObject lazer = Instantiate(LazerBeam, piranhas[6].GetComponent<Transform>().position, trs.rotation);
    	float x = -Mathf.Sign(trs.localScale.x);
    	Debug.Log(x);
    	lazer.GetComponent<Transform>().eulerAngles = Vector2.up*(90 + x*90); 
    }

    public void Die()
    {   
        particles.Stop();
        GameObject.FindWithTag("Player").GetComponent<MasterController>().TogglePlayable(false);
        anim.Play("Die");

    }

    public void Dead()
    {      
        GameEvents.ScreamEvent("BossDead");
        Destroy(this.gameObject);
        
    }


}

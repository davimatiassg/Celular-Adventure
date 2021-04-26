using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{	
    private Transform Player;
	private Transform trs;
	private Animator anim;
	private Rigidbody2D rigb;

	private Transform ptrs;

	[SerializeField] private float speed;

	[SerializeField] private float range;
	[SerializeField] private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();	
		trs = this.gameObject.GetComponent<Transform>();
		anim = GetComponent<Animator>(); 
		rigb = GetComponent<Rigidbody2D>();
		ptrs = particles.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        int dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - trs.position.x));
        float hdif = Mathf.Abs(Player.position.y - trs.position.y);
        float pdistance = Vector2.Distance(trs.position, Player.position);
        trs.localScale = new Vector2 (-Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
        ptrs.localScale = new Vector2 (-dir, 1);
        if(pdistance <= 3 && hdif < 3)
        {
        	atk();
        }
        else if(pdistance < range && hdif < 3)
        {
        	walk(dir);
        }
        else
        {
        	idle();
        }
    }
    void atk()
    {
    	anim.Play("attack");
    	rigb.velocity = new Vector2(0, rigb.velocity.y);
    }
    void walk(int dir)
    {	
    	if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
    	{
    		anim.Play("walk");
    		rigb.velocity = Vector2.right*dir*speed + new Vector2(0, rigb.velocity.y);
    	}	
    }
    void idle()
    {	
    	if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck())
    	{
    		anim.Play("Idle");
    		rigb.velocity = new Vector2(0, rigb.velocity.y);
    	}
    }

    public void PlayParticles()
    {
    	particles.Play();
    }
    public void StopParticles()
    {
    	particles.Stop();
    }
    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(this.transform.position + Vector3.left*Mathf.Sign(this.transform.localScale.x)*range/2, new Vector3(range, 3, 0));
	}
}

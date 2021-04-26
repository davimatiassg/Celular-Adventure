using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyTaeniaBehavior : MonoBehaviour
{
    private Rigidbody2D rigb;
    private RaycastHit2D hitb;

   	[SerializeField] private Transform trs;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spr;
	[SerializeField] private LayerMask solid;
	
	[SerializeField] private Transform Player;

	[SerializeField] private Vector2 localupside = Vector2.up;
	[SerializeField] private Vector2 dir = new Vector2(1, 1);

	private float wtime = 3f;
	private bool llwalk; 



	[SerializeField] public float atk_delay;
	public float speed;
	public float range;



    void Start()
    {	
    	Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rigb = this.gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {	
    	if(wtime <= 0f)
    	{
    		wtime = 3f;
    		llwalk = (Random.value > 0.5f);

    	}
    	else
    	{
    		wtime -= Time.deltaTime;
    		
    	}
    	
       	rigb.velocity -= localupside*1f;
    	

        if(Mathf.Abs(Player.position.x - trs.position.x) <= range && Mathf.Abs(Player.position.y - trs.position.y) <= range && spr.isVisible)
        {	
        	if(!Physics2D.Linecast(Player.position, trs.position, solid) )
    		{
    			dir = new Vector2(Mathf.Sign(trs.position.x - Player.position.x), Mathf.Sign(Player.position.y - trs.position.y));
    		}
    		else
    		{
    			dir = new Vector2(Mathf.Sign(trs.position.x - Player.position.x), 1);
    		}
        	trs.localScale = new Vector3(Mathf.Abs(trs.localScale.x)*dir.x, trs.localScale.y, 0f);

        	if(atk_delay <= 0)
        	{
       			if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
       			{
       				anim.Play("attack");
       			}
        	}
        	else if(llwalk == false)
        	{	
        		atk_delay -= Time.deltaTime;
        	  	anim.Play("Idle");
        		rigb.velocity = Vector2.zero;
        	}
       		else
        	{	
        		atk_delay -= Time.deltaTime;
        		anim.Play("walk");
        		if(Mathf.Abs(rigb.velocity.x) < speed && Mathf.Abs(rigb.velocity.y) < speed)
        		{
        			rigb.velocity += Vector2.Scale(Vector2.Perpendicular(localupside), dir)*speed;
        		}
       		}        	
        }
        else
        {
        	anim.Play("Idle");
        	rigb.velocity = Vector2.zero;
        }
       
   	}


    void OnCollisionExit2D(Collision2D col)
    {
    	if(col.gameObject.tag == "Scenary")
    	{	
    		rigb.velocity -= localupside*5f;
    		//localupside = Vector2.up;
    		//trs.eulerAngles = new Vector3(0, 0, 0);
    	}
    }
  	public void OnCollisionStay2D(Collision2D col)
    {
    	if(col.gameObject.tag == "Scenary")
    	{
    		localupside = col.GetContact(col.contactCount-1).normal;
    		trs.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(col.GetContact(col.contactCount-1).normal, Vector2.up));
    	}
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, range);
	}
}

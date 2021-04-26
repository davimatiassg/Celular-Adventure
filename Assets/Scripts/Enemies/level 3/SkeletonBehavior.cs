using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehavior : MonoBehaviour
{	
   	private Transform Player;
	private Transform trs;
	private Animator anim;
	private Rigidbody2D rigb;
	[SerializeField] private Transform ShootLocal;

	private bool isGrounded;
	private int dir;
	[SerializeField] private float jumpforce;

	[SerializeField] private float range;
	public GameObject shoot;
    // Start is called before the first frame update
    void Start()
    {
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();	
		trs = this.gameObject.GetComponent<Transform>();
		anim = GetComponent<Animator>(); 
		rigb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - trs.position.x));
        float hdif = Mathf.Abs(Player.position.y - trs.position.y);
        float pdistance = Vector2.Distance(trs.position, Player.position);
        trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
        if(isGrounded)
        {	

        	if(Mathf.Abs(pdistance) < range && hdif < 5)
	        {	

		        if(pdistance >= range/4)
		        {
		        	atk();
		        }
		        else
		        {
		        	jump();
		        }
	        }
	        else
	        {
	        	idle();
	        }
        }
       
    }
    void atk()
    {
    	if(isGrounded)
    	{
	     	anim.Play("attack");
	    	rigb.velocity = new Vector2(0, rigb.velocity.y);   		
    	}

    }
    void jump()
    {	
        AudioInterface a = this.gameObject.GetComponent<AudioInterface>();
    	if((!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck()) && isGrounded)
    	{	
    		rigb.AddForce(new Vector2(-dir*jumpforce/2, jumpforce), ForceMode2D.Impulse);
    		isGrounded = false;
            a.PlaySound("jump");
    	}
    }
    void idle()
    {	
    	if((!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck()) && isGrounded)
    	{	
    		anim.Play("Idle");
    		rigb.velocity = new Vector2(0, rigb.velocity.y);
    	}
    }

    void shot()
    {
    	GameObject projectile = Instantiate(shoot, ShootLocal.position, ShootLocal.rotation);
    	projectile.transform.eulerAngles = new Vector3(0 , 90  -dir*90 , 0);
    }

    void OnCollisionStay2D(Collision2D col)
    {	
    	if(this.gameObject.GetComponent<SpriteRenderer>().isVisible)
    	{
	    	ContactPoint2D[] contacts = new ContactPoint2D[3];
	        if(col.GetContacts(contacts) > 0)
	        {
	            foreach(ContactPoint2D cn in contacts)
	            {
	            	if(cn.normal.y >= 0.5)
	            	{
	            		isGrounded = true;
	            	}
	            }
	        }    		
    	}

    }
    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(this.transform.position + Vector3.right*Mathf.Sign(this.transform.localScale.x)*range/2, new Vector3(range, 5, 0));
	}
}

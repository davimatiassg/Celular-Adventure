using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimingTrypBehavior : MonoBehaviour
{	
	private float stunedTime;

	private SwimmingTypes swCode;

	private CombatEnemy mainCode;

	private Transform Player; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim;

	private SpriteRenderer spr;

	[SerializeField] private Vector3 stPos;

    [SerializeField] private float speed;

    [SerializeField] private float range;

    [SerializeField] private bool insideWater = false;

	[SerializeField] private float swimdistance;

	[SerializeField] private bool jumperEnemy;

	[SerializeField] private bool grabed;

    void Start()
    {	

        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		spr = GetComponent<SpriteRenderer> ();

		mainCode = GetComponent<CombatEnemy>();

		swCode = GetComponent<SwimmingTypes>();

		stPos = trs.position;
    }	
    // Update is called once per frame
    void FixedUpdate()
    {	

    	if(!mainCode.stuncheck())
    	{	
    		stunedTime = 0;
    		if(!grabed)
    		{	
    			spr.sortingOrder = 5;
	    		insideWater = swCode.Getinwater();
	    		if(!insideWater)
	    		{	

	    			rigb.isKinematic = false;
	    			rigb.gravityScale = 40;
		      		anim.SetFloat("Rspeed", -rigb.velocity.x);
		    		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swim"))
		        	{
		        		anim.Play("RollEntry");
		        	}
		        	else if(anim.GetCurrentAnimatorStateInfo(0).IsName("stun"))
		        	{
		        		anim.Play("Roll");
		        	}
		    		float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
		    		float speedFactor = (1 - Mathf.Abs(rigb.velocity.x)/(speed*10));
		        	if(PlayerDistance <= range)
		        	{
		        		rigb.velocity += Vector2.right * Mathf.Sign(Player.transform.position.x -trs.position.x) * speed * speedFactor;


		        	}
		        	else if(Mathf.Abs(rigb.velocity.x) > 0)
		        	{
		        		rigb.velocity = Vector2.zero + Vector2.up*rigb.velocity.y;
		        	}  
		        	trs.eulerAngles -= Vector3.forward*rigb.velocity.x*2 + Vector3.up*trs.localEulerAngles.y;		
	    		}
	    		else
	    		{	
	    			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("stun"))
	    			{	
	    				trs.localEulerAngles -= Vector3.forward*trs.localEulerAngles.z;
						rigb.isKinematic = true;
		    			rigb.gravityScale = 0;
		    			float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
		    			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swim"))
		    			{	
		    				rigb.velocity = Vector2.zero;
		    				anim.Play("swim");
		    			}
		    			
		    			Vector3 frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*swimdistance, Time.fixedDeltaTime*speed*4);
			        	trs.position = frameTranslocation;
			        	if(stPos - trs.right*swimdistance - trs.position == Vector3.zero)
			        	{	
			        		
			        		trs.localEulerAngles += Vector3.up*180;

			        		trs.position = Vector2.MoveTowards(trs.position, stPos + trs.right*swimdistance, Time.fixedDeltaTime*speed*4);
			        	}

			        	if(PlayerDistance <= range*2/3 && jumperEnemy && Player.position.y > trs.position.y)
			        	{	
			        		trs.localEulerAngles -= Vector3.forward*90;
			        		rigb.velocity = Vector2.up * 30;
			        	}
	    			}
	    			
		        }
		    
		    }
		    else
		    {	

		    	rigb.isKinematic = true;
		    	trs.position = Player.position + Vector3.down;
		    	anim.Play("Grab");
		    	spr.sortingOrder = 55;
		    	if(InputManager.instance.GetButtonDown("spec"))
		    	{	
		    		spr.sortingOrder = 5;
		    		grabed = false;
		    		Player.gameObject.GetComponent<MacroBehavior>().hand.GetComponent<MacroHand>().SearchEnemies(this.gameObject);
		    	}

		    }
    	}
    	else
    	{	
    		rigb.isKinematic = false;
    		
    		
    		
    		if(stunedTime > 1.5f)
    		{
    			mainCode.SetStuned(0);
    			if(Player.gameObject.GetComponent<MacroBehavior>().grabed)
    			{
    				Player.gameObject.GetComponent<MacroBehavior>().ToggleGrabOn();
    				grabed = true; 
    			}

    		} 
    		else
    		{	
    			trs.eulerAngles -= trs.eulerAngles;
    			stunedTime += Time.fixedDeltaTime;
    			
    			grabed = false;
    			anim.Play("stun");
    		}
    	}
    }

    public void DamageDeal()
    {
		Player.gameObject.GetComponent<MasterController>().takedamage(mainCode.attackdmg);//vai receber a classe de controle geral, que tem em todos os jogadores
    }	



    void OnDrawGizmosSelected()
	{

		
		if(insideWater)
		{	

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.transform.position, range*2/3);
			
			if(stPos != Vector3.zero)
			{	
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(stPos + this.transform.right*swimdistance, stPos - this.transform.right*swimdistance);
			}
			else
			{	
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(this.transform.position + this.transform.right*swimdistance, this.transform.position - this.transform.right*swimdistance);
			}
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.transform.position, range);
		}
			
		
	}
}

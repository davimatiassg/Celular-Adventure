using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JpTrypBehavior : MonoBehaviour
{	

	[SerializeField] private LayerMask solid;

	[SerializeField] private CombatEnemy mainCode;

	private Transform Player; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim;

	private SpriteRenderer spr;

	private CircleCollider2D col;

    [SerializeField] private float jumpForce;

    [SerializeField] private float range;

    private float dir = 0;

    [SerializeField] private bool Grounded;

    private AudioInterface a;

 // Start is called before the first frame update
    void Start()
    {	
    	a = this.gameObject.GetComponent<AudioInterface>();
        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		spr = GetComponent<SpriteRenderer> ();

		mainCode = GetComponent<CombatEnemy>();

		col = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {	
    	bool stuned = this.mainCode.stuncheck();
    	if(!stuned)
    	{
    		Grounded = Physics2D.OverlapCircle(col.bounds.center, col.radius*6f, solid) && rigb.velocity.y <= 0;
	        
	        if(Grounded)
	        {	
	        	dir = searchPlayer();
	        	trs.eulerAngles = Vector3.zero;

	        	if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("trans"))
	        	{	
	        		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
	        		{
	        			anim.Play("land");
	        			a.PlaySound("slime");
	        		}
	        		else
	        		{	
	        			anim.Play("Idle");
	        		}
	        	}
	        	else
	        	{
	        		rigb.velocity = rigb.velocity.x/2*Vector2.right + Vector2.up*rigb.velocity.y;
	        	}

	        }
	        else
	        {	
	        	if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
	        	{
	        		anim.Play("Jump");
	        		a.PlaySound("slime");
	        	}
	        	
	        	trs.eulerAngles = -dir*Vector3.forward*60 + Vector3.forward* Mathf.Clamp(30, -45, rigb.velocity.y*4)*dir;
	        }
    	}
    	else
    	{
    		anim.Play("Stun");
    	}

    }

    public float searchPlayer()
    {
    	float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
    	if(PlayerDistance <= range)
		{	
			anim.Play("JumpStill");
		    return Mathf.Sign(Player.transform.position.x -trs.position.x);
		}
		return 0;
    }


    public void jumpGo()
    {
    	anim.Play("Jump");
    	rigb.velocity = Vector2.up*jumpForce + Vector2.right*dir*jumpForce/2;
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, range);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taenia1Behavior : MonoBehaviour
{
	private Transform Player; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim; //variável que receberá o valor do componente de animação (a Classe Animator) do inimigo

	private Vector3 PlayerDistance; //vetor resultante do cálculo para achar a distância entre o jogador e o inimigo

	public Vector2 range; 

	public float speed; //velocidade de movimento desse inimigo

	private float dir = -1; //direção em que o inimigo está olhando

	private CombatEnemy combat;

	public bool hooked;

	[SerializeField] private float hookcd = 0f;





	// Use this for initialization
	void Start () //método padrão do unity que roda no início da fase/cena
	{

		Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		combat = this.gameObject.GetComponent<CombatEnemy>();
	}

    // Update is called once per frame

    void Update()
    {	
    	if(!combat.stuncheck() )
    	{	
    		PlayerDistance = Player.transform.position - trs.position;

    		if(Mathf.Abs(PlayerDistance.y) < range.y)
    		{
    			if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
    			{
    				trs.localScale = new Vector3(Mathf.Abs(trs.localScale.x) * -dir, trs.localScale.y, trs.localScale.z);
    				dir = Mathf.Sign(PlayerDistance.x);
    			


					if(Mathf.Abs(PlayerDistance.x) < range.x*1/4 )
					{	
							Debug.Log("atk");

							anim.Play("lil taenia Melee");
							rigb.velocity = new Vector2(0f, 0f);
					}
					else if(Mathf.Abs(PlayerDistance.x) < range.x/2)
					{	
						if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk") && hookcd <= 0f)
						{	
							Debug.Log("hook");

							anim.Play("lil taenia Hook");
						}
						
					}
					else if(Mathf.Abs(PlayerDistance.x) < range.x)
					{	
						Debug.Log("walk");
						anim.Play("lil taenia Walk");
						rigb.velocity = new Vector2(speed*dir, rigb.velocity.y);	
					}
			
					else
					{	
						GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>().TogglePlayable(true);
						anim.Play("lil taenia Idle");
						rigb.velocity = new Vector2(0f, rigb.velocity.y);
						
					}
					hookcd = Mathf.Abs(hookcd) - Time.deltaTime;
				}
    		}
    	}
    	else
    	{	GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>().TogglePlayable(true);
    		anim.Play("lil taenia takedamage");
 
    	}
    }


    public void grabPlayer()
    {
    	RaycastHit2D hit;

    	if(hooked)
    	{
    		hooked = false;
			hookcd = 2;
    	}
    	else
    	{
    		hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 4*dir, this.transform.position.y - 3), new Vector2(dir, 0), 7f);
    		
    		if(hit)
    		{	

    			if(hit.collider.CompareTag("Player"))
    			{	
    				MasterController hited = hit.collider.gameObject.GetComponent<MasterController>();
    				hited.trs.position = new Vector2(this.transform.position.x + 3*dir, this.transform.position.y - 3);
    				hited.anim.Play("takedmg");
					hited.isGrounded = false;
    				hooked = true;
    				
    			}
    	   	}

    	}
    }
}

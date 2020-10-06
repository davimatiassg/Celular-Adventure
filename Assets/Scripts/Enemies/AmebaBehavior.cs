using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AmebaBehavior : MonoBehaviour {

	//declaração das variáveis:

	public float walktime;

	private float wkt = 0f;

	public float speed;

	public Transform Eye; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim; //variável que receberá o valor do componente de animação (a Classe Animator) do inimigo

	private CombatEnemy combat;

	[SerializeField] private bool ststill;


	private RaycastHit2D detectedobj;

	// Use this for initialization
	void Start () //método padrão do unity que roda no início da fase/cena
	{	
		wkt = walktime;

		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		combat = GetComponent<CombatEnemy>(); //vai buscar aquele script genérico para todos os inimigos, que tem os métodos pra tomar dano e oscar alho

	}
	
	
	void Update () // Update is called once per frame (uma vez por frame, esse método aqui é chamado)
	{	

		if(combat.life <= 0)
		{
			anim.Play("Dead");
			if(Mathf.Abs(rigb.velocity.x/5) >= 0.6)
			{
				rigb.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
			else
			{
				rigb.constraints = RigidbodyConstraints2D.None;
			}
			
			combat.KnBackIntensity = new Vector2(2f, 2f);
			anim.SetFloat("RollSpeed", -1*rigb.velocity.x/5);
		}
		else
		{	
			detectedobj = Physics2D.Raycast(new Vector2(Eye.position.x, Eye.position.y) , Vector2.left * (trs.localScale.x/Mathf.Abs(trs.localScale.x)), Mathf.Infinity, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);

			if(combat.stuncheck())
			{
				anim.Play("Stun");
			}

			if(detectedobj.distance <= 2f && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over"))
			{
				if(detectedobj.collider.gameObject.CompareTag("Player"))
				{
					anim.Play("Attack");
					rigb.velocity = new Vector2(0f, rigb.velocity.y);
				}
				else if(((detectedobj.collider.gameObject.CompareTag("Untagged") || detectedobj.collider.gameObject.CompareTag("hitable")) && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over")) || ststill)
				{

					rigb.velocity = new Vector2(0f, rigb.velocity.y);
					anim.Play("Idle");
					wkt -= Time.deltaTime;
				}
			}
			else if(wkt >= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over") && !ststill)
			{	
				rigb.velocity = new Vector2(-trs.localScale.x/Mathf.Abs(trs.localScale.x) * speed, rigb.velocity.y);

				anim.Play("Walk");
				wkt -= Time.deltaTime;
			}
			if(wkt <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over") && !ststill)
			{
				Flip();
				wkt = walktime;
			}

		}
		

		//stuned = combat.stuncheck(); //vai verificar se o inimigo tomou atordoamento
	
	}

	void OnCollisionEnter2D(Collision2D other)//método de ataque
	{
		if(other.gameObject.tag.Equals("hitable") && rigb.velocity.x > 20 && combat.life <= 0)
		{
				var hited = other.gameObject.GetComponent<CombatEnemy>();
				hited.takedamage(50, rigb.velocity);
		}
	}
	public void Flip()
	{
		trs.localScale = trs.localScale = new Vector2 (-trs.localScale.x, trs.localScale.y);
	}
}

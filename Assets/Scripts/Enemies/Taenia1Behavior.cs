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

	private float dir; //direção em que o inimigo está olhando

	private CombatEnemy combat;


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
    	if(!combat.stuncheck())
    	{
    		PlayerDistance = Player.transform.position - trs.position; //calcula a distância entre o Player e o inimigo

			dir = PlayerDistance.x/Mathf.Abs(PlayerDistance.x); //mude a direção do inimigo, para ele olhar para o jogador

			if(Mathf.Abs(PlayerDistance.x) < range.x && Mathf.Abs(PlayerDistance.y) < range.y)
			{
				anim.Play("lil taenia Walk");
				rigb.velocity = new Vector2(speed*dir, rigb.velocity.y);
				trs.localScale = new Vector3(Mathf.Abs(trs.localScale.x) * -1f * dir, trs.localScale.y, trs.localScale.z);
			}
			else
			{
				anim.Play("lil taenia Idle");
				rigb.velocity = new Vector2(0f, rigb.velocity.y);
			}	
    	}
    	else
    	{
    		anim.Play("lil taenia takedamage");
 
    	}
    }
}

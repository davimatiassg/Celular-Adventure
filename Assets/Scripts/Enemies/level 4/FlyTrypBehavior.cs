using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyTrypBehavior : MonoBehaviour
{
	[SerializeField] private float atkdelay;

	[SerializeField] private float ad;

	private CombatEnemy mainCode;

	private Transform Player; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim;

	private SpriteRenderer spr;

	[SerializeField] private Vector3 stPos;

    [SerializeField] private float speed;

    [SerializeField] private float range;

	[SerializeField] private float flydistance;

	[SerializeField] private bool advancing;

    void Start()
    {	

        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		spr = GetComponent<SpriteRenderer> ();

		mainCode = GetComponent<CombatEnemy>();

		stPos = trs.position;
    }	

    // Update is called once per frame
    void Update()
    {	
    	bool stun = mainCode.stuncheck();
    	
    	if(!stun)
    	{	
    		rigb.velocity = Vector2.zero;

    		if(ad > 0)
    		{
    			ad -= Time.deltaTime;
    		}
    		float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
    		if(PlayerDistance <= range && anim.GetCurrentAnimatorStateInfo(0).IsName("Fly") && ad <= 0)
    		{
    			advancing = true;
    			anim.Play("Dash");
    		}
    		Vector3 frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*flydistance, Time.fixedDeltaTime*speed);
    		if(advancing)
    		{
    			frameTranslocation = Vector2.MoveTowards(trs.position, Player.position, Time.deltaTime*speed*3);
    			trs.localEulerAngles = Vector2.up * (90 - 90*Mathf.Sign(trs.position.x - Player.position.x));
    		}
    		else
    		{	
    			if(trs.position.y == stPos.y)
    			{
    				anim.Play("Fly");
    			}
    			else
    			{
    				frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*flydistance, Time.fixedDeltaTime*speed*3);
    			}
    		}
			trs.position = frameTranslocation;
			if(stPos - trs.right*flydistance - trs.position == Vector3.zero && anim.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
			{	
					        		
				trs.localEulerAngles += Vector3.up*180;

				trs.position = Vector2.MoveTowards(trs.position, stPos + trs.right*flydistance, Time.deltaTime*speed*4);
    		}
		}
		else
		{
			anim.Play("Stun");
		}
    }

    public void Retreat()
    {
    	advancing = false;
    	ad = atkdelay+1;
    } 
    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.transform.position, range);
		Gizmos.color = Color.blue;
		if(stPos != Vector3.zero)
		{
			Gizmos.DrawLine(stPos + this.transform.right*flydistance, stPos - this.transform.right*flydistance);
		}
		else
		{
			Gizmos.DrawLine(this.transform.position + this.transform.right*flydistance, this.transform.position - this.transform.right*flydistance);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Staph1Behavior : MonoBehaviour {

	//declaração das variáveis:

	
	private Transform Player; //declaração para receber a classe das coordenadas do jogador

	private Transform trs; //declaração para receber a classe das coordenadas do inimigo
	
	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	private Animator anim; //variável que receberá o valor do componente de animação (a Classe Animator) do inimigo

	private Vector3 PlayerDistance; //vetor resultante do cálculo para achar a distância entre o jogador e o inimigo

	public float rangex; //distância da "visão do inimigo" (até onde ele consegue enxergar o jogador) no eixo x

	public float rangey; //distância da "visão do inimigo" (até onde ele consegue enxergar o jogador) no eixo x

	public float speed; //velocidade de movimento desse inimigo

	private float dir; //direção em que o inimigo está olhando

	private Vector2 PdMod; //módulo da distância entre o inimigo e o jogador (por que né, o jogador pode estar atrás do inimigo e vice-versa)

	public bool Charge = false; //variável que indica se o inimigo está carregando um ataque

	private float loadtime; //tempo atual de carregamento do ataque do inimigo

	public float load;//o tempo que leva para ele carregar o ataque (o comportamento desse inimigo é andar até uma distância específica até o jogador, parar por um tempo carregando o ataque e depois, se jogar na direção do player pra causar dano)

	private bool alratack = false; // variável que identifica se o inimigo já atacou

	public bool stuned; // variável que identifica se o inimigo foi atordoado (ela vai pegar verificar usando o método stuncheck, do script CombatEnemy.cs)

	public bool dash; //verifica se o inimigo está se lançando em direção ao jogador




	// Use this for initialization
	void Start () //método padrão do unity que roda no início da fase/cena
	{

		Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
		
		trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

		rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
		
		anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

		alratack = false; //isso mesmo que você ta leno

		Charge = false; //n sei pq isso tá aqui só ignora

		loadtime = load;// o tempo que leva pra carregar o ataque = o tempo que você especificar no 
	}
	
	
	void Update () // Update is called once per frame (uma vez por frame, esse método aqui é chamado)
	{
		dash = (Mathf.Abs(rigb.velocity.x) > speed); //vai verificar se o inimigo está se lançando em direção ao jogador, o módulo da velocidade dele for maior que a sua velocidade normal de andar(que seria a variável "speed")

		var combat = GetComponent<CombatEnemy>(); //vai buscar aquele script genérico para todos os inimigos, que tem os métodos pra tomar dano e oscar alho

		stuned = combat.stuncheck(); //vai verificar se o inimigo tomou atordoamento

		anim.SetBool("Stunned", stuned); //vai mudar um atributo do componente de animação, para que se o inimigo for atordoado, o animator tocar a animação de atordoamento

		anim.SetBool("Dash", dash); //vai mudar um atributo do componente de animação, para se o inimigo se lançar no jogador, o animator tocar a animação correspondente a esse ataque

		if(stuned) //se tiver atordoado
		{
			loadtime = load; //vai voltar ao tempo máximo para carregar o ataque

			anim.SetBool("Charge", false); //vai dizer para o animator não tocar mais a animação de carregar o ataque especial

			Charge = false; //vai parar de carregar o ataque

			anim.Play("takedmg"); //vai tocar a animação de inimigo tomando dano 
		}


		PlayerDistance = Player.transform.position - trs.position; //calcula a distância entre o Player e o inimigo

		PdMod = new Vector2 (Mathf.Abs(PlayerDistance.x), PlayerDistance.y); //encontra o módulo da distância através do método Mathf.Abs();

		if (!Charge)//se não estiver carregando o ataque
		{
			dir = PlayerDistance.x/PdMod.x; //mude a direção do inimigo, para ele olhar para o jogador
		}
		//fiz isso para que, caso o jogador seja esperto e note que o inimigo está carregando um ataque especial e decida pular para trás dele, o inimigo parar de carregar o ataque e virar para o jogador


		if(PdMod.x < rangex && PdMod.y < rangey && !stuned) //se o jogador estiver perto o suficiente e o inimigo não estiver atordoado
		{
			if(!Charge)  
			{
				rigb.velocity = new Vector2(speed*dir, rigb.velocity.y); //ande em direção ao jogador (muda a velocidade para um vetor com a direção do jogador e a velocidade de andar do inimigo)
				anim.SetBool("Charge", true);//faz com que o inimigo possa carregar o ataque
			}
			if(PdMod.x < rangex/2 && PdMod.y < rangey/4)//se o jogador está ainda mais perto
			{
				Attack();// começe a carregar o ataque
			}
			else
			{
				alratack = false; //"variável que indica se o inimigo já atacou" = false

				loadtime = load; //voltar ao tempo máximo para carregar o ataque

				anim.SetBool("Charge", false); //vai parar de carregar o ataque
			}
			
		}

		if(Mathf.Abs(rigb.velocity.x) == 0) //se está parado
		{
			Charge = false; //não carregar o ataque
		}
		

		trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*-dir, trs.localScale.y); // mudar a escala (literalmente, o tamanho do objeto) para virar para a direção em que o jogador está (uma escala horizontal (ou largura se preferir), negativa, faz com que o objeto vire para o outro lado (esquerda ou direita) )
		
		}
	

	void Attack()//método de ataque
	{
			Charge = true;//está carregando o ataque
			if (!alratack) //se já não tiver atacado
			{
				loadtime -= Time.deltaTime; //diminua o tempo para usar o ataque em um frame

				anim.SetBool("Charge", true); //"avisae pro animator que pode rodar a animação de carregar o ataque"
				if(loadtime > 0.0f) //até o tempo de carregamento do ataque acabar ¬
				{	//															   v
					rigb.velocity = new Vector2(0.0f, rigb.velocity.y); //fica parado, corno
				}
			}

			if(loadtime <= 0.0f) //se o tempo de carregamento do ataque acabou
			{
				rigb.AddForce(new Vector2(speed*4*dir, 0.0f), ForceMode2D.Impulse);//empurre o inimigo na direção do jogador, muito rápido

				anim.SetBool("Charge", false); //"já parou de carregar, não precisa mais tocar a animação de carregamento"

				loadtime = load; // reinicia o tempo de carregamento do ataque

				alratack = true;// o inimigo acabou de atacar, não dá pra ele começar a carregar o ataque de novo até ele ficar parado e na distância certa(linha 116)
			}

	}
	
}

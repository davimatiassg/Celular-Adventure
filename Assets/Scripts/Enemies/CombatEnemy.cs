using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEnemy : MonoBehaviour {

	[Header("Detalhes do inimigo")]

    [Tooltip("Identificador")]
    [SerializeField] public int enemyID; 

    [Tooltip("Nome científico")]
    [SerializeField] public string enemyName;

    [Tooltip("Local de Encontro")]
    [SerializeField] public string encounterLocal; 

    [TextArea]
    [Tooltip("Comportamento In-Game")]
    [SerializeField] public string enemyBehavior;

    [TextArea]
    [Tooltip("Informações científicas")]
    [SerializeField] public string realInfo; 

    [SerializeField] public Sprite inGameImg;

    [SerializeField] public Sprite realImg;

	[Header("Atributos do Inimigo")]

	//declaração das variáveis:
	public bool DieDestroy = true;

	public string tname; //nome do inimigo

	private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

	public int life; //vida do inimigo

	public int attackdmg; //o dano aplicado ao jogador caso ele entre na área de colisão

	public float hitstun = 1.4f; //o tempo de atordoamento que o inimigo receberá ao tomar dano do jogador  (stun = atordoamento, só pra constar)

	[SerializeField] private float stun; //a mesma coisa que a variável anterior, só que essa daqui vai ser utilizada nos cálculos (a cada instante(frame) em que estiver atordoado, o tempo de atordoamento restante diminui)

	private bool stuned; // uma bool pra identificar se o iinimigo está atordoado ou não

	public GameObject hiteffect; //um espaço para colocar um objeto, que será instanciado e usado como efeito visual ao acertar o inimigo

	public ParticleSystem hitparticles; //espaço para receber o objeto que controla as os efeitos com partículas visuais do inimigo 

	private SpriteRenderer HitF; //isso daqui vai receber a classe que controla a cor da variável da linha 23 (GameObject hiteffect)


	public Vector2 KnBackIntensity = new Vector2(1f, 1f);

	public AudioInterface a;
			

	void Start()//método padrão do unity que roda no início da fase/cena
	{
		a = this.gameObject.GetComponent<AudioInterface>(); //this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<AudioSource>() vai pegar o componente de áudio dele e jogar para a variável aud;
	}

	private void OnTriggerStay2D(Collider2D other)//método padrão do unity que roda sempre que um objeto entrar numa caixa de colisão classificada como gatilho ("Trigger", que não vai ser solida mas vai ativar um comando quando algo entrar nela)
	{								//^^^^^ other é o componente do colisor que entrou no gatilho do inimigo

		if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && !stuned) //vai ver se o objeto que entrou na caixa de colisão é o jogador e se o inimigo não está atordoado
		{
			var hited = other.gameObject.GetComponent<MasterController>();//vai receber a classe de controle geral, que tem em todos os jogadores
			hited.takedamage(attackdmg); // chama o método que faz o jogador tomar dano, dentro do código dele
		}

	}

	public void takedamage(int dmg, Vector2 knockback) //método que faz o inimigo tomar dano, dmg = dano recebido (inteiro) e Knockback = direção e intensidade da repulsão (Vector2(x, y))
	{	
		
		if(life > 0 || (life <= 0 && !DieDestroy) )//verifica se o inimigo é capaz de receber dano de alguma forma
		{	
			stun = hitstun; //faz com que o inimigo tenha o tempo de atordoamento ativado
			stuned = true; //ativa o status de atordoado
			if(life > dmg || (life <= 0 && !DieDestroy)) // se o inimigo não for morrer ao tomar esse dano...
			{
				if(hiteffect != null)
				{
					Instantiate(hiteffect, transform.position, transform.rotation); // instanciar o efeito visual para tomar dano não-letal
				}
				
				if(a != null)
				{
					a.PlaySound("dmg");
				}
				
				life -= dmg; //diminuir a vida do inimigo
			}

			else //se ele for morrer ao receber esse dano
			{	
				if(a != null)
				{
					a.PlaySound("die");
				}
				
				life = 0; // duh, vida = 0
				

				GameEvents.ScreamEvent("EnemyKilled"); //diga pra todo mundo que um inimigo morreu
				CardIndex enemyCard = new CardIndex(enemyID, realImg, inGameImg, encounterLocal, enemyBehavior, enemyName, realInfo);
				BestiaryElements.AddCardEnemy(enemyCard);
				PontuationCounter.AddScore(500);
			}

			this.gameObject.GetComponent<Rigidbody2D>().velocity += knockback * KnBackIntensity; 
		}
	}


	public bool stuncheck() // um método para outro script conseguir ver se o inimigo está stunado, usado para comunicação
	{
		return stuned;
	}
	public void SetStuned(float t = 1)
	{
		
		stun = t;
		if(t == 0)
		{
			stuned = false;
		}
		else
		{
			stuned = true;
		}

	}


	void Update() //método padrão do unity que roda no início de cada frame
	{
		if (life <= 0) //verifica se o inimigo está morto
		{	
			if(DieDestroy)
			{
				if(!stuned) // se ele não estiver atordoado...
				{	
					Destroy(this.gameObject); //destrua esse objeto

				}
				else //se ele estiver atordoado (pra gente poder ver o inimigo saindo voando depois de tomar o último hit)
				{	

					HitF = Instantiate(hiteffect, transform.position, transform.rotation).GetComponent<SpriteRenderer>(); //instanciar o efeito visual de dano
					HitF.color = new Color(1f, stun, stun, 1f); //escolher uma cor para o efeito cada vez mais avermelhada de acordo com o tempo de atordoamento restante 
					HitF.gameObject.transform.localScale = new Vector3(1/(stun+0.1f)+6f, 1/(stun+0.1f)+6f, 1f); // ir aumentando o tamanho do efeito visual
					this.gameObject.GetComponent<Collider2D>().enabled = false; // desabilitar os colisores do inimigo, para ele não bater em nada
				}
			}
			else
			{
				if(!stuned)
				{	
					GameEvents.ScreamEvent("EnemyKilled");
					CardIndex enemyCard = new CardIndex(enemyID, realImg, inGameImg, encounterLocal, enemyBehavior, enemyName, realInfo);
					BestiaryElements.AddCardEnemy(enemyCard);
					PontuationCounter.AddScore(500);
					a.PlaySound("die");
				}
				SetStuned(5);
				life = 0;
				
			}

		}
		else
		{
			if(stuned && stun <= 0)
			{
				stuned = false;
			}
		}

		if(stun > 0.0f && (DieDestroy || life > 0)) //se o inimigo estiver atordoado..
		{
			stun -= Time.deltaTime; //diminuir o tempo restante de atordoamento, com base em quanto tempo passa entre cada frame
		}
		else if(DieDestroy || life > 0) //se ele não estiver atordoado
		{
			stuned = false; //então ele não está atordoado. (não dá pra ser verdadeiro e falso ao mesmo tempo ainda.)
		}

	}
}

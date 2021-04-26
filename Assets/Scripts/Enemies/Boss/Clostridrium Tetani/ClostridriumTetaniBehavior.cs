using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClostridriumTetaniBehavior : MonoBehaviour {

	[Header("Detalhes do Boss")]

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

	private Animator anim;

	public GameObject P;

	private MasterController player;

	private float timebtwattacks;

	public bool isIdle, isEntry, isPinch;

	private List<string> atks = new List<string>{"atk1", "atk2", "atk3"};

	public HitableParts Core;

	private bool paralized;

	private float starty;



	void OnEnable()
	{
		GameEvents.StartListening("BossDie", Die);
	}
	void OnDisable()
	{
		GameEvents.StopListening("BossDie", Die);
	}

	void Awake () 
	{
		
		gameObject.SetActive(false);
		timebtwattacks = 4f;

	}

	void Start()
	{
		anim = GetComponent<Animator> ();
		player = GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController> ();
		starty = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
			if(Core.life <= Core.maxlife*2/5 && Core.life > 0)
			{
				isPinch = true;
			}
			else
			{
				isPinch = false;
			}
			isIdle = anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
			if(isIdle || anim.GetCurrentAnimatorStateInfo(0).IsTag("Pinch"))
			{
				timebtwattacks -= Time.deltaTime;
				if(timebtwattacks <= 0)
				{	
					if (isPinch)
					{
						timebtwattacks = 2f;
						anim.speed = 1.7f;
					}
					else
					{
						timebtwattacks = 4f;
					}

					int atkchosen = Random.Range(0, atks.Count);
					anim.SetTrigger(atks[atkchosen]);
					
				}
			}
			anim.SetBool("IsPinch",isPinch);
	}

	private void Die()
	{	
		
		anim.SetTrigger("Die");
		player.TogglePlayable(false);
		Time.timeScale = 1f;
		CardIndex enemyCard = new CardIndex(enemyID, realImg, inGameImg, encounterLocal, enemyBehavior, enemyName, realInfo);
		BestiaryElements.AddCardEnemy(enemyCard);
		PontuationCounter.AddScore(3000);
	}

	private void Dead()
	{	
		this.gameObject.SetActive(false);
		GameEvents.ScreamEvent("FinalBossIsDead");
		GameEvents.ScreamEvent("BossDead");
		GameEvents.ScreamEvent("FadeOut");
		Debug.Log("BossDead");
		Time.timeScale = 1f;
		Destroy(this.gameObject);

	}



}

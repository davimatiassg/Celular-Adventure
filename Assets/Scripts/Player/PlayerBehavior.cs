using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour 
{

	//atributos do objeto
	private Rigidbody2D rigb;
	private Animator anim;
	private Transform trs;
	public Transform flchk;
	public Transform tpchk;
	public Collider2D hitbox;
	public SpriteRenderer spr;

	public CameraBehavior camcontroll;
	public ParticleSystem dustmaker;
	public ParticleSystem DmgParticles;
	public ParticleSystem HitParticle;


	//bools privadas para detecção de estado
	private bool isGrounded;
	private bool isWalling;
	private bool isinMov;
	private bool isJump;
	private bool isAscend;
	public bool isCrouch;
	public bool isRoll;

	//variáveis dos controles
	public float axis;
	private float ayis;
	private bool atk;
	private bool reset;

	//atributos de movimentação
	public float speed;
	public float jspeed;
	
	//atributos de movimentação específicos
	public int movSen;
	private int lstmovSen;
	private float yfly;
	private float flytime;
	private float runtime;



	//variáveis para o combate
	public Collider2D AtkRadius;
	public int attackdmg;
	public float invtime;
	public float invt;
	private bool dead;
	public bool gothit;
	private bool downdash = false;
	private bool candowndash;
	private bool aheavied;
	public Vector2 knockback;
	public float rollcdr;
	public float rolltm;
	public float downdashtm = 0.3f	;

	//vida e hud
	public HealthBar lifeBar;
	public int life;
	public int maxlife;

	//checkpoint e pitfallpoint

	private Vector2 GroundReturn;


	//vetores de força
	private Vector2 Jforce;
	private Vector2 Wforce;
	private Vector2 Cforce;

	//partículas

	private bool landed;
	private bool dashed;

	//Layers
	public LayerMask solid;

	//variáveis usadas para corrigir bugs
	public float radius;
	public bool playable;

	private int framestop = 0;

	public GameObject FinalScreen;
	
	void Awake()
	{

	}

	// Use this for initialization
	void Start () 
	{
		
		//coletando componentes
		anim = GetComponent<Animator> ();
		rigb = GetComponent<Rigidbody2D> ();
		trs = GetComponent<Transform> ();
		spr = GetComponent<SpriteRenderer> ();
		camcontroll = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<CameraBehavior>();
		lifeBar = GameObject.FindWithTag("HealthBar").gameObject.GetComponent<HealthBar>();
		movSen = 1;
		lstmovSen = 1;
		life = maxlife;
		playable = true;
	}

	
	//main
	void Update () 
	{	
		if(playable)
		{
			Jforce = new Vector2 (0, jspeed);
			Cforce = new Vector2 (speed/12*movSen, 0.0f);
			isGrounded = Physics2D.OverlapCircle(flchk.position, radius, solid);;
			
			if(framestop > 0)
			{
				framestop -= 1;
				camcontroll.ToggleShake(true, 0.6f, 0.6f);
			}
			else
			{
				camcontroll.ToggleShake(false, 0.6f,  0.6f);
			}


			if (life <= 0)
	 		{
	 			life = 0;
	 			die();
	 		}
			if(invt > 0 && life>0f)
			{
				invt -= Time.deltaTime;
				spr.color = new Color(1, 1, 1, Mathf.PingPong(Time.time*10f, 1f));
			}
			else
			{
				spr.color = new Color(1, 1, 1, 1);
			}

				//"se o sentido de movimentação atual não é o que foi definido no último frame"
			if (movSen != lstmovSen)
			{
				FliptFr();
				if(isGrounded)
				{
					rigb.velocity = new Vector2(rigb.velocity.x/2,rigb.velocity.y);
				}
				
				lstmovSen = movSen;
				axis = 0;
				runtime = 0;
			}

			//setando o valor das variáveis de eixos x (axis) e y (ayis)

			if(Time.timeScale != 0f)
			{	
				lifeBar.ToggleVisibility(true);
				if (isGrounded)
				{
					GroundMoviment();
					if(!landed)
					{
						MakeDust();
						landed = true;
					}
				}
				else
				{	
					landed = false;
					AirMoviment();
				}
				

				Animate();



				if(!downdash)
				{
					GetControlInput();

					isinMov = (axis*rigb.velocity.x != 0);

					if(axis != 0.0f)
					{	
						if(Mathf.Abs(axis) == 1.0f)
						{
							movSen = (int) axis;
						}
					}	
				}
			}
		}
		else 
		{
			lifeBar.ToggleVisibility(false);	
			if(life <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Dying"))
			{
				anim.Play("Die");
				lifeBar.ToggleVisibility(false);

			}	
		}
		
	}

	//método para inverter a direção do objeto
	void FliptFr ()
	{	
		if(isGrounded)
		{
			MakeDust();
		}
		

		trs.localScale = trs.localScale = new Vector2 (-trs.localScale.x, trs.localScale.y);
	}

	void GetControlInput()
	{	
		axis =  Input.GetAxisRaw("Horizontal");
		ayis = Input.GetAxisRaw("Vertical");
		atk = Input.GetButtonDown("Fire1");
		reset = Input.GetButtonDown("reset");

		if(reset){die();}
		
	}

	void GroundMoviment()
	{
		flytime = 0;
		gothit = false;
		if(isCrouch && !isRoll)
		{
			rigb.velocity = new Vector2(0, rigb.velocity.y);
		}



		if(axis == 0)
		{
			rigb.velocity = new Vector2(0, rigb.velocity.y);
			runtime = 0f;
		}
			
		candowndash = true;
		downdash = false;

		//rolamento


		if(isCrouch == true && axis != 0 && rolltm >= rollcdr)
		{	
			isRoll = true;
		}

		if(isRoll)
		{
			rigb.velocity = new Vector2(speed*4*movSen, rigb.velocity.y);
			
			if(rolltm >= 0)
			{
				rolltm -= Time.deltaTime;
			}
			else
			{
				isRoll = false;
				rolltm -= rollcdr;
			}
		}
		else
		{	
			if(rolltm < 0)
			{
				rolltm += Time.deltaTime;
			}
			else
			{
				rolltm = rollcdr;
			}
		}

		//pulo
		if (Input.GetButtonDown("Jump"))
		{	
			rigb.AddForce(Jforce, ForceMode2D.Impulse);
			flytime = 0f;
			MakeDust();
			isRoll = false;
			isCrouch = false;
		}

		//"se as direcionais forem pressionadas e não estiver agachado" (andar)
		if (axis != 0 && !isCrouch && !anim.GetCurrentAnimatorStateInfo(0).IsTag("atk")) 
		{
			runtime += Time.deltaTime;
			if(runtime > 1f)
			{
				runtime = 1f;
			}

			rigb.velocity = new Vector2(1*movSen*speed*Mathf.Pow(0.05f, 0 - runtime), rigb.velocity.y);
		}

		//se a velocidade atual for maior que 2.5x a variável pública speed
		if	(rigb.velocity.x*movSen > speed*2 && !isCrouch)
		{	
			if(!dashed)
			{
				MakeDust();
				dashed = true;
			}
			rigb.velocity = new Vector2 (speed*movSen*2, rigb.velocity.y);
		}
		else
		{
			dashed = false;
		}
		//Se estiver num local com teto baixo, para agachar automaticamente
		if(ayis == -1 || Physics2D.Linecast(trs.position, tpchk.position, 1 << LayerMask.NameToLayer("solid")))
		{
			isCrouch = true;	
		}
		else 
		{	
			isCrouch = false;
		}
	}
	void AirMoviment()
	{	
		//movimento vertical
		flytime += Time.deltaTime;
			isRoll = false;
			isCrouch = false;

			if(Input.GetButton("Jump") ||  ayis > 0.0f)
			{	
				if(rigb.velocity.y >= 0.0f)
				{
					if(flytime <= 0.2f)
					{	
						if(rigb.velocity.y < jspeed)
						rigb.velocity = new Vector2(rigb.velocity.x, jspeed);
					}

				}
				else if(rigb.velocity.y < -17f)
				{
					rigb.velocity = new Vector2(rigb.velocity.x, -17f);
				}
			}


		//movimento horizontal


		if(axis == 0 && Mathf.Abs(rigb.velocity.x) > 0)
		{
			SlowDown(3.0f);
			runtime -= Time.deltaTime;

		}
		if(Mathf.Abs(rigb.velocity.x) < 1)
		{
			runtime = 0;
		}
		if (axis != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("flying down")) 
		{
			runtime += Time.deltaTime;
			if(runtime > 1f)
			{
				runtime = 1f;
			}

			rigb.velocity = new Vector2(rigb.velocity.x + movSen*speed*Mathf.Pow(0.5f, 3.5f - runtime), rigb.velocity.y);
		}
		if(Mathf.Abs(rigb.velocity.x) > speed*2)
		{
			rigb.velocity = new Vector2(movSen*speed*2, rigb.velocity.y);
		}

	}

	//método as animações do personagem
	void Animate()
	{	
		if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("takedmg"))
		{		if(rigb.velocity.y < -5)
				{
					yfly = -5;
				}
				else if(rigb.velocity.y > 5)
				{
					yfly = 5;
				}
				else
				{
					yfly = 0;
				}
				anim.SetFloat("AirmovV", yfly);

			if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{

				if (isGrounded) 
				{	if(isRoll)
					{
						anim.Play("Roll");
						isCrouch = isRoll;
						MakeDust();
					}
					else if(isCrouch)
					{
						anim.Play("Crouch");
						anim.SetTrigger("Crouch");
					}

					else if (isinMov)
					{
						anim.Play("Run");
					}

					else if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("idle") && !anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
					{
						anim.Play("Idle");
					}
				}
				else
				{
					if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
					{
						if(isinMov)
						{
							anim.Play("Dash_Jump");
						}
						else
						{
							anim.Play("Jump");
						}
					}
				}
			}
			Attack();
		}
		else
		{
			gothit = false;
		}	
	}

	public void Attack()
	{		

		if(atk)
		{
			if(isGrounded)
			{	
				if(anim.GetCurrentAnimatorStateInfo(0).IsTag("down"))
				{
					anim.Play("downtilt");
					MakeDust();

					knockback = new Vector2(3f*movSen, 15f);
				}
				else if(anim.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
				{
					anim.Play("first");
					rigb.velocity = new Vector2(0, rigb.velocity.y);
					rigb.AddForce(new Vector2(20*movSen, 0), ForceMode2D.Impulse);
					knockback = new Vector2(3f*movSen, 5f);

					
				}
				else if(anim.GetCurrentAnimatorStateInfo(0).IsTag("run"))
				{
					anim.Play("dash attack");
					rigb.velocity = new Vector2(0, rigb.velocity.y);
					rigb.AddForce(new Vector2(20*movSen, 0), ForceMode2D.Impulse);
					knockback = new Vector2(15f*movSen, 15f);
					
				}

				else if(anim.GetCurrentAnimatorStateInfo(0).IsName("first"))
				{
					anim.Play("second");
					MakeDust();
					rigb.AddForce(new Vector2(50*movSen, 0), ForceMode2D.Impulse);
					knockback = new Vector2(3f, 5f);
				}
				else if(anim.GetCurrentAnimatorStateInfo(0).IsName("second"))
				{
					anim.Play("third");
					rigb.velocity = new Vector2(0, 0);
					knockback = new Vector2(15f*movSen, 15f);
					
				}
				else if(anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
				{
					anim.SetTrigger("Atk");
					if((ayis < 0.0f) && !isGrounded && candowndash){downdash = true; anim.Play("flying down");}
				}
			}
			else
			{	if(!downdash)
				{
					if(candowndash && ayis <= 0.0f && atk)
					{	
						anim.Play("flying down");
						downdash = true;
						knockback = new Vector2(15f*movSen, 15f);
						

					}
					else if (ayis > 0.0f)
					{
						anim.Play("flying up");
						
						knockback = new Vector2(0f, 20f);
					}
				}
			}
		}

		if(!candowndash)
		{
			downdash = false;
			if(downdashtm < 0.3f)
			{
				downdashtm += Time.deltaTime;
			}
			else
			{
				candowndash = true;
			}

		}
		if(downdash)
		{
			rigb.velocity = new Vector2(movSen * 20.0f, -20.0f);

		}
		if(Mathf.Abs(rigb.velocity.x) < 20 && downdash)
		{
			downdash = false;
			rigb.velocity = new Vector2(-10*movSen, 20.0f);
		}
		anim.SetBool("Downattack", downdash);
	}

	public void takedamage(int dmg)
	{	

		if (!downdash && invt <= 0 && !isRoll)
		{	
			anim.Play("takedmg");
			life -= dmg;
			invt = invtime;
			gothit = true;

			framestop = 10;
			
			isGrounded = false;
			rigb.velocity = new Vector2(movSen*-10, 20);
			Explode();
		}
		lifeBar.SetGaugeValue(life, maxlife);

	}

	public void gainlife(int gainl)
	{
		life += gainl;

		if (life >= maxlife)
 		{
 			life = maxlife;
 		}
 		lifeBar.SetGaugeValue(life, maxlife);
	}

	private void die()
	{
		anim.Play("Die");
		TogglePlayable(false);
		spr.sortingLayerName = "ForeOcult";
		camcontroll.camscale = 4;
		camcontroll.camsensex = 0f;
		camcontroll.camsensey = 0f;
		camcontroll.targeted = true;
		

		
		//dead = true;
	}

	public void Dying()
	{
		camcontroll.ToggleShake(false, 0.6f,  0.6f);
		lifeBar.ToggleVisibility(false);
		FinalScreen.SetActive(true);


	}
	public void Dead()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
		SetTimeScale(1);
	}
	public void fall (int dmg)
	{	
		life -= dmg;
		invt = invtime;
		trs.position = GroundReturn;
		rigb.velocity = new Vector2(0, 0);
		lifeBar.SetGaugeValue(life, maxlife);
		candowndash = false;
		downdashtm = -0.3f;

	}

	private void OnTriggerEnter2D(Collider2D other)
	{	

		if ((other.gameObject.tag.Equals("hitable") || other.gameObject.tag.Equals("Boss")) && other.isTrigger == false)
		{	
			
			if(other.gameObject.tag.Equals("hitable"))
			{
				var hited = other.gameObject.GetComponent<CombatEnemy>();
				hited.takedamage(attackdmg, knockback);
			}
			if(other.gameObject.tag.Equals("Boss"))
			{
				var hited = other.gameObject.GetComponent<HitableParts>();
				hited.takedamage(attackdmg);
			}
			if(downdash)
			{	
				rigb.velocity = new Vector2(movSen, 40.0f);
				downdash = false;
				candowndash = false;
				downdashtm = 0.0f;			
			}

		}

	}
	public void GroundSave(Vector2 returnpoint)
	{
		GroundReturn = returnpoint;	
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(flchk.position, radius);
	}



 	public void TogglePlayable(bool x)
 	{
 		playable = x;
 	}
 	public void MakeDust()
 	{
 		dustmaker.Play();
 	}
 	public void Explode()
 	{
 		DmgParticles.Play();
 	}
 	public void SlowDown(float factor)
 	{
		rigb.velocity = new Vector2(rigb.velocity.x/factor, rigb.velocity.y);
 
 	}

/* 	void hitenemy(GameObject hited, bool isBoss)
 	{	
 		//HitParticle.Transform.position = hited.Transform.position;
 		//HitParticle.Play;
 		pass 
 	}


*/
 	void SetTimeScale(float x)
 	{
 		Time.timeScale = x;
 	}
}
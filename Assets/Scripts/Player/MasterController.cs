using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{	
	//atributos do objeto
	public Rigidbody2D rigb;
	public Animator anim;
	public Transform trs;
	public Transform flchk;
	public Transform tpchk;
	public Collider2D hitbox;
	public SpriteRenderer spr;


	public CameraBehavior camcontroll;
	public ParticleSystem dustmaker;
	public ParticleSystem DmgParticles;
	public ParticleSystem HitParticle;


	//bools privadas para detecção de estado
	public bool isGrounded;
	public bool isinMov;
	public bool isJump;
	public bool isAscend;
	public bool isCrouch;


	//variáveis dos controles
	public float axis;
	public float ayis;
	public bool i_jump;
	public bool jump;
	public bool atk;
	public bool atk2;
	public bool reset;

	//atributos de movimentação
	public float speed;
	public float maxspeed;
	public float jspeed;
	
	//atributos de movimentação específicos
	public int movSen;
	public int lstmovSen;
	public float yfly;
	public float flytime;
	public float runtime;
	public float dragforce = 1f;


	//variáveis para o combate
	public int attackdmg;
	public float invtime;
	public float invt;
	public bool dead;
	public bool gothit;
	public Vector2 knockback;


	//vida e hud
	public HealthBar lifeBar;
	public int life;
	public int maxlife;

	//checkpoint e pitfallpoint

	public Vector2 GroundReturn;


	//vetores de força
	public Vector2 Jforce;
	public Vector2 Wforce;
	public Vector2 Cforce;
	public Vector2 jumpDirection;

	//partículas

	public bool landed;
	public bool dashed;

	//Layers
	public LayerMask solid;

	//variáveis usadas para corrigir bugs
	public float radius;
	public bool playable;

	public int framestop = 0;

	public GameObject FinalScreen;

	private int jumpframes = 0;
    // Start is called before the first frame update
    void Start()
    {
        //coletando componentes
		anim = this.gameObject.GetComponent<Animator> ();
		rigb = this.gameObject.GetComponent<Rigidbody2D> ();
		trs = this.gameObject.GetComponent<Transform> ();
		spr = this.gameObject.GetComponent<SpriteRenderer> ();
		camcontroll = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<CameraBehavior>();
		lifeBar = GameObject.FindWithTag("HealthBar").gameObject.GetComponent<HealthBar>();
		movSen = 1;
		lstmovSen = 1;
		life = maxlife;
		playable = true;
    }

    // Update is called once per frame
    public void GetControlInput()
	{	
		jumpDirection = Vector2.up;
		axis =  Input.GetAxisRaw("Horizontal");
		ayis = Input.GetAxisRaw("Vertical");
		i_jump = Input.GetButtonDown("Jump");
		jump = Input.GetButton("Jump");
		atk2 = Input.GetButtonDown("Fire2");
		atk = Input.GetButtonDown("Fire1");
		reset = Input.GetButtonDown("reset");

		if(reset)
		{
			die();
		}
	}


	public void FliptFr ()
	{	
		if(isGrounded)
		{	
			MakeDust();
		}
		

		trs.localScale = trs.localScale = new Vector2 (-trs.localScale.x, trs.localScale.y);
	}
	void FixedUpdate()
	{	
		float d = 0.0f;
		jumpDirection = OnSlopeMoviment(out d);
		if(!isGrounded || (isGrounded && jumpDirection == Vector2.up && d > 0.05f))
		{
			rigb.velocity -= jumpDirection*2f;
		}

	}
	
	public void GroundMoviment()
	{	
		
		flytime = 0;
		gothit = false;
		if(!i_jump && jumpframes == 0)
		{
			if(axis == 0)
				{	
					rigb.velocity -= new Vector2(movSen*Mathf.Abs(rigb.velocity.x)*Mathf.Pow(dragforce, 2f)*runtime, Mathf.Abs(Vector2.Perpendicular(jumpDirection).y) + Mathf.Abs(rigb.velocity.y)*Mathf.Pow(dragforce, 3f));
					runtime -= dragforce*Time.deltaTime;

			if(runtime <= 0)
				{	
				
					runtime = 0;
					rigb.velocity -= new Vector2(rigb.velocity.x, 0);
				}
			}

			//"se as direcionais forem pressionadas e não estiver agachado" (andar)
			if (axis != 0 && !isCrouch && !anim.GetCurrentAnimatorStateInfo(0).IsTag("atk")) 
			{
				runtime += Time.deltaTime;
				if(runtime > 1f)
				{
					runtime = 1f;
				}
				rigb.velocity += new Vector2(dragforce*movSen*speed*Mathf.Pow(0.05f, 0 - runtime) * Mathf.Abs(Vector2.Perpendicular(jumpDirection).x), -dragforce*movSen*speed*Mathf.Pow(0.05f, 0 - runtime) * Vector2.Perpendicular(jumpDirection).y);
			}

			//se a velocidade atual for maior que 2.5x a variável pública speed
			if	(Mathf.Abs(rigb.velocity.x) > speed*maxspeed && !isCrouch)
			{	
				if(!dashed)
				{
					MakeDust();
					dashed = true;
				}
				rigb.velocity = new Vector2 (speed*movSen*maxspeed, rigb.velocity.y);
			}
			else
			{
				dashed = false;
			}

			if((Mathf.Abs(rigb.velocity.x) < Mathf.Abs(rigb.velocity.y) ||  (Mathf.Abs(rigb.velocity.x) > Mathf.Abs(rigb.velocity.y)) && jumpDirection != Vector2.up))
			{	
				rigb.velocity = new Vector2(rigb.velocity.x, (Mathf.Abs(rigb.velocity.x)/Mathf.Abs(Vector2.Perpendicular(jumpDirection).x))*-movSen*Vector2.Perpendicular(jumpDirection).y);
			}
		}
		
		//pulo
		else if(i_jump)
		{	
			jumpframes = 3;

			rigb.velocity += Vector2.up*jspeed;
			flytime = 0f;
			MakeDust();
			isCrouch = false;
		}
		else
		{
			jumpframes -= 1;
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
	public void AirMoviment()
	{	
		//movimento vertical
		flytime += Time.deltaTime;
		
		
			isCrouch = false;

			if(jump)
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
			if(maxspeed >= 2f)
			{
				maxspeed -= Time.deltaTime*2;
			}
			else
			{
				maxspeed = 2f;
			}

		if(axis == 0 && Mathf.Abs(rigb.velocity.x) > 0)
		{
			SlowDown(1f/3.0f);
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
		if(Mathf.Abs(rigb.velocity.x) > speed*maxspeed)
		{
			rigb.velocity = new Vector2(movSen*speed*maxspeed, rigb.velocity.y);
		}

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
			
		}
	}

	public void takedamage(int dmg)
	{	

		if (invt <= 0)
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

	public void die()
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

	}

	public void GroundSave(Vector2 returnpoint)
	{
		GroundReturn = returnpoint;	
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(flchk.position, flchk.position + Vector3.down*radius);
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
		rigb.velocity = new Vector2(rigb.velocity.x*factor, rigb.velocity.y);
		runtime -= Time.deltaTime;
 	}

	public void SetTimeScale(float x)
 	{
 		Time.timeScale = x;
 	}

 	private Vector2 OnSlopeMoviment(out float dist)
 	{	
 		RaycastHit2D hit = Physics2D.Raycast(flchk.position, Vector2.down, 1f, solid);

 		if(hit)
 		{	
 			dist = hit.distance;
 			float Angle = Vector2.Angle(hit.normal, Vector2.up);

 			Debug.DrawRay(hit.point, Vector2.Perpendicular(hit.normal), Color.green);
 			Debug.DrawRay(hit.point, hit.normal, Color.red);

 			return hit.normal;
 		}
 		dist = 0.0f;
 		return Vector2.up;
 	}


}

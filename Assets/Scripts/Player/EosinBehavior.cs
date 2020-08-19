using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EosinBehavior : MonoBehaviour
{
    [SerializeField] private MasterController mainCode;
    [SerializeField] private GameObject Shot;
    [SerializeField] private Transform ShotLocal;
   	private float TbtwAtk = 0f;
   	private bool atkqueue = false;
   	private float direction;

    private bool downdash = false;
	private bool candowndash;
	public float downdashtm = 0.3f;
	private float yfly;
	public bool atk2;	

    void Start()
    {
        mainCode = this.gameObject.GetComponent<MasterController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
		if ((other.gameObject.tag.Equals("hitable") || other.gameObject.tag.Equals("Boss")) && other.isTrigger == false)
		{	
			if(downdash)
			{	
				mainCode.rigb.velocity = new Vector2(mainCode.movSen, 40.0f);
				downdash = false;
				candowndash = false;
				downdashtm = 0.0f;			
			}
		}
	}
    // Update is called once per frame
	void Update()
    {	
    	if(TbtwAtk > 0f)
    	{	
    		Debug.Log(TbtwAtk);
    		TbtwAtk -= Time.deltaTime;
    		
    	}
    	else
    	{
    		TbtwAtk = 0f;
    	}
    	if(mainCode.invt == mainCode.invtime)
    	{
    		candowndash = false;
			downdashtm = -0.3f;
    	}
      if(mainCode.playable)
		{
			mainCode.Jforce = new Vector2 (0, mainCode.jspeed);
			mainCode.Cforce = new Vector2 (mainCode.speed/12*mainCode.movSen, 0.0f);
			mainCode.isGrounded = Physics2D.OverlapCircle(mainCode.flchk.position, mainCode.radius, mainCode.solid);;
			
			if(mainCode.framestop > 0)
			{
				mainCode.framestop -= 1;
				mainCode.camcontroll.ToggleShake(true, 0.6f, 0.6f);
			}
			else
			{
				mainCode.camcontroll.ToggleShake(false, 0.6f,  0.6f);
			}


			if (mainCode.life <= 0)
	 		{
	 			mainCode.life = 0;
	 			mainCode.die();
	 		}
			if(mainCode.invt > 0 && mainCode.life>0f)
			{
				mainCode.invt -= Time.deltaTime;
				mainCode.spr.color = new Color(1, 1, 1, Mathf.PingPong(Time.time*10f, 1f));
			}
			else
			{
				mainCode.spr.color = new Color(1, 1, 1, 1);
			}

				//"se o sentido de movimentação atual não é o que foi definido no último frame"
			if (mainCode.movSen != mainCode.lstmovSen)
			{
				mainCode.FliptFr();
				if(mainCode.isGrounded)
				{
					mainCode.rigb.velocity = new Vector2(mainCode.rigb.velocity.x/2, mainCode.rigb.velocity.y);
				}
				
				mainCode.lstmovSen = mainCode.movSen;
				mainCode.axis = 0;
				mainCode.runtime = 0;
			}

			//setando o valor das variáveis de eixos x (axis) e y (ayis)

			if(Time.timeScale != 0f)
			{	
				mainCode.lifeBar.ToggleVisibility(true);
				if (mainCode.isGrounded)
				{
					mainCode.GroundMoviment();
					candowndash = true;
					downdash = false;
					if(!mainCode.landed)
					{
						mainCode.MakeDust();
						mainCode.landed = true;
					}
				}
				else
				{	
					mainCode.landed = false;
					mainCode.AirMoviment();
				}
				Animate();

				if(!downdash)
				{
					mainCode.GetControlInput();

					atk2 = Input.GetButtonDown("Fire2");

					mainCode.isinMov = (mainCode.axis*mainCode.rigb.velocity.x != 0);

					if(mainCode.axis != 0.0f)
					{	
						if(Mathf.Abs(mainCode.axis) == 1.0f)
						{
							mainCode.movSen = (int) mainCode.axis;
						}	
					}	
				}
			}
		}
		else 
		{
			mainCode.lifeBar.ToggleVisibility(false);	
			if(mainCode.life <= 0 && !mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("Dying"))
			{
				mainCode.anim.Play("Die");
				mainCode.lifeBar.ToggleVisibility(false);

			}	
		} 
    }

    ///para esse código funcionar, ainda precisa-se mudar uma pá de coisa no MasterController.cs que estão como "Private" e não dá pra acessar;
    //e trazer o void Animate(){} também pra cá.

	public void Animate()
	{	
		if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("takedmg"))
		{		if(mainCode.rigb.velocity.y < -5)
				{
					yfly = -5;
				}
				else if(mainCode.rigb.velocity.y > 5)
				{
					yfly = 5;
				}
				else
				{
					yfly = 0;
				}
				mainCode.anim.SetFloat("AirmovV", yfly);
				if (mainCode.isGrounded) 
				{	
					if(mainCode.isRoll)
					{
						mainCode.anim.Play("Roll");
						mainCode.isCrouch = mainCode.isRoll;
						mainCode.MakeDust();
					}
				}
			if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{

				if (mainCode.isGrounded) 
				{	
					if(!mainCode.isRoll)
					{
						if(mainCode.isCrouch)
						{
							mainCode.anim.Play("Crouch");
							mainCode.anim.SetTrigger("Crouch");
						}

						else if (mainCode.isinMov)
						{
							mainCode.anim.Play("Run");
						}

						else if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
						{
							mainCode.anim.Play("Idle");
						}
					}
				}
				else
				{
					if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
					{
						if(mainCode.isinMov)
						{
							mainCode.anim.Play("Dash_Jump");
						}
						else
						{
							mainCode.anim.Play("Jump");
						}
					}
				}
			}
			Attack();
		}
		else
		{
			mainCode.gothit = false;
		}	
	}

	public void Attack()
	{		

		if(mainCode.atk)
		{
			if(mainCode.isGrounded)
			{	
				if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("down"))
				{
					mainCode.anim.Play("downtilt");
					mainCode.MakeDust();

					mainCode.knockback = new Vector2(3f*mainCode.movSen, 15f);
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("idle") && TbtwAtk <= 0f)
				{	
					direction = this.gameObject.GetComponent<Transform>().localScale.x;
					mainCode.anim.Play("first");
					TbtwAtk = mainCode.anim.GetCurrentAnimatorStateInfo(0).length/2;
					atkqueue = false;
					Debug.Log("F");
	
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("run"))
				{
					mainCode.anim.Play("dash attack");
					mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
					mainCode.rigb.AddForce(new Vector2(20*mainCode.movSen, 0), ForceMode2D.Impulse);
					mainCode.knockback = new Vector2(15f*mainCode.movSen, 15f);
					
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
				{
					mainCode.anim.SetTrigger("Atk");
					if((mainCode.ayis < 0.0f) && !mainCode.isGrounded && candowndash)
					{
						downdash = true; 
						mainCode.anim.Play("flying down");
					}
				}
			}
			else
			{	
				if(!downdash)
				{
					if(candowndash && mainCode.ayis <= 0.0f)
					{	
						mainCode.anim.Play("flying down");
						downdash = true;
						mainCode.knockback = new Vector2(15f*mainCode.movSen, -15f);
						

					}
					else if (mainCode.ayis > 0.0f)
					{
						mainCode.anim.Play("flying up");
						
						mainCode.knockback = new Vector2(0f, 20f);
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
			mainCode.rigb.velocity = new Vector2(mainCode.movSen * 20.0f, -20.0f);

		}
		if(Mathf.Abs(mainCode.rigb.velocity.x) < 20 && downdash)
		{
			downdash = false;
			mainCode.rigb.velocity = new Vector2(-10*mainCode.movSen, 20.0f);
		}
		mainCode.anim.SetBool("Downattack", downdash);

		if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("first") && mainCode.isGrounded)
		{	
			if(TbtwAtk <= 0 && atkqueue)
			{
			direction = this.gameObject.GetComponent<Transform>().localScale.x;
			mainCode.anim.Play("second");
			atkqueue = false;
			Debug.Log("F");
			}
			else if(mainCode.atk)
			{
				atkqueue = true;
				Debug.Log("V");
			}
		}
	}


	void shot()
	{	
		
		var Object = Instantiate(Shot, ShotLocal.position, ShotLocal.rotation);
		Object.transform.Rotate(0, (direction/Mathf.Abs(direction)-1)*(-90), 0, Space.Self);
	}
	void endatk()
	{	
		if(!atkqueue)
		{
			TbtwAtk = 0.2f;
		}
		
	}
}

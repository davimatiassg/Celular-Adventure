using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrBehavior : MonoBehaviour
{
    [SerializeField] private MasterController mainCode;
    [SerializeField] private AudioInterface a;
    private bool downdash = false;
	private bool candowndash;
	public float downdashtm = 0.3f;
	private float yfly;
	public bool isRoll;
	public float rollcdr;
	public float rolltm;

    void Start()
    {
        mainCode = this.gameObject.GetComponent<MasterController>();
        a = this.gameObject.GetComponent<AudioInterface>();
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
    	if(mainCode.isCrouch && !isRoll)
		{
			mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
		}
		//rolamento

		if(((mainCode.isCrouch == true && mainCode.axis != 0) || mainCode.atk2 && mainCode.isGrounded)&& rolltm >= rollcdr)
		{	
			isRoll = true;
		}

		if(isRoll)
		{
			mainCode.rigb.velocity = new Vector2(mainCode.speed*4*mainCode.movSen, mainCode.rigb.velocity.y);
			mainCode.axis = mainCode.movSen;
			
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
    	if(mainCode.invt == mainCode.invtime)
    	{
    		candowndash = false;
			downdashtm = -0.3f;
    	}
      if(mainCode.playable)
		{
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

				//"se o sentido de movimenta????o atual n??o ?? o que foi definido no ??ltimo frame"
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

			//setando o valor das vari??veis de eixos x (axis) e y (ayis)

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
				AnotationPoint an = this.gameObject.GetComponent<AnotationPoint>();
				if(an != null)
				{
					an.AdcthisNote();
				}
				mainCode.lifeBar.ToggleVisibility(false);

			}	
		} 
    }

    ///para esse c??digo funcionar, ainda precisa-se mudar uma p?? de coisa no MasterController.cs que est??o como "Private" e n??o d?? pra acessar;
    //e trazer o void Animate(){} tamb??m pra c??.

	public void Animate()
	{	
		if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("takedmg"))
		{		
			if(mainCode.rigb.velocity.y < -5)
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

			if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{

				if (mainCode.isGrounded) 
				{	
					if(isRoll)
					{	
						if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
						{
							a.PlaySound("dash");
						}
						mainCode.anim.Play("Roll");
						mainCode.isCrouch = isRoll;

						mainCode.MakeDust();
					}
					else if(mainCode.isCrouch)
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
				else
				{
					if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
					{	
						a.PlaySound("dash");
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
			if(mainCode.gothit)
			{
				a.PlaySound("dmg");
			}
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
					a.PlaySound("atk1");
					mainCode.knockback = new Vector2(3f*mainCode.movSen, 15f);
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
				{	
					a.PlaySound("atk1");
					mainCode.anim.Play("first");
					mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
					mainCode.rigb.AddForce(new Vector2(20*mainCode.movSen, 0), ForceMode2D.Impulse);
					mainCode.knockback = new Vector2(3f*mainCode.movSen, 5f);

					
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("run"))
				{	
					a.PlaySound("atk2");
					mainCode.anim.Play("dash attack");
					mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
					mainCode.rigb.AddForce(new Vector2(20*mainCode.movSen, 0), ForceMode2D.Impulse);
					mainCode.knockback = new Vector2(15f*mainCode.movSen, 15f);
					
				}

				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("first"))
				{	
					a.PlaySound("atk2");
					mainCode.anim.Play("second");
					mainCode.MakeDust();
					mainCode.rigb.AddForce(new Vector2(50*mainCode.movSen, 0), ForceMode2D.Impulse);
					mainCode.knockback = new Vector2(3f, 5f);
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("second"))
				{	
					a.PlaySound("atk3");
					mainCode.anim.Play("third");
					mainCode.rigb.velocity = new Vector2(0, 0);
					mainCode.knockback = new Vector2(15f*mainCode.movSen, 15f);
					
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
				{	
					
					mainCode.anim.SetTrigger("Atk");
					if((mainCode.ayis < 0.0f) && !mainCode.isGrounded && candowndash)
					{	
						a.PlaySound("dash");
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
						a.PlaySound("dash");
						mainCode.anim.Play("flying down");
						downdash = true;
						mainCode.knockback = new Vector2(15f*mainCode.movSen, -15f);
						

					}
					else if (mainCode.ayis > 0.0f)
					{	
						a.PlaySound("atk2");
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
	}
}

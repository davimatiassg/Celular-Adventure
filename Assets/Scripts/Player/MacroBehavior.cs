using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroBehavior : MonoBehaviour
{
    [SerializeField] public MasterController mainCode;

    private int nxatk = 1;
    public GameObject hand;
	private float yfly;
	private bool isDash = false;
	private float direc;
	[SerializeField] private bool aerial = false;

	[SerializeField] public bool grabed;

	[SerializeField] private AudioInterface a;
    void Start()
    {
        mainCode = this.gameObject.GetComponent<MasterController>();
        a = this.gameObject.GetComponent<AudioInterface>();
    }

    // Update is called once per frame
	void Update()
    {	
    	if(mainCode.isCrouch)
		{
			mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
		}
		//rolamento

      	if(mainCode.playable)
		{
			if(!grabed)
			{
				if(isDash)
				{
					dashing();
				}
				mainCode.isGrounded = Physics2D.OverlapCircle(mainCode.flchk.position, mainCode.radius, mainCode.solid);
				if(mainCode.isGrounded && Physics2D.OverlapCircle(mainCode.flchk.position, mainCode.radius, mainCode.solid).isTrigger) mainCode.isGrounded = false;
				
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
					if(mainCode.inWater == 0f)
					{	
						mainCode.flchk.gameObject.SetActive(true);
						mainCode.tpchk.gameObject.SetActive(true);
						if (mainCode.isGrounded)
						{
							mainCode.GroundMoviment();
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
					}
					else
					{
						WaterMoviment();
					}

					Animate();

					if(!isDash)
					{
						mainCode.GetControlInput();
					}
					

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
			else
			{	
				mainCode.rigb.velocity = Vector2.up*mainCode.rigb.velocity.y;
				if(mainCode.axis != 0.0f)
				{	
					if(Mathf.Abs(mainCode.axis) == 1.0f)
					{
						mainCode.movSen = (int) mainCode.axis;
					}	
				}	
				if (mainCode.movSen != mainCode.lstmovSen)
				{	
					mainCode.FliptFr();
					mainCode.lstmovSen = mainCode.movSen;
				}
				isDash = false;
				if(mainCode.isGrounded)
				{	
					mainCode.rigb.velocity = Vector2.zero;
					if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk")) 
					{
						mainCode.axis = 0;
					}
					if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("GrabPunch") && !mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("AirGrab") && grabed)
					{
						mainCode.anim.Play("Grab");
					}
					mainCode.GetControlInput();

					if(mainCode.axis != 0)
					{
						mainCode.knockback = Vector2.right*40*mainCode.movSen + Vector2.up*mainCode.ayis*25;
						mainCode.anim.Play("GrabPunch");
					}

					
				}
				else
				{	
					if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk")) 
					{
						mainCode.axis = 0;
					}
					mainCode.anim.Play("AirGrab");
					mainCode.rigb.velocity += Vector2.down;
					mainCode.GetControlInput();
					mainCode.AirMoviment();

					if(Physics2D.OverlapCircle(mainCode.flchk.position, mainCode.radius, mainCode.solid))
					{	
						a.PlaySound("atk");
						hand.GetComponent<MacroHand>().damageEnemy();
						grabed = false;
						mainCode.anim.Play("Crouch");
					}

				}
			}
		}
		else 
		{
			mainCode.lifeBar.ToggleVisibility(false);	
			if(mainCode.life <= 0 && !mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("Dying"))
			{
				//mainCode.anim.Play("Die");
				mainCode.lifeBar.ToggleVisibility(false);

			}	
		} 
    }

    ///para esse código funcionar, ainda precisa-se mudar uma pá de coisa no MasterController.cs que estão como "Private" e não dá pra acessar;
    //e trazer o void Animate(){} também pra cá.
    public void WaterMoviment()
    {	
    	bool boost = mainCode.InPut.GetButton("Spec") || mainCode.InPut.GetButton("Attack");
    	float spdmult = 2;

    	if(boost)
    	{	
    		mainCode.dustmaker.Play();
    		spdmult = 4f;
    	}
    	mainCode.anim.speed = spdmult/4;
    	Vector2 f = Vector2.zero;
    	
    	if(mainCode.jump)
    	{
    		f += Vector2.up*spdmult*mainCode.speed;
    	}
    	else
    	{
    		f += Vector2.down/5;
    	}

    	mainCode.rigb.velocity = Vector2.MoveTowards(mainCode.rigb.velocity, ((Vector2.right*spdmult*mainCode.axis + Vector2.up*mainCode.ayis)*mainCode.speed + f), Time.deltaTime*mainCode.speed*spdmult*5);

    	if(isDash)
    	{
    		dashed();
    	}
    	mainCode.flchk.gameObject.SetActive(false);
    	mainCode.tpchk.gameObject.SetActive(false);
    }
	public void Animate()
	{	
		if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("takedmg"))
		{	

			if(mainCode.inWater == 0)
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
				if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk") || (aerial && mainCode.isGrounded))
				{

					if (mainCode.isGrounded)
					{	
						aerial = false;
						if(mainCode.isCrouch)
						{
							mainCode.anim.Play("Crouch");
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
						if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
						{
							mainCode.anim.speed = 1;
							mainCode.anim.Play("Jump");
						}
					}
					Attack();
				}
			}
			else
			{	
				if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("unWater") && !mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("takedmg"))
				{
					mainCode.anim.Play("inoutWater");
				}
				
			}
		}
		else
		{
			if(mainCode.gothit)
			{
				a.PlaySound("dmg");
			}
			mainCode.gothit = false;
			grabed = false;
		}
	}
	public void Attack()
	{		
		if(mainCode.atk2)
		{	
			mainCode.anim.Play("DashGrab");
			dashGrab(mainCode.movSen);
		}

		if(mainCode.atk)
		{
			if(mainCode.isGrounded)
			{	
				if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("run"))
				{	
					dashGrab(mainCode.movSen);
					mainCode.anim.Play("AirAtk");
					mainCode.knockback = Vector2.right*40 + Vector2.up*15;
				}
				else
				{	
					isDash = false;
					if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
					{
						if(nxatk == 2)
						{
							nxatk = 1;
						}
						else
						{
							nxatk = 2;
						}
						mainCode.knockback = Vector2.right*25 + Vector2.up*mainCode.ayis*15;
						mainCode.anim.Play("atk" + nxatk);
					}

				}
			}
			else
			{	
				aerial = true;
				mainCode.knockback = Vector2.right*20;
				mainCode.anim.Play("AirAtk");
			}
		}
	}
	public void Advance(float distance = 0.5f)
	{
		mainCode.trs.position += Vector3.right*distance*mainCode.movSen;
	}
	public void dashGrab(float direction)
	{	

		direc = direction;
		isDash = true;
	}
	public void dashing()
	{	
		hand.GetComponent<MacroHand>().SearchEnemies();
		mainCode.axis = direc;
		mainCode.speed = 8;
		mainCode.rigb.velocity = new Vector2 (mainCode.speed*mainCode.movSen*mainCode.maxspeed, 0);
	}
	public void dashed()
	{	
		isDash = false;
		mainCode.speed = 6.5f;
	}
	public void ToogleGrab()
	{
		if(!grabed)
		{
			hand.SetActive(true);
		}
		else
		{
			hand.SetActive(false);
		}	
	}
	public void ToggleGrabOn()
	{
		grabed = !grabed;
	}
	public bool GetGrab()
	{
		return grabed;
	}
}

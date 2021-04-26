using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EosinBehavior : MonoBehaviour
{
    [SerializeField] private MasterController mainCode;
    [SerializeField] private GameObject Shot;
    [SerializeField] private GameObject MeleeShot;
    [SerializeField] private Transform ShotLocal;

    [SerializeField] private AudioInterface a;

   
   	private float TbtwAtk = 0f;
   	private bool atkqueue = false;
   	private float direction;
	private float yfly;
	private bool rangedatk;

    void Start()
    {	
    	a = this.gameObject.GetComponent<AudioInterface>();
        mainCode = this.gameObject.GetComponent<MasterController>();
    }

    // Update is called once per frame
	void Update()
    {	
    	if(mainCode.isCrouch)
    	{
    		mainCode.rigb.velocity = new Vector2(0f, 0f);
    	}	
    	if(TbtwAtk > 0f)
    	{	
    		TbtwAtk -= Time.deltaTime;
    		
    	}
    	else
    	{
    		TbtwAtk = 0f;
    	}
      if(mainCode.playable)
		{
			mainCode.isGrounded = GroundDetect();
			
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
				direction = this.gameObject.GetComponent<Transform>().localScale.x;	
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
			if (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{
				if (mainCode.isGrounded) 
				{	
					if(mainCode.isCrouch && !mainCode.atk && !mainCode.atk2) 
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
						a.PlaySound("jump");
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
		{	if(mainCode.gothit)
			{
				a.PlaySound("dmg");
			}
			mainCode.gothit = false;
		}	
	}

	public void Attack()
	{	

		if(mainCode.atk || mainCode.atk2)
		{
			rangedatk = mainCode.atk && !mainCode.atk2;

			if(mainCode.isGrounded)
			{	
				if(TbtwAtk <= 0f && (!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk")))
				{	
					direction = this.gameObject.GetComponent<Transform>().localScale.x;
					mainCode.anim.Play("first");
					TbtwAtk = mainCode.anim.GetCurrentAnimatorStateInfo(0).length/2;
					atkqueue = false;
					mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
	
				}

			}
			else if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{	
				if(TbtwAtk <= 0 && !atkqueue)
				{
					mainCode.anim.Play("air attack");
					mainCode.knockback = new Vector2(15f*mainCode.movSen, -15f);
				}
				
			}
		}
		if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("first") && mainCode.isGrounded)
		{	
			if(TbtwAtk <= 0 && atkqueue)
			{
				mainCode.anim.Play("second");
				atkqueue = false;
			}
			else if(mainCode.atk || mainCode.atk2)
			{
				atkqueue = true;
			}
		}
	}


	public void selAttack()
	{
		if(rangedatk)
		{	
			a.PlaySound("shot");
			Fire();
		}
		else
		{
			Melee();
		}
	}
	void Fire()
	{
		var Object = Instantiate(Shot, ShotLocal.position, ShotLocal.rotation);
		Object.transform.Rotate(0f, (-Mathf.Sign(direction)*(90) + 90), mainCode.ayis*(45f), Space.Self);
	}
	private void Melee()
	{
		var Object = Instantiate(MeleeShot, ShotLocal.position, ShotLocal.rotation);
		Object.transform.Rotate(0f, (-Mathf.Sign(direction)*(90) + 90), mainCode.ayis*(45f), Space.Self);
	}
	private void endatk()
	{	
		if(!atkqueue)
		{
			TbtwAtk = 0.3f;
		}
	}
	public bool GroundDetect()
	{	
		RaycastHit2D hit = Physics2D.Raycast(mainCode.flchk.position, Vector2.down, mainCode.radius, mainCode.solid);

		if(hit)
		{
			if(Vector2.Angle(Vector2.up, hit.normal) < 50)
			{
				return true;
			}
		}
		return false;
	}
}

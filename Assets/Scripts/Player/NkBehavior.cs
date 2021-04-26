using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NkBehavior : MonoBehaviour
{
    [SerializeField] private MasterController mainCode;

    [SerializeField] private AudioInterface a;

	private float yfly;
	public bool isMeteor = false;
	public GameObject meteorStrike;
	public GameObject Tp;
	public float teleportRange = 5f;
	public float tpCd = 0.5f;
	private float tptm = 0f;
	private bool candash = true;
	private GameObject ext_portal;
	public TrailRenderer trail;
	public TrailRenderer swordtrail;

    void Start()
    {
        mainCode = this.gameObject.GetComponent<MasterController>();
        mainCode.knockback = Vector2.zero;
        a = this.gameObject.GetComponent<AudioInterface>();
    }

    // Update is called once per frame
	void Update()
    {	
    	if(ext_portal != null)
    	{
    		ext_portal.GetComponent<Transform>().position = mainCode.trs.position;
    	}
    	else
    	{
    		trail.emitting = false;
    	}
    	if(tptm > 0)
    	{
    		tptm -= Time.deltaTime;
    	}
    	if(mainCode.isCrouch )
		{
			mainCode.rigb.velocity = new Vector2(0, mainCode.rigb.velocity.y);
		}
		//rolamento

      if(mainCode.playable)
		{
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
				if (mainCode.isGrounded)
				{
					mainCode.GroundMoviment();
					if(!mainCode.landed)
					{
						mainCode.MakeDust();
						mainCode.landed = true;
						candash = true;
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
				//mainCode.anim.Play("Die");
				mainCode.lifeBar.ToggleVisibility(false);

			}	
		} 
    }

    ///para esse código funcionar, ainda precisa-se mudar uma pá de coisa no MasterController.cs que estão como "Private" e não dá pra acessar;
    //e trazer o void Animate(){} também pra cá.

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
			if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
			{
				if (mainCode.isGrounded)
				{
					if(!isMeteor)
					{	
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
						mainCode.anim.Play("downstab");
					}
				}
				else
				{
					if(!mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("air"))
					{
						mainCode.anim.Play("Jump");
						a.PlaySound("jump");
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

	public void meteor(int charge = 0)
	{	
		if (charge != 2)
		{	
			isMeteor = true;
			if(charge == 0)
			{	
				mainCode.rigb.velocity = Vector2.zero;
			}
			else
			{
				mainCode.gScale = 6f;
			}	
		}
		else
		{
			mainCode.gScale = 2f;
			isMeteor = false;
		}
	}
	public void Attack()
	{		
		if(mainCode.atk2 && tptm <= 0 && candash)
		{
			
			teleport();
			if(!mainCode.isGrounded)
			{
				candash = false;
			}
		}
		if(mainCode.atk)
		{
			if(mainCode.isGrounded)
			{	
				if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("down"))
				{	
					a.PlaySound("atk2");
					mainCode.anim.Play("third");
					mainCode.MakeDust();
					
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("run") || mainCode.anim.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
				{	a.PlaySound("atk1");
					if(mainCode.ayis <= 0)
					{	
						mainCode.anim.Play("first");
					}
					else
					{
						mainCode.anim.Play("second");
					}
					
					mainCode.rigb.velocity = Vector2.zero;
				}

				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("first"))
				{	
					a.PlaySound("atk1");
					mainCode.anim.Play("second");
				}
				else if(mainCode.anim.GetCurrentAnimatorStateInfo(0).IsName("second"))
				{	a.PlaySound("atk2");
					mainCode.anim.Play("third");
				}
			}
			else if(!isMeteor)
			{	
				if(mainCode.ayis < 0.0f)
				{	
					mainCode.anim.Play("chargedownair");
				}
				else
				{
					mainCode.anim.Play("airspin");
				}

			}
		}
	}
	public void teleport()
	{	
		a.PlaySound("dash");
		trail.emitting = true;
		Vector2 startp = mainCode.trs.position;
		trail.AddPosition(mainCode.trs.position);
		mainCode.trs.position += Vector3.right * mainCode.movSen * 1f;
		
		tptm = tpCd;
		Vector2 dir = new Vector2(Mathf.RoundToInt(mainCode.axis), Mathf.RoundToInt(mainCode.ayis));
		dir.Normalize();
		Vector2 siz = this.gameObject.GetComponent<CapsuleCollider2D>().size;
		Vector2 ofs = this.gameObject.GetComponent<CapsuleCollider2D>().offset;

		Vector2 final;
		GameObject in_portal = Instantiate(Tp, mainCode.trs.position, mainCode.trs.rotation);
		in_portal.GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.4f, 0.33f, 1);
		CapsuleCollider2D col = this.gameObject.GetComponent<CapsuleCollider2D>();
		List<RaycastHit2D> touchingpoints = new List<RaycastHit2D>();

		ContactFilter2D c = new ContactFilter2D();

		c.SetLayerMask(mainCode.solid);

		int dest = col.Raycast(dir, c, touchingpoints, teleportRange);

	
		
		if(dest != 0)
		{	

			Vector2 v = col.bounds.center;
			final = touchingpoints[0].point - (col.ClosestPoint(touchingpoints[0].point) - v);
		}
		else
		{	

			if(dir == Vector2.zero)
			{
				final = (Vector2) mainCode.trs.position +  new Vector2(teleportRange * mainCode.movSen, 0f);
				mainCode.rigb.velocity = Vector2.right * mainCode.movSen * 3f;
				trail.AddPosition(final);
			}
			else
			{
				final = (Vector2) mainCode.trs.position + (dir * teleportRange);
			}
		}


		Debug.DrawLine(mainCode.trs.position, final, Color.red, 5);
		ext_portal = Instantiate(Tp, final, mainCode.trs.rotation);
		ext_portal.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.57f, 1);

		mainCode.flytime = 0;
		mainCode.trs.position = final;
		trail.AddPosition(final);

		if(swordtrail.emitting)
		{	
			foreach(RaycastHit2D targ in touchingpoints)
			{	
				if(targ)
				{
					if(targ.collider.gameObject.tag.Equals("hitable"))
					{
						var hited = targ.transform.gameObject.GetComponent<CombatEnemy>();
						hited.takedamage(mainCode.attackdmg, mainCode.knockback);

					}
					if(targ.collider.gameObject.tag.Equals("Boss"))
					{
						var hited = targ.collider.gameObject.GetComponent<HitableParts>();
						if(hited)
						{
							hited.takedamage(mainCode.attackdmg);
						}
						else
						{
							var bhited = targ.collider.gameObject.GetComponent<InfectedNKBehavior>();
							bhited.bossCore.takedamage(mainCode.attackdmg);
						}
						
					}
				}

			}
		}
		//trail.emitting = false;

	}
	public void downsmash(int phase = 0)
	{
		if(phase == 0)
		{	

			List<ContactPoint2D> touchingpoints = new List<ContactPoint2D>();

			this.gameObject.GetComponent<CapsuleCollider2D>().GetContacts(touchingpoints);
			if(touchingpoints.Count == 0)
			{
				Instantiate(meteorStrike, mainCode.trs.position+Vector3.down*1.5f, mainCode.trs.rotation);
			}
			else
			{
				foreach(ContactPoint2D c in touchingpoints)
				{	

					if(c.normal == Vector2.up)
					{
						Instantiate(meteorStrike, c.point, mainCode.trs.rotation);
						break;
					}
				}
			}
			
			
			mainCode.playable = false;
		}
		else
		{
			mainCode.playable = true;
		}
	}
}

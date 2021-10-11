using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.Events;
using TMPro;

public class SummaryButtons : MonoBehaviour
{

	public GameObject BestButton;
  public GameObject AnButton;

    void Awake()
    {
    	GameEvents.StartListening("GamePaused", ToggleButtons);
    }
  	void OnDisable()
  	{
  		GameEvents.StopListening("GamePaused", ToggleButtons);
  	}


    public void ToggleButtons()
    {	
    	if(BestiaryElements.Bestiary.Count <= 0)
    	{
    		BestButton.GetComponent<Button>().interactable = false;  	 	
   		}
   		else
   		{
   			BestButton.GetComponent<Button>().interactable = true;
   		}

      if(AnotationManager.Notes.Count <= 0)
      {
        AnButton.GetComponent<Button>().interactable = false;     
      }
      else
      {
        AnButton.GetComponent<Button>().interactable = true;
      }

    }
}




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




public class TutorialBehavior : MonoBehaviour
{
  [SerializeField] private SpriteRenderer hint;

  [SerializeField] private Sprite hintimg;

  [SerializeField] private SpriteRenderer hint2;

  [SerializeField] private Sprite hintimg2;

  private Animator anim;

  void OnEnable()
  { 
    anim = this.gameObject.GetComponent<Animator>();
    anim.speed = 1;
    anim.Play("TutorialEnter");
  }

  public void Vanish()
  {
    this.gameObject.SetActive(false);

  }

  public void CloseTutorial()
  { 
    anim.Play("TutorialExit");
  }

  public void AddTutorialImages(Sprite img1)
  {
    hint.sprite = img1;
    hintimg = img1;
  }
  public void AddTutorialImages(Sprite img1, Sprite img2)
  {
    hint.sprite = img1;
    hintimg = img1;
    hint2.sprite = img2;
    hintimg2 = img2;      
  }
}




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
    {   if(mainCode.rigb.velocity.y < -5)
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
    { if(mainCode.gothit)
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
        { a.PlaySound("atk1");
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
        { a.PlaySound("atk2");
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


    c.SetLayerMask(LayerMask.GetMask("solid"));

    if(dir == Vector2.zero)
    {
      dir = mainCode.movSen*Vector2.right;
    }

    int dest = col.Raycast(dir, c, touchingpoints, teleportRange);

    

    if(dest > 0)
    { 
      Debug.Log(c.IsFilteringLayerMask(touchingpoints[0].transform.gameObject));
      final = touchingpoints[0].point - dir;
    }
    else
    { 

      final = (Vector2) mainCode.trs.position + (dir * teleportRange);
          
    }
    trail.AddPosition(final); 


    Debug.DrawLine(mainCode.trs.position, final, Color.red, 5);
    ext_portal = Instantiate(Tp, final, mainCode.trs.rotation);
    ext_portal.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.57f, 1);

    mainCode.flytime = 0;
    mainCode.trs.position = final;
    trail.AddPosition(final);

    if(swordtrail.emitting)
    { 
      c.SetLayerMask(LayerMask.GetMask("hitable"));
      col.Raycast(-dir, c, touchingpoints, teleportRange);
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




public class MacroHand : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private MacroBehavior mainCode; 
    public GameObject grabedenemy;
    // Update is called once per frame
    void Update()
    {
        if(mainCode.GetGrab())
        {
          GrabEnemy(grabedenemy);
        }
        else
        { 
          if(grabedenemy)
          { 
            grabedenemy.GetComponent<CombatEnemy>().SetStuned(0.2f);
            grabedenemy = null;
          }
          
        }
    }

    public void SearchEnemies()
    { 
    float distanceToClosestEnemy = radius;
    GameObject closestEnemy = null;
    GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("hitable");

    foreach (GameObject currentEnemy in allEnemies)
    { 
      float distanceToEnemy = new Vector2(Mathf.Abs(currentEnemy.transform.position.x - this.transform.position.x),Mathf.Abs(currentEnemy.transform.position.y - this.transform.position.y)).magnitude;
      
      if (distanceToEnemy < distanceToClosestEnemy) 
      { 
        distanceToClosestEnemy = distanceToEnemy;
        closestEnemy = currentEnemy;
      }
    }
    if(closestEnemy)
    { 

      GrabEnemy(closestEnemy);
      mainCode.ToggleGrabOn();
      
    }
    }

    public void SearchEnemies(GameObject enemy)
    {   
        GrabEnemy(enemy);
        mainCode.ToggleGrabOn();

    }
    public void GrabEnemy(GameObject enemy)
    { 
      enemy.GetComponent<CombatEnemy>().SetStuned(0.5f);
      grabedenemy = enemy;
      enemy.transform.position = this.transform.position;
    }

    public void damageEnemy()
    {
      grabedenemy.GetComponent<CombatEnemy>().takedamage(mainCode.mainCode.attackdmg, Vector2.up*20 + mainCode.mainCode.movSen*Vector2.right*40);
      grabedenemy = null;
    }

    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, radius);
  }

}






[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour, AudioInterface
{
    [SerializeField] private AudioSource aud;

    public string thistag;

    [SerializeField]
    public List<AudioClip> clip = new List<AudioClip>();

    public List<string> cname = new List<string>();

    public Dictionary<string, AudioClip> clipstoPlay = new Dictionary<string, AudioClip>();
    
    void OnEnable()
    {
        GameEvents.StartListening("changevolume", getVolume);
    foreach(AudioClip c in clip)
        {
            if(!clipstoPlay.ContainsKey(cname[clip.IndexOf(c)]))
            {
                clipstoPlay.Add(cname[clip.IndexOf(c)], c);
            }
          
        }
    }
    void OnDisable()
    {
        GameEvents.StopListening("changevolume", getVolume);
    }

    void Start()
    {
        PauseMenu.musicController = this;
        aud = this.gameObject.GetComponent<AudioSource>();
        aud.enabled = true;
        aud.volume = AudioController.GetSoundVol(thistag);


    }



    // Update is called once per frame
    public void UnPause()
    {
        aud.UnPause();
        getVolume();
    }
    
    public void Pause()
    {
        aud.Pause();
    }


    public void PlaySound(string clipName)
    { 
      getVolume();
      aud.Stop();
      aud.clip = clipstoPlay[clipName];
      aud.Play();
    }

    public void PlaySound(AudioClip clip)
    { 
      getVolume();
      aud.clip = clip;
        if(!clipstoPlay.ContainsKey(clip.name))
        {
            clipstoPlay.Add(clip.name, clip);
        }
      aud.Play();

    }

    public void getVolume()
    {
        aud.volume = AudioController.GetSoundVol(thistag);
    }
}




public class ShootBehavior : MonoBehaviour {

  //declaração das variáveis da inst
  public int dmg;
  public float speed;
  private Rigidbody2D rigb;
  public GameObject HitEffect;
  public Vector2 knockback = new Vector2(0f, 0f);

  public bool destroyOnContact = true;

  public bool enemie = true;

  public float lifetime = 5;

  // Use this for initialization
  void Start () 
  { 
    if(this.gameObject.GetComponent<Rigidbody2D>() != null)
    {
      rigb = GetComponent<Rigidbody2D>();
      rigb.velocity = transform.right * speed;
    }
    
    
  }

  void Update () 
  { 
    if(this.gameObject.GetComponent<Rigidbody2D>() != null)
    {
      rigb.velocity = transform.right * speed;
    }
    if (!enemie)
    {
      Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), GameObject.FindWithTag("Player").GetComponent<Collider2D>(), true);
    }

    if(lifetime > 0)
    {
      lifetime -= Time.deltaTime;
    }
    else
    {
      Destroy(this.gameObject);
    }
  }
  
  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player") && enemie)
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(dmg);
      if(destroyOnContact)
      {
        Destroy(gameObject);
      }
      
    }
    else if(other.gameObject.tag.Equals("hitable") && !enemie)
    { 
      var hited = other.gameObject.GetComponent<CombatEnemy>();
      hited.takedamage(dmg, new Vector2(Mathf.Sign(transform.right.x)*knockback.x, knockback.y*transform.right.y));

      if(speed != 0f)
      { 
        Destroy(gameObject);

      }
      
      Instantiate(HitEffect);
    }
    else if(other.gameObject.tag.Equals("Boss") && !enemie)
    {
      var hited = other.gameObject.GetComponent<BossCore>();
      if(!hited)
      {
        other.gameObject.GetComponent<HitableParts>();
      }
      hited.takedamage(dmg);

      if(speed != 0f)
      { 
        Destroy(gameObject);

      }
      
      Instantiate(HitEffect);
    }
  }

  private void OnCollisionEnter2D(Collision2D other)
  { 
    if(!other.gameObject.tag.Equals("Player"))
    {
      Instantiate(HitEffect);
      Destroy(this.gameObject);
    }
  }
  public void end()
  {
    Destroy(gameObject);
  }
}




public class TrypShoalBehavior : MonoBehaviour
{ 
    [SerializeField] private BossCore core;
  [SerializeField] private float atkCoolDown;
  [SerializeField] private float cd;
  private Animator anim;
  private Transform trs;
  private AnimatorStateInfo currentAnimation;
  [SerializeField] private List<GameObject> piranhas = new List<GameObject>();
  public Vector2 destination;
  private Vector2 StartPos;
  [SerializeField] private float mainSpeed = 45;
  [SerializeField] private float range;
  [SerializeField] private float speed;

  [SerializeField] private GameObject LazerBeam;

    [SerializeField] private ParticleSystem particles;


  private Transform Player;
    // Start is called before the first frame update
    void OnEnable()
    {
        GameEvents.StartListening("BossDie", Die);
    }
    void OnDisable()
    {
        GameEvents.StopListening("BossDie", Die);
    }
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        trs = this.gameObject.GetComponent<Transform>();
        StartPos = trs.position;
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        speed = mainSpeed;
        core = this.gameObject.GetComponent<BossCore>();
        anim.Play("Entry");
        cd = atkCoolDown;
    }

    // Update is called once per frame
    void Update()
    { 
      currentAnimation = anim.GetCurrentAnimatorStateInfo(0);
        
        if(!currentAnimation.IsTag("Cutscene") && core.life > 0)
        {
            if(currentAnimation.IsTag("Idle"))
            {   
                destination = StartPos + (range*Mathf.Sin(Time.time))*Vector2.right;
                if(cd > 0)
                {
                    cd -= Time.deltaTime;
                }
                else
                {
                    Attack();
                }
            }
            else if(currentAnimation.IsTag("atk"))
            {
                
                if(currentAnimation.IsName("atk1"))
                {
                    destination = StartPos + (range*Mathf.Sin(Time.time))*Vector2.right + Vector2.up*10;
                }
            }
            trs.position = Vector2.MoveTowards(trs.position, destination, speed*Time.deltaTime);
        }
        else if(core.life <= 0)
        {
            Die();
        }

    }

    public void Attack()
    {
      anim.Play("atk"+(Random.Range(0, 3)+1));
    }

    public void ReturnIdle()
    {
      anim.Play("Idle");
      cd = atkCoolDown;
      trs.localScale = Mathf.Abs(trs.localScale.x)*Vector2.one;
      speed = mainSpeed;
    }

    private float FindOpositeSide()
    {
      return Mathf.Sign(StartPos.x-Player.position.x);
    }

    public void AxeAttack()
    { 
      float u = FindOpositeSide();
      destination = StartPos + 10*Vector2.up + 15*Vector2.right*u;
      trs.localScale = Vector2.up*Mathf.Abs(trs.localScale.x)*-u + Vector2.right*Mathf.Abs(trs.localScale.x)*-u;

    }
    public void TornadoAttack()
    {
      speed = speed*3/2;
    }
    public void LazerAttack()
    {
      float u = FindOpositeSide();
      destination = StartPos + 11.5f*Vector2.up + 15*Vector2.right*u;
      trs.localScale = Vector2.up*Mathf.Abs(trs.localScale.x) + Vector2.right*Mathf.Abs(trs.localScale.x)*-u;
    }
    public void SpawnLazer()
    {
      GameObject lazer = Instantiate(LazerBeam, piranhas[6].GetComponent<Transform>().position, trs.rotation);
      float x = -Mathf.Sign(trs.localScale.x);
      Debug.Log(x);
      lazer.GetComponent<Transform>().eulerAngles = Vector2.up*(90 + x*90); 
    }

    public void Die()
    {   
        particles.Stop();
        GameObject.FindWithTag("Player").GetComponent<MasterController>().TogglePlayable(false);
        anim.Play("Die");

    }

    public void Dead()
    {      
        GameEvents.ScreamEvent("FinalBossIsDead");
        Destroy(this.gameObject);
        
    }


}




public class PiranhaBehavior : MonoBehaviour
{ 
  public BossCore bossCore;
  public int attackdmg;
  private SpriteRenderer spr;
  private CircleCollider2D cColider;
    private Transform trs;

    [SerializeField] private Color atkcolor = new Color(255, 227, 0, 255)/255;
    [SerializeField] private Color ncolor = new Color(0, 241, 255, 255)/255;

  public bool isAtkMode;

    void Start()
    {
        spr = this.GetComponent<SpriteRenderer>();
        trs = this.GetComponent<Transform>();
        cColider = this.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isAtkMode)
        {
          spr.material.color = atkcolor;
          cColider.isTrigger = true;
            

        }
        else
        {
          spr.material.color = ncolor;
          cColider.isTrigger = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.tag.Equals("Player") && !other.isTrigger)
    { 
      
      var hited = other.gameObject.GetComponent<MasterController>();
        
      if(hited)
      {
        hited.takedamage(attackdmg);
      }
    }
    }

}




public class PontuationCounter : MonoBehaviour
{

    [SerializeField] private static int score;


    void OnEnable()
    {
        GameEvents.StartListening("FadeOut", SaveScore);
    }
    void OnDisable()
    {
        GameEvents.StopListening("FadeOut", SaveScore);
    }


    public void Start()
    {
        if(score <= 0 && PlayerPrefs.HasKey("score"))
        {
            score = PlayerPrefs.GetInt("score");
        }
    }
    public static void AddScore(int s)
    { 

        score += s;
        GameEvents.ScreamEvent("ScoreUp");
       
    }

    public static int GetScore()
    {
      return score;
    }

    public static string GetScoreString()
    { 


      return "Pontuação: " + score; 
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("score", GetScore());
        PlayerPrefs.Save();
    }
}




public class PannelElements : MonoBehaviour
{
    private RectTransform trs;
    private Transform trs2;


    [SerializeField] private Vector2 pos = Vector2.zero;

    [SerializeField] private Vector2 sc = Vector2.zero;

    [SerializeField] private bool basedOnCamSize = false;
    [SerializeField] private bool texted = false;

    private Camera c;



    void Start()
    {
      trs = this.gameObject.GetComponent<RectTransform>(); 
        trs2 = this.gameObject.GetComponent<Transform>();
        c = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {   


      if(basedOnCamSize)
        {
            trs2.localScale = sc*c.orthographicSize*5/9; 
        }
        else
        {
            trs.anchoredPosition = pos*Screen.width;
            if(texted)
            {
                trs.localScale = sc*Screen.width;
            }
            else
            {
                trs.sizeDelta = sc*Screen.width;
            }
                     
        }       
    }
}






public class Icon : MonoBehaviour
{ 
  public GameObject Selecter;
  public Sprite SplashScreen;
    public Sprite Player;
  [SerializeField] public UnityEngine.UI.Image Screen;
  public Text label;
    [TextArea] 
  public string text;
    [TextArea] 
    public string ptext;
  public GameObject Pannel;
  public string scene;
  public MainMenu startb;
  [SerializeField] private bool isselected;
    [SerializeField] private bool selectable = true;
    [SerializeField] private int level;
    // Start is called before the first frame update
    void Start()
    {
        
        int l = 0;
        SpriteRenderer s= this.gameObject.GetComponent<SpriteRenderer>();
        if(PlayerPrefs.HasKey("level"))
        {
          l = PlayerPrefs.GetInt("level");
        }
        else
        {
            PlayerPrefs.SetInt("level", 0);
        }
        if(l < level -1)
        {
            selectable = false;
            s.color = new Color(1, 0, 0, 0.3f); 
        }
        else
        {
            selectable = true;
            s.color = new Color(1, 1, 1, 1f); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !Pannel.activeSelf && isselected)
        {
          Screen.sprite = SplashScreen;
          label.text = text;
          startb.scenename = scene;
          Pannel.SetActive(true);
            GameEvents.StartListening("ContentUpdate", ToggleContent);

        }
        else if(Input.GetMouseButtonDown(0) && !Pannel.activeSelf)
        {
            GameEvents.StopListening("ContentUpdate", ToggleContent);
        }
    }
    void OnMouseEnter()
    {   
        if(selectable)
        {
            Selecter.GetComponent<Transform>().position = this.gameObject.transform.position;
            Selecter.GetComponent<Cursor>().Attached = true;
            isselected = true;
        }


        
    }
    void OnMouseExit()
    {
        if(selectable)
        {
          Selecter.GetComponent<Cursor>().Attached = false;
          isselected = false;
        }
    }

    public void ToggleContent()
    {
        if(label.text == text)
        {
            Screen.sprite = Player;
            label.text = ptext;
        }
        else
        {
            Screen.sprite = SplashScreen;
            label.text = text;
        }
        
    }
}




public class SaveFileShow : MonoBehaviour
{
    [SerializeField] private int p = 0;
    [SerializeField] private int l = 0;
    [SerializeField] private float t = 0;

    void Start()
    {
        GetFiles();
        this.gameObject.GetComponent<Text>().text = FormulateString();
    }

    // Update is called once per frame
    public void GetFiles()
    {
      if(PlayerPrefs.HasKey("level"))
      {
        l = PlayerPrefs.GetInt("level");
      }
      if(PlayerPrefs.HasKey("score"))
      {
        p = PlayerPrefs.GetInt("score");
      }
      if(PlayerPrefs.HasKey("playtime"))
      {
        t = PlayerPrefs.GetFloat("playtime");
      }   
    }
    
    public string FormulateString()
    {
      string time = (Mathf.RoundToInt(t/3600) + "H, " + (Mathf.RoundToInt(t%3600)/60)) + "m, e " + (Mathf.RoundToInt(t%3600%60)) + "s";
      string m_time = Mathf.RoundToInt(t/60) + " Minutos";
      string str = p + "\r\n \r\n" + l + "\r\n \r\n" + m_time + "\r\n";
      return str;
    }

    public void DeleteAllFiles()
    {
      p = 0;
      t = 0;
      l = 0;
      this.gameObject.GetComponent<Text>().text = FormulateString();
    PlayerPrefs.SetFloat("playtime", 0);
    PlayerPrefs.SetInt("level", 0);
    PlayerPrefs.SetInt("score", 0);

    }
}




public class Teleporter : MonoBehaviour
{
    [SerializeField] private bool interactable;

    [SerializeField] private Vector3 exit;

    [SerializeField] private Vector3 dest = Vector3.zero;

    [SerializeField] private GameObject exitObj;

    [SerializeField] private Collider2D Player;

    [SerializeField] private Collider2D ThisCol; 

    [SerializeField] private int framecount;

    public InputManager InPut;

    void OnEnable()
    {
      GameEvents.StartListening("CheckTP", CheckTP);
    }

    void OnDisable()
    {
      GameEvents.StopListening("CheckTP", CheckTP);
    }

    void Start()
    {
      if(exitObj)
      {
        ThisCol = this.gameObject.GetComponent<Collider2D>();
        Player = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
        exit = exitObj.transform.position + exit;
        InPut = InputManager.instance;
    }
    }
  

  void Update()
  {
    if(dest != Vector3.zero)
    {   
      if(framecount == 0)
      {

        Player.transform.position = dest;
        dest = Vector3.zero;
      }
      else if(framecount > 0)
      {
        framecount --;
      }
    }

    
    if(InPut.GetButtonDown("Attack"))
    { 
      GameEvents.ScreamEvent("CheckTP");

    }
  }

  public void CheckTP()
  {
    if(ThisCol.Distance(Player).isOverlapped)
    {
      framecount = 5;
      dest = exit + Player.transform.position.z*Vector3.forward;
      
    }
  }

}





public class FlyingNote : MonoBehaviour
{ 
  [SerializeField] private Collider2D thiscol;

  [SerializeField] private GameObject border;

  [SerializeField] private GameObject Line;

  private float loadTime = 1;

  private Camera mainc;

  private SpriteRenderer borderspr; 

 public bool isDrag;

 public bool isSelected;

 public bool isLoading;

 public bool Locked;

  private Vector2 lastMousePos;

  [SerializeField] private GameObject Ligation;

  void OnEnable()
  {
    GameEvents.StartListening("ClosedTable", Vanish);
    GameEvents.StartListening("InvisibleNotes", ToogleVisible);
  }
  void OnDisable()
  {
    GameEvents.StopListening("ClosedTable", Vanish);
    GameEvents.StopListening("InvisibleNotes", ToogleVisible);
  }
  void Start()
  {
    mainc = Camera.main;
    borderspr = border.GetComponent<SpriteRenderer>();
  }
  void FixedUpdate()
  {
    Vector2 mousep = mainc.ScreenToWorldPoint(Input.mousePosition);
    if(!Locked)
    {

      if(Input.GetMouseButton(0))
      {
        Collider2D[] cols = Physics2D.OverlapPointAll(mousep, Physics2D.DefaultRaycastLayers, -1, 1);
        if(cols.Length > 0)
        { 

          if(cols[0] == thiscol)
          { 

            isDrag = true;
            isSelected = true;
            if(Vector2.Distance(mousep, lastMousePos) < 0.1f)
            {
              CountDown();
            }
            else
            {
              isLoading = false;
            }
          }
          else
          { 
            isSelected = false;
            isLoading = false;
          }

        }
        else
        { 

          isSelected = false;
          isLoading = false;
        }


        
      }
      else
      {
        isDrag = false;
        isLoading = false;
        border.SetActive(false);
      }

      if(!isLoading)
      { 
        loadTime = 1;
        if(isDrag)
        { 
          border.SetActive(true);
          borderspr.color = Color.yellow;
          this.transform.position = mousep;
        }
        else if(isSelected)
        {
          border.SetActive(true);
          borderspr.color = Color.green;
        }   
      }
    }
    else
    { 
      Collider2D[] cols = Physics2D.OverlapPointAll(mousep, Physics2D.DefaultRaycastLayers, -1, 1);
      if(isSelected)
      {
        
        if(Input.GetMouseButton(0))
        {
          if(cols.Length > 0)
          {
            if(cols[0] != thiscol)
            { 
              Lock(cols[0].gameObject);
            }

          }
          else
          {
            Unlock();
          }
        }
      }
      else
      {
        if(Input.GetMouseButton(0))
        {
          if(cols.Length > 0)
          {
            if(cols[0] == thiscol && Vector2.Distance(mousep, lastMousePos) < 0.1f)
            { 
              CountUp();
            }
            else
            { 
              borderspr.color = Color.blue;
              loadTime = 1;
            }

          }
        }
        else
        { 
          borderspr.color = Color.blue;
          loadTime = 1;
        }
      }
      
    }
    lastMousePos = mousep;
  }

  private void CountDown()
  {
    isLoading = true;
    loadTime -= Time.fixedDeltaTime;
    if(loadTime <= 0f)
    {
      Load();
    }
    borderspr.color = Color.Lerp(borderspr.color, Color.blue, (1 - loadTime));
  }
  private void CountUp()
  {
    loadTime -= Time.fixedDeltaTime;
    if(loadTime <= 0f)
    {
      Unlock();
    }
    borderspr.color = Color.Lerp(borderspr.color, Color.red, (1 - loadTime));
  }
  private void Load()
  {
    isSelected = true;
    isLoading = false;
    isDrag = false;
    Locked = true;
    border.SetActive(true);
    borderspr.color = Color.blue;

    Line = Instantiate(Ligation, this.gameObject.transform.parent);

    Line.GetComponent<NoteLigator>().Ob1 = this.gameObject;

  }

  public void Lock(GameObject otherCol)
  { 
    if(otherCol.GetComponent<PhisicNote>().isText != this.gameObject.GetComponent<PhisicNote>().isText)
    {
      otherCol.GetComponent<FlyingNote>().GetLocked();
      otherCol.GetComponent<FlyingNote>().Line = Line;
      Line.GetComponent<NoteLigator>().Ob2 = otherCol;
      isSelected = false;
      borderspr.color = Color.blue;
      loadTime = 1;   
    }

  }

  public void GetLocked()
  { 
    isSelected = false;
    isLoading = false;
    isDrag = false;
    Locked = true;
    border.SetActive(true);
    borderspr.color = Color.blue;
  }

  public void Unlock()
  { 
    loadTime = 1;
    NoteLigator LNote= Line.GetComponent<NoteLigator>();
    isSelected = false;
    if(LNote.Ob2)
    {
      LNote.Ob2.GetComponent<FlyingNote>().Locked = false;
    }
    
    if(LNote.Ob1)
    {
      LNote.Ob1.GetComponent<FlyingNote>().Locked = false;
    }

    Destroy(Line);

  }

  public void ToogleVisible()
  {
    this.gameObject.GetComponent<SpriteRenderer>().enabled = !this.gameObject.GetComponent<SpriteRenderer>().enabled;
    this.gameObject.GetComponent<Collider2D>().enabled = this.gameObject.GetComponent<SpriteRenderer>().enabled;
    for(int i = 0; i < this.gameObject.transform.childCount ; i ++)
    {
      GameObject f = this.gameObject.transform.GetChild(i).gameObject;
      f.SetActive(!f.activeSelf); 
    }
  }

  void Vanish()
  {
    Destroy(this.gameObject);
  }

}




public class Cursor : MonoBehaviour
{

  private Transform trs;
  private Vector3 mouse;
  public CameraBehavior cam;
  [SerializeField] private GameObject Pannel;

  public bool Attached = false;

  public bool LvlSelec = true;


  void OnEnable()
  { 
    GameEvents.StartListening("ToogleCursor", ToggleVisible);
  }
  void OnDisable()
  {
    GameEvents.StopListening("ToogleCursor", ToggleVisible);
  }
  void Start()
  {
    trs = this.gameObject.GetComponent<Transform>();
    if(LvlSelec)
    {
      CameraBehavior.target = trs;
      ToggleVisible();
    }

    
    
  }

  void Update()
  { 
    if(LvlSelec)
    {
      if(!Attached && !Pannel.activeSelf)
      { 
        mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        trs.position = Camera.main.ScreenToWorldPoint(mouse);
      }
      else if(Pannel.activeSelf)
      {
        UnityEngine.Cursor.visible = true;
      }

      cam.enabled = !Attached;
    }
    else
    {
      mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
      trs.position = Camera.main.ScreenToWorldPoint(mouse);
    }


  }

  void ToggleVisible()
  {
    Debug.Log("toogled cur");
    UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
    this.gameObject.GetComponent<SpriteRenderer>().enabled = !UnityEngine.Cursor.visible;
  }
}




public class DojoBehaviors : MonoBehaviour
{
    public GameObject Table;

  public void ToggleTableOn()
    {   
        GameObject.FindWithTag("Player").GetComponent<MasterController>().playable = false;        
        Table.SetActive(true);
        this.gameObject.GetComponent<AnotationHotBar>().SpawnNotes();
        GameEvents.ScreamEvent("ToogleCursor");
        
    }
    public void ToggleTableOff()
    {   
        GameEvents.ScreamEvent("ToogleCursor");   
        GameObject.FindWithTag("Player").GetComponent<MasterController>().playable = true;     
        Time.timeScale = 1f;
        Table.SetActive(false);
        this.gameObject.GetComponent<AnotationHotBar>().DestroyNotes();
    }
}




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
  private float temprad;

    void Start()
    {
        mainCode = this.gameObject.GetComponent<MasterController>();
        a = this.gameObject.GetComponent<AudioInterface>();
        temprad = mainCode.radius;
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
      mainCode.invt = 0.05f;
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
      if(!mainCode.isGrounded && mainCode.rigb.velocity.y > 0)
      {
        mainCode.radius = 0f;

      }
      else
      {
        mainCode.radius = temprad;
      }
      mainCode.isGrounded = Physics2D.OverlapCircle(mainCode.flchk.position, mainCode.radius, mainCode.solid);
      
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





public class HealthBar : MonoBehaviour
{ 
  public static GameObject Instance; 
  private RectTransform rtrs;

  public bool isVisible = true;
  private Image gaugeImage;


    void OnEnable()
  { 
     if(HealthBar.Instance == null)
        {
            HealthBar.Instance = this.gameObject;
        }
        else if(HealthBar.Instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }

        Time.timeScale = 1f;
    GameEvents.StartListening("GameResumed", SetVisible);
    GameEvents.StartListening("GamePaused", SetInvisible);
  }
  void OnDisable()
  { 
    GameEvents.StopListening("GamePaused", SetVisible);
    GameEvents.StopListening("GameResumed", SetInvisible);
    
  }

  private void Awake()
  {
    gaugeImage = transform.Find("Gauge").GetComponent<Image>();
  }

  void Start()
  {
    rtrs = GetComponent<RectTransform>();
  }

  public static void SetGaugeValue(float delta, float life, float maxlife)
  {
    HealthBar x = Instance.GetComponent<HealthBar>();
    float deltapertick = (delta)/10;
    Debug.Log("st life" + (life - delta));
    Debug.Log("true life" + life);
    Debug.Log("delta p/ tick" + deltapertick);


    x.StartCoroutine(lerpLife(deltapertick, maxlife, 0.02f, 10));
    
  }
  public static IEnumerator lerpLife(float a, float m, float t, int times)
  {

    yield return new WaitForSeconds(t);
    Instance.GetComponent<HealthBar>().gaugeImage.fillAmount += a/m;
    if(times > 0)
    {
      Instance.GetComponent<HealthBar>().StartCoroutine(lerpLife(a, m, t, times-1));
    }
    

  }
    
  public void ToggleVisibility(bool x)
  {
    isVisible = x;
    HealthBar.Instance.SetActive(x);
  }
  public static void SetInvisible()
  { 
    Instance.GetComponent<HealthBar>().ToggleVisibility(false);
  }
  public static void SetVisible()
  { 
    Instance.GetComponent<HealthBar>().ToggleVisibility(true);
  }


}
  



public class DialogInitializer : MonoBehaviour
{
    // Start is called before the first frame update

  [SerializeField] public List<DialogContent> dialog = new List<DialogContent>();

    // Update is called once per frame
  [SerializeField]
  public static DialogBox TextBox;

  public void OpenDialogBox()
    {   
        GameEvents.ScreamEvent("GetDialogBox");
        DialogInitializer.TextBox.dialogSequence = new List<DialogContent>(dialog);
      DialogInitializer.TextBox.Activate(true);
        Debug.Log("START DIALOG");
      
    }
  public void CloseDialogBox()
    {
      DialogInitializer.TextBox.dialogSequence = new List<DialogContent>();
      DialogInitializer.TextBox.Activate(false);
    }

}




[RequireComponent(typeof(DialogInitializer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DialogTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float range;
    private DialogInitializer logStarter;
    private InputManager InPut;
    private Transform p;
    private Vector2 t_pos;
    private bool inside = false;
    private bool showing = false ;

    [SerializeField]
    private bool interactable;
    [SerializeField]
  private bool pause;
  [SerializeField]
  private bool destroyonread;

  [SerializeField]


    void Start()
    {
        logStarter = this.gameObject.GetComponent<DialogInitializer>();
        InPut = InputManager.instance;
        p = GameObject.FindWithTag("Player").transform;
        t_pos = this.gameObject.transform.position;
    }
    void Update()
    {
        float d = ((Vector2)p.position - t_pos).sqrMagnitude;
        bool i = (d <= range*range);

        if(inside != i)
        {      
            inside = i;
            if(!inside)
            {  
                showing = false;
                ExitRange();
            }
            
        }
        if(inside)
        {
            EnterRange();
        }
        
    }

    void ExitRange()
    {
        Debug.Log("OutRange");
        logStarter.CloseDialogBox();
      
    }
    void EnterRange()
    {
        Debug.Log("Inrange");
        if((!interactable || InPut.GetButtonDown("Jump") || InPut.GetAxisRaw("Vertical") > 0) && !showing)
        {   
            showing = true;
            logStarter.OpenDialogBox();

            if(pause)
            {
                Time.timeScale = 0f;
            }
            if(destroyonread)
            {
                Destroy(this.gameObject);
            }

        }
    }
    



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, range*range);
    }
}





public class DialogBox : MonoBehaviour
{
    // Start is called before the first frame update
    public List<DialogContent> dialogSequence = new List<DialogContent>();

    private DialogContent currentDialog;

    [SerializeField] private List<GameObject> buttons = new List<GameObject>(){null, null};

    [SerializeField] private GameObject Box;

    [SerializeField] private Text label;

    public InputManager InPut;

    [SerializeField] private Text namelabel;

    [SerializeField] private Image picture;
    
    public int msgnum = 0;
   
    public Animator anim;


    void Awake()
    {   
        anim = this.gameObject.GetComponent<Animator>();
        
    }
    void OnEnable()
    {
        GameEvents.StartListening("GetDialogBox", SetDialogBox);
        InPut = InputManager.instance;

    }
    void OnDisable()
    {
        GameEvents.StopListening("GetDialogBox", SetDialogBox);
    }

    // Update is called once per frame
    void Update()
    {   
        if(Box.activeSelf)
        {   
            if(InPut.GetButtonDown("Jump")|| InPut.GetButtonDown("up"))
            {      

                sendNextMsg();
            }
        }    
    }

    void sendNextMsg()
    {   
        if(msgnum <= dialogSequence.Count)
        {
           

            
            if(msgnum == dialogSequence.Count)
            {
              Debug.Log("Dialog Number " + (msgnum +1) + "/" + (dialogSequence.Count) + " completed - Exiting dialogSequence");
                Time.timeScale = 1f;
                anim.Play("Dialog Out");
                dialogSequence = new List<DialogContent>();
                msgnum = 0;
                currentDialog.FinalAction();
                currentDialog = null;
            }
            else
            {
        Debug.Log("Dialog Number " + (msgnum +1) + "/" + (dialogSequence.Count));
              currentDialog = dialogSequence[msgnum];

              label.text = currentDialog.GetMainText();
              namelabel.text = currentDialog.GetOwnerName();
              picture.sprite = currentDialog.GetOwnerPhoto();
              currentDialog.FinalAction();
              msgnum ++;
            }
            
        }
        else
        {
            Time.timeScale = 1f;
            anim.Play("Dialog Out");
            dialogSequence = new List<DialogContent>();
            msgnum = 0;
            currentDialog.FinalAction();
            currentDialog = null;         
        }

    }
    void EnterChildren()
    {   
        if(dialogSequence.Count != 0)
        {   
            namelabel.gameObject.SetActive(true);
            label.gameObject.SetActive(true);
            picture.gameObject.SetActive(true);
            label.text = dialogSequence[0].GetMainText();
            namelabel.text = dialogSequence[0].GetOwnerName();
            picture.sprite = dialogSequence[0].GetOwnerPhoto();
        }
    }

    void ExitChildren()
    {   
        namelabel.gameObject.SetActive(false);
        label.gameObject.SetActive(false);
        picture.gameObject.SetActive(false);
    }
    void Exit()
    {
        Box.SetActive(false);
    }

    void SetDialogBox()
    {
        DialogInitializer.TextBox = this;
        msgnum = 0;
    }

    public void Activate(bool state)
    {   
      if(state)
      { 
        Box.SetActive(true);
        anim.Play("Dialog In"); 
      }
      else
      { 
        anim.Play("Dialog Out");
      }

    }
}





public class MainMenu : SceneLoader
{ public string scenename;

  public GameObject OtherPannel1;

  public GameObject OtherPannel2;

  public void CallScene(string cscene)
  {
    SceneManager.LoadScene(cscene);
  }

  public void CallOptions()
  {
    OtherPannel1.SetActive(true);
    gameObject.SetActive(false);
  }
  public void CallLvlS()
  {
    OtherPannel2.SetActive(true);

  }

  public void Quit()
  {
    Application.Quit();
  }

  public void Scenecaller()
  {
    CallScene(scenename);
  }
}





public class UiResScaler : MonoBehaviour
{
private RectTransform trs;
    private Transform trs2;


    [SerializeField] private Vector2 pos = Vector2.zero;

    [SerializeField] private Vector2 sc = Vector2.zero;
    [SerializeField] private Vector2 sd = Vector2.one;

    [SerializeField] private bool defaultTRS = true;
    [SerializeField] private bool texted = false;

  [SerializeField] private bool w_x_h = true;

    private Camera c;
    private Vector2 img_dim = Vector2.one;
    [SerializeField] private float modvalue = 1; 



    void Start()
    {
      trs = this.gameObject.GetComponent<RectTransform>(); 
        trs2 = this.gameObject.GetComponent<Transform>();
        c = Camera.main;
  
        

      if(pos == Vector2.zero)
      {
      if(defaultTRS)
        {
            pos = trs2.position;
            sc = trs2.localScale;
        }
        else
        {
          pos = trs.anchoredPosition; 
          sc = trs.localScale;
        }
      }

        AdaptToCurrentResolution(1f, 1f);

    }


    void AdaptToCurrentResolution(float wid = 1, float hei = 1)
    {
      w_x_h = (c.aspect > 16/9);
      Vector2 res = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    if(w_x_h)
      {
        
        modvalue = res.x/Screen.width;
      }
      else
      {
        modvalue = res.y/Screen.height;

      }
      //modvalue = modvalue/Mathf.Floor(modvalue);
      Debug.Log("MOD VALUE: " + modvalue);

      if(defaultTRS)
      {
        trs2.localScale =  new Vector3(sc.x*modvalue, sc.y*modvalue, 1f);
      }
      else
        {

            trs.anchoredPosition = new Vector3(pos.x/modvalue, pos.y/modvalue, 1f);
            if(texted)
            {
                trs.localScale = new Vector3(sc.x/modvalue, sc.y/modvalue, 1f);
            }
            else
            {
                trs.sizeDelta = trs.sizeDelta*modvalue;
            }
                     
        }   

    }
}





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
  public InputManager InPut;


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

  //atributos de movimentação
  public float speed;
  public float maxspeed;
  public float jspeed;
  public float gScale = 2f;
  public float inWater = 0; // 0 = fora, 1 = dentro d'água;
  
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

  private float jumpframes = 0;
    // Start is called before the first frame update
    void Start()
    {
        //coletando componentes
    anim = this.gameObject.GetComponent<Animator> ();
    rigb = this.gameObject.GetComponent<Rigidbody2D> ();
    trs = this.gameObject.GetComponent<Transform> ();
    spr = this.gameObject.GetComponent<SpriteRenderer> ();
    camcontroll = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<CameraBehavior>();
    lifeBar = HealthBar.Instance.GetComponent<HealthBar>();
    movSen = 1;
    lstmovSen = 1;
    life = maxlife;

    Vector2 chckpos = CheckPointManager.GetLastCheckPoint();
    if(chckpos != Vector2.zero)
    {
      GameEvents.ScreamEvent("CheckPointFound");  
      trs.position = chckpos;
      playable = true;
    }

    playable = true;
    InPut = InputManager.instance;

    }

    // Update is called once per frame
    public void GetControlInput()
  { 
    jumpDirection = Vector2.up;
    axis =  InPut.GetAxisRaw("Horizontal");
    ayis = InPut.GetAxisRaw("Vertical");
    i_jump = InPut.GetButtonDown("Jump");
    jump = InPut.GetButton("Jump");
    atk2 = InPut.GetButtonDown("Spec");
    atk = InPut.GetButtonDown("Attack");
  }


  public void FliptFr ()
  { 
    if(isGrounded)
    { 
      MakeDust();
    }
    

    trs.localScale = new Vector2 (-trs.localScale.x, trs.localScale.y);
  }
  void FixedUpdate()
  { 
    
    float d = 0.0f;
    jumpDirection = OnSlopeMoviment(out d);

    if(!isGrounded || (isGrounded && jumpDirection == Vector2.up && d > 0.05f) && rigb.velocity.y > -30)
    {
      rigb.velocity -= jumpDirection*gScale;
    }

  }
  
  public void GroundMoviment()
  { 
    
    flytime = 0;
    gothit = false;
    if(!i_jump && jumpframes <= 0)
    { 
      jumpframes = 0;
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
      if  (Mathf.Abs(rigb.velocity.x) > speed*maxspeed && !isCrouch)
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
      IgniteJump();
    }
    else
    {
      jumpframes -= Time.deltaTime;
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
    
    flytime += Time.deltaTime;

    isCrouch = false;
    if(jumpframes <= 0)
    {

      if(i_jump && flytime <= 0.15f)
      {
        Debug.Log(jumpframes);
        IgniteJump();
      }
    }
    else
    {
      jumpframes -= Time.deltaTime;
    }

    if(jump)
    { 

      if(rigb.velocity.y >= 0.0f)
      {

        if(flytime <= 0.2f)
        { 
          if(rigb.velocity.y < jspeed)
          rigb.velocity += new Vector2(0, jspeed -rigb.velocity.y)*1/2;
        }

      }
      else if(rigb.velocity.y < -17f)
      {
        rigb.velocity += new Vector2(0, -17f - rigb.velocity.y)/2;
      }
    }




    //movimento horizontal
    if(maxspeed >= 2f)
    {
      maxspeed -= Time.deltaTime*2;
    }

    if(axis == 0 && Mathf.Abs(rigb.velocity.x) > 0)
    {
      SlowDown(1f/3.0f);
    }
    if(Mathf.Abs(rigb.velocity.x) < 1)
    {
      runtime = 0;
    }
    if (axis != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("flying down") && Mathf.Abs(rigb.velocity.x) < speed*maxspeed) 
    {
      
      if(runtime < 1f)
      {
        runtime += Time.deltaTime;
      }

      rigb.velocity = new Vector2(rigb.velocity.x + movSen*speed*Mathf.Pow(0.5f, 3.5f - runtime), rigb.velocity.y);
    }


  }
  /*https://www.youtube.com/watch?v=zJDR_wD0J5U*/
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
        
        
        if(other.gameObject.GetComponent<HitableParts>())
        {
          other.gameObject.GetComponent<HitableParts>().takedamage(attackdmg);
        }
        else if(other.gameObject.GetComponent<InfectedNKBehavior>())
        {
          other.gameObject.GetComponent<InfectedNKBehavior>().bossCore.takedamage(attackdmg);
        }
        else if(other.gameObject.GetComponent<PiranhaBehavior>())
        {
          other.gameObject.GetComponent<PiranhaBehavior>().bossCore.takedamage(attackdmg);
        }
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
      HealthBar.SetGaugeValue(-dmg, life, maxlife);
    }
    

  }

  public void gainlife(int gainl)
  {
    life += gainl;

    int sup = 0;

    if (life > maxlife)
    {
      sup = life - maxlife;
      life = maxlife;
    }
    HealthBar.SetGaugeValue(gainl-sup, life, maxlife);
    
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
    camcontroll.edgeright = Mathf.Infinity;
    camcontroll.edgeup = Mathf.Infinity;
    camcontroll.edgedown = Mathf.NegativeInfinity;
    camcontroll.edgeleft = Mathf.NegativeInfinity;

    
    //dead = true;
  }

  public void Dying()
  {
    camcontroll.ToggleShake(false, 0.6f,  0.6f);
    lifeBar.ToggleVisibility(false);
    GameEvents.ScreamEvent("GameOver");


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
    HealthBar.SetGaugeValue(-dmg, life, maxlife);

  }

  public void GroundSave(Vector2 returnpoint)
  {
    GroundReturn = returnpoint; 
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(flchk.position, radius);
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
  private void IgniteJump()
  {
    jumpframes = 0.2f;
    rigb.velocity += Vector2.up*jspeed;
    flytime = 0f;
    MakeDust();
    isCrouch = false;
  }

  private Vector2 OnSlopeMoviment(out float dist)
  { 
    RaycastHit2D hit = Physics2D.Raycast(flchk.position, Vector2.down, 1f, solid);

    if(hit)
    { 
      dist = hit.distance;
      Debug.DrawRay(hit.point, Vector2.Perpendicular(hit.normal), Color.green);
      Debug.DrawRay(hit.point, hit.normal, Color.red);

      return hit.normal;
    }
    dist = 0.0f;

    return Vector2.up;
  }


}




public class SceneChanger : MonoBehaviour
{  

    public static string nextScene;
  private static GameObject instance;
    [SerializeField] private Animator anim;

    void OnEnable()
  { 
        GameEvents.StartListening("FinalBossIsDead", CompleteLevel);
    GameEvents.StartListening("FadeOut", FadeOut);
  }
  void OnDisable()
  {
        GameEvents.StopListening("FinalBossIsDead", CompleteLevel);
    GameEvents.StopListening("FadeOut", FadeOut);
  }
    void Start()
    {
        
        if(SceneChanger.instance == null)
        {
            SceneChanger.instance = this.gameObject;
        }
        else if(SceneChanger.instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        Time.timeScale = 1f;
    }



    public void FadeOut()
    { 
        PlayerPrefs.SetFloat("playtime", Time.timeSinceLevelLoad);
        PlayerPrefs.Save();
      anim.Play("FadeOut");
    }

    public void LoadScene(string s)
    { 

        if(nextScene != null)
        { 
            GameEvents.ScreamEvent("LoadingAScene");
            SceneManager.LoadScene(nextScene);
            nextScene = null;
        }
        else if(s != null)
        {
            GameEvents.ScreamEvent("LoadingAScene");

            SceneManager.LoadScene(s);
        }
      
    }

    public void CompleteLevel()
    {
        int i = PlayerPrefs.GetInt("level");
        PlayerPrefs.SetInt("level", i+1);
        PlayerPrefs.Save();
        GameEvents.ScreamEvent("FadeOut");

    }
    public static void Load(string s)
    {
        GameEvents.ScreamEvent("LoadingAScene");
        SceneChanger.nextScene = s;
        SceneChanger.instance.GetComponent<SceneChanger>().FadeOut();
    }

}





public class PauseMenu : MonoBehaviour
{   
    public static bool isPaused = false;

  public GameObject MenuObject;

  public static MusicPlayer musicController;

    public GameObject GameOverMenu;

    private InputManager InPut;

    public bool canpause = true;


    void OnEnable()
    {
        GameEvents.StartListening("GameOver", OverMenuShow);
        
    }
    void OnDisable()
    {
        GameEvents.StopListening("GameOver", OverMenuShow);
    }
    void Start()
    {
        InPut = InputManager.instance;
    }
    void Update()
    {
        if(InPut.GetButtonDown("pause") && canpause)
        {
          PauseAlternate();
        }
    }
    public void Restart()
    {   

        MenuObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        //PauseMenu.musicController.UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Resume()
    {      
        GameEvents.ScreamEvent("GameResumed");
      MenuObject.SetActive(false);
      Time.timeScale = 1f;
      isPaused = false;
      PauseMenu.musicController.UnPause();
        
    }

    public void Menu()
    {
      SceneManager.LoadScene("Title");
      Time.timeScale = 1f;
    }


    public void Pause()
    { 

      GameEvents.ScreamEvent("GamePaused");
      MenuObject.SetActive(true);
      Time.timeScale = 0f;
        Debug.Log(PauseMenu.musicController);
        if(PauseMenu.musicController != null)
        {
            PauseMenu.musicController.Pause();
        }
      
      isPaused = true;
    }

    public void PauseAlternate()
    {   
        
      if(isPaused)
          {
            Resume();
            
          }
          else
          {
            Pause();
            
          }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CallScene(string cscene)
    {
        SceneManager.LoadScene(cscene);
    }

    public void BestiaryPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Bestiary");
    }
    public void AnotationPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Anotation");
    }
    public void OptionPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Options");
    }
    public void MainPauseMenu()
    {
        MenuObject.GetComponent<Animator>().Play("PMenuFadein");
    }
    public void OverMenuShow()
    {
        GameOverMenu.SetActive(true);
    }
}




public class CheckPointBehaviour : MonoBehaviour
{
    [SerializeField] private int checknum;

    private Animator anim;
    [SerializeField] bool isOn;

    void Start()
    { 
      anim = this.gameObject.GetComponent<Animator>();
      isOn = CheckPointManager.GetCheckPoint(checknum);
        if(isOn)
        {
          anim.Play("on");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
      if(!isOn)
      {
        if(other.gameObject.tag == "Player")
        { 
          CheckPointManager.RefreshCheckPoint(checknum, this.gameObject.GetComponent<Transform>().position);
          isOn = true;
          anim.Play("on");
        }       
      }

    }
}




public class DojoPositionSaver : MonoBehaviour
{
    void OnEnable()
  { 
        GameEvents.StartListening("LoadingAScene", SavePlayerPosition);

  }
  void OnDisable()
  {
        GameEvents.StopListening("LoadingAScene", SavePlayerPosition);

  }

  public void SavePlayerPosition()
  {
    CheckPointManager.RefreshCheckPoint(1, GameObject.FindWithTag("Player").GetComponent<Transform>().position);
  }
  

}





public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;
    public string assingedLevel;
    public static Dictionary<int, Vector2> Points = new Dictionary<int, Vector2>();


    void Awake()
    {   
      string Scene = SceneManager.GetActiveScene().name;
      

        if(instance == null)
        {
            instance = this;
            Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
      if(Scene != "")
        {
            if(instance.assingedLevel == Scene)
            {
              Destroy(this.gameObject);
            }
            else
            {

            CheckPointManager.Points.Clear();
              Destroy(instance.gameObject);
              instance = this;
                Object.DontDestroyOnLoad(this.gameObject);
            }
          }
            
        }
        else
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }
        

    }
    public static void NewCheckPoint(int n, Vector2 pos)
    {
      Points.Add(n, pos);
    }

    public static void RefreshCheckPoint(int n, Vector2 pos)
    {
      if(Points.ContainsKey(n))
      {
        Points[n] = pos;
      }
      else
      {
        NewCheckPoint(n, pos);
      }

      
      
    }

    public static bool GetCheckPoint(int n)
    {
      return (Points.ContainsKey(n));
    }
    public static Vector2 GetLastCheckPoint()
    {
      Vector2 checkpos = Vector2.zero;
      Points.TryGetValue(Points.Count, out checkpos);
      return checkpos;
    }
}
  



public class AnotationPoint : MonoBehaviour
{ 
  [SerializeField] private bool Attached = false;
    [SerializeField] private AnotationManager AnBook;

    [SerializeField] private ParticleSystem part;

    [SerializeField] private Texture sp;
    [SerializeField] private int id;

    [SerializeField] public GameObject ao;
    [SerializeField] public AudioInterface a;
    
    [TextArea]
    [Tooltip("Nome do inimigo (Identificador)")]
    [SerializeField] private string txt;

    [SerializeField] private bool destroyonread = true;

    void Awake()
    {
      AnBook = GameObject.Find("AnotationBook").GetComponent<AnotationManager>();
      a = ao.GetComponent<AudioInterface>();
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D other)
    {
      if(!Attached)
      {
        if(other.gameObject.tag == "Player" && InputManager.instance.GetAxisRaw("Vertical") > 0)
        { 
          AdcthisNote();
          if(destroyonread)
          { 
            a.PlaySound("got");
            part.Stop();
            ao.GetComponent<Animator>().Play("Vanish");
            this.gameObject.transform.DetachChildren();
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
          }
        }
      }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if(!Attached)
      { 
        if(other.gameObject.tag == "Player")
        { 
          a.PlaySound("see");
          part.Play();
        }
      }
    }

    void OnTriggerExit2D(Collider2D other)
    { 
      if(!Attached)
      {
        if(other.gameObject.tag == "Player")
        { 
          part.Stop();
        }
      }
    }

    public void AdcthisNote()
    {
      AnotationManager.AddNote(new Anotation(id, sp, txt));
      PontuationCounter.AddScore(1000);
    }
}





[RequireComponent(typeof(PlayableDirector))]
public class Cutscenechanger : MonoBehaviour
{   
    [SerializeField] private float canceltime;
    void OnEnable()
    {
        GameEvents.StartListening("CheckPointFound", CancelCutscene);
    }
    void OnDisable()
    {
        GameEvents.StopListening("CheckPointFound", CancelCutscene);
    }
    public List<TimelineAsset> cutscenes;
    // Start is called before the first frame update
    public void PlayCutscene(int index)
    {   
        if((Time.time - canceltime) > 1f)
        {
            TimelineAsset sel_Cutscene;
            if(cutscenes.Count > index)
            {
                sel_Cutscene = cutscenes[index];
                
            }
            else
            {
                sel_Cutscene = cutscenes[cutscenes.Count -1];
            }
            if(sel_Cutscene)
            {
                this.gameObject.GetComponent<PlayableDirector>().playableAsset = sel_Cutscene;
                this.gameObject.GetComponent<PlayableDirector>().Play(sel_Cutscene);  
            }      
        }

        
    }
    
    public void CancelCutscene()
    {
        canceltime = Time.time;
        this.gameObject.GetComponent<PlayableDirector>().Stop();
    }


}





public class HitableParts : MonoBehaviour {

  public SpriteRenderer spr;
  public Animator anim;


  public bool isinvencible;
  public bool isdangerous;

  public Collider2D h1, h2, h3, h4;
  public bool hit1, hit2, hit3, hit4;

  public int damage;

  public bool hasmultipleparts;

  public int life;
  public int maxlife;
  public float dmgtime;
  private float blinktime;

  //caso estejamos tratando do objeto contendo a vida do chefe
  public bool isBossCore;
  public Texture hplevel, frame, bottom;

  
  void Start()
  {
    life = maxlife;
    if(isBossCore)
    {
      anim = GetComponent<Animator>();  
    }
    
    spr = GetComponent<SpriteRenderer>();
  }
  void Update() 
  { 
    if(hasmultipleparts)
    {
      h1.enabled = hit1;
      h2.enabled = hit2;
      h3.enabled = hit3;
      h4.enabled = hit4;
    }
    if(blinktime > 0)
    {
      blinktime -= Time.deltaTime;
      if(isBossCore){anim.Play("BlinkDamage");}
      
      spr.color = new Color(Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*5, 1f), Mathf.PingPong(Time.time*10f, 1f));
    }
    else
    {
      if(isBossCore){anim.Play("Idle");}
    }


    if(isdangerous)
    {
      spr.color  = new Color(1f, 1f - Mathf.PingPong(Time.time*2/3, 0.67f) ,1f - Mathf.PingPong(Time.time, 1f));
    }
    else
    {
      spr.color = Color.white;
    }


  }

  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && isdangerous)
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(damage);
    }

  }
  public void takedamage(int dmg)
  {

    if(!isinvencible && blinktime <= 0)
    {
      life -= dmg;

      blinktime = dmgtime;
      if (life <= 0)
      { 
        if (!isBossCore)
        {
          Destroy(this.gameObject);
        }
        else
        {
          GameEvents.ScreamEvent("BossDie");
        }
        
      }

    }

  }
  void OnGUI ()
  { 
    if(isBossCore)
    {
      GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), bottom);
      if(life > 0)
      {
        GUI.DrawTexture (new Rect (Screen.width*0.405f, Screen.height*0.874f, Screen.width*0.587f*life/maxlife, Screen.height*0.117f), hplevel);
      }
      
      GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), frame);
    }
    
  }

}




public class BossCore : MonoBehaviour
{ 
  public bool showlifebar;
  public float damageMultiplicator = 1;
  public int life;
  public int maxlife;
  public float dmgtime;
  public int damage;
  public bool isinvencible = false;
  public bool isdangerous = true;

  public Transform trs;

  private float blinktime;

  public Texture hplevel, frame, bottom;
  void Start()
    { 
      trs = this.gameObject.GetComponent<Transform>();
        life = maxlife;
    }

    // Update is called once per frame
    void Update()
    {
        if(blinktime > 0f)
        {
          isinvencible = true;

          blinktime -= Time.deltaTime;
        }
        else
        {
          isinvencible = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && isdangerous)
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(damage);
    }

  }
  public void takedamage(int dmg)
  {

    if(!isinvencible)
    {
      life -= Mathf.RoundToInt(dmg*damageMultiplicator);
      GameEvents.ScreamEvent("BossDamaged");
      blinktime = dmgtime;
      if (life <= 0)
      { 
        GameEvents.ScreamEvent("BossDie");
        GameEvents.ScreamEvent("BossDie");
        Debug.Log("BossdieEvent");
      }
    }
  }

  void OnGUI ()
  { 
    if(showlifebar)
    {
      GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), bottom);
      if(life > 0)
      {
        GUI.DrawTexture (new Rect (Screen.width*0.405f, Screen.height*0.874f, Screen.width*0.587f*life/maxlife, Screen.height*0.117f), hplevel);
      }
        
      GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), frame);       
    }

  }

  public void sendEvent(string eventname)
  {
    GameEvents.ScreamEvent(eventname);
  }
  public void changeCamScale(float scale)
  {
    GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().camscale = scale;
  }
  public void changeCamPositionx(float positionx)
  { 
    GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().posx = positionx;
  }
   public void changeCamPositiony(float positiony)
  { 
    GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().posy = positiony;
  }
  public void ToggleCamTarget(float targetHeight = 5)
  { 
    var cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>();
    if(cam.targeted)
    {
      cam.posx = trs.position.x;
      cam.posy = trs.position.y + targetHeight;
    }
    cam.targeted = !cam.targeted;
  }


  public void ded()
  {
    GameEvents.ScreamEvent("FinalBossIsDead");
  }


}



public class BackGroundTrigger : MonoBehaviour
{ 
  private Transform obj;
  private Transform Player;
  public int startbg = 0;
  private bool arin = false;
    public Vector2 BackgLeft_Right;
    public Color[] BgCollors;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        obj = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    { 
        

        if(1f > Mathf.Abs(Player.position.x - obj.position.x) && arin == false)
        {   
            GameObject[] bgn = GameObject.FindGameObjectsWithTag("Parallax");
          setBackground(bgn);
          arin = true;
        }
        else if(1f < Mathf.Abs(Player.position.x - obj.position.x))
        {
          arin = false;
        }
        
    }

    void setBackground(GameObject[] bglist)
    {  
        Debug.Log("Bg settado");
        if(Player.position.x < obj.position.x)
        {
            startbg = Mathf.RoundToInt(BackgLeft_Right.y);
            cam.backgroundColor =  BgCollors[1];
        }
        else if(Player.position.x > obj.position.x)
        {
            startbg = Mathf.RoundToInt(BackgLeft_Right.x);
            cam.backgroundColor =  BgCollors[0];
        }
      foreach(GameObject b in bglist)
      {
        b.GetComponent<NewParallax>().ChangeBackground(startbg);
      }
    }
}




public class EventScreamer : MonoBehaviour
{
  public void Scream(string EventName)
  {
    GameEvents.ScreamEvent(EventName);
  }
}




public class BossMovableTerrain : MonoBehaviour
{ 
  public List<GameObject> SideTerrains = new List<GameObject>{};
  private Animator anim;

  private Transform trs;

  private Rigidbody2D rigb;

  public bool uped = false;

  public float uptime = 0;

  private float direction = 0;

  private float inten = 0;

  
    // Start is called before the first frame update
    void Start()
    { 
      trs = this.gameObject.GetComponent<Transform>();
        anim = this.gameObject.GetComponent<Animator>();
        rigb = this.gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    { 
      if(trs.position.y < -88)
      {
        trs.position = new Vector3(trs.position.x, -88, trs.position.z);
      }
      if(uptime > 0f)
      { 
        uptime -= Time.deltaTime;
        Move(direction, true);
      }
      else
      { 

        uptime = 0;
        if(trs.position.y > -88)
        { 

          Move(-88, false);
          inten = 0.5f;
        }
        else
        {
          uped = false;
        }
        
      }
    }

    
    public void GetPulled(float intencity)
    { 
      direction = trs.position.y + intencity;
      uptime = 0.8f;
      inten = intencity;
      uped = true;
    }
    public void Move(float dir, bool movesides)
    { 
      
      trs.position = Vector3.MoveTowards(trs.position, new Vector3(trs.position.x, dir, 0), Time.deltaTime*inten);
      foreach(GameObject Terrain in SideTerrains)
      { 
        var Strr = Terrain.GetComponent<BossMovableTerrain>();
        if(!Strr.uped && movesides)
        {
          Strr.GetPulled(inten*6/8);
        }
      }
    }

    void OnCollisionStay2D(Collision2D col)
    { 
      if(col.collider.gameObject.tag.Equals("Player"))
    {
      var hited = col.collider.gameObject.GetComponent<MasterController>();
      if(anim.GetCurrentAnimatorStateInfo(0).IsName("Slimed"))
      {
        hited.maxspeed = 0.9f;
      }
      else
      {
        hited.maxspeed = 2f;
      }
      
    }
    }
}




public class ElevatorBehavior : MonoBehaviour
{
    [SerializeField] private LineRenderer ln;

    private Transform trs;

    [SerializeField] private bool isMoving;

    [SerializeField] private int c_point;

    [SerializeField] private List<float> H_points = new List<float>();

    private MasterController player;

    void Start()
    {
        trs = this.gameObject.GetComponent<Transform>();
        ln = this.gameObject.GetComponent<LineRenderer>();
        ln.SetPosition(0, Vector3.right*trs.position.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if(isMoving)
      {
        
      trs.position = Vector2.MoveTowards(trs.position, H_points[c_point]*Vector2.up + trs.position.x*Vector2.right, 30*Time.fixedDeltaTime);
        player.trs.position -= Vector3.up*Mathf.Sign(trs.position.y - H_points[c_point])*30*Time.fixedDeltaTime; 
        if(Mathf.Abs(H_points[c_point] - trs.position.y) <= 0.05f)
        {
        isMoving = false;
          player.playable = true;
        }
        ln.SetPosition(1, trs.position + Vector3.up*2);
      }  
    }

    public void OnTriggerStay2D(Collider2D other)
    {
      if(other.gameObject.tag == "Player")
      {
        if(!isMoving)
        { 

          int i = -(int)Input.GetAxisRaw("Vertical");
          Debug.Log(i);
          if(c_point + i < H_points.Count && c_point + i >= 0 && i != 0)
          {
          c_point += i;
          player = other.gameObject.GetComponent<MasterController>();
          player.playable = false;
          player.rigb.velocity = Vector2.zero;
          isMoving = true;

          }
          
        }
      }
    }
}




public class NewParallax : MonoBehaviour
{ 
  [SerializeField] private Vector2 V_Limit;
  [SerializeField] private Vector2 parallax_factor;
    [SerializeField] private Vector2 compensation = Vector2.zero;
    [SerializeField] private float startpos = 0f;
    private int tex_index = 0;
    public List<Sprite> nextTextures = new List<Sprite>();
    private float lenght;
    //private float temp;
    
  [SerializeField] private Transform camera_trs;
  private Vector3 last_cam_position;

    [SerializeField] private SpriteRenderer spr;

    public bool parallaxing = false;

    // Start is called before the first frame update
    void Start()
    {   
        camera_trs = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        //last_cam_position = camera_trs.position;
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        lenght = spr.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        float temp = (camera_trs.position.x * (1f - parallax_factor.x));
        float dist = camera_trs.position.x * parallax_factor.x;

        transform.position = new Vector3(startpos + camera_trs.position.x + dist, transform.position.y, transform.position.z);

        if(temp > startpos + lenght + camera_trs.position.x)
        {
          startpos += lenght;
        }
        else if(temp < startpos - lenght + camera_trs.position.x)
        {
          startpos -= lenght;
        }

    temp = (camera_trs.position.y * (1f - parallax_factor.y));
        dist = camera_trs.position.y * parallax_factor.y;

        if((camera_trs.position.y + dist + compensation.y < V_Limit.y && camera_trs.position.y + dist + compensation.y > V_Limit.x) || V_Limit == Vector2.zero)
        {
          transform.position = new Vector3(transform.position.x, camera_trs.position.y + dist + compensation.y, transform.position.z);
        }
        


    }
    public void ChangeBackground(int t)
    {
      if(t < nextTextures.Count && nextTextures[t] != null)
      {
        spr.sprite = nextTextures[t];
        int i = 0;
        while(i < transform.childCount)
        {
          transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = nextTextures[t];
          i += 1;
        }
        tex_index = t;
      }
    }
}




public class MovingPlatform : MonoBehaviour
{   
    public bool waitForPlayer = false;

    public bool stopOnPlayerQuit = false;

    private float keepspeed = 0;

    public Vector2 StartPos = Vector2.zero;

    public Vector2 EndPos = Vector2.zero;

    public Vector2 Dest = Vector2.zero;

    public Vector2 Strt = Vector2.zero;

    public float speed;

    public float stayTime;

    public float actualtime;

    private Transform trs;

    private SpriteRenderer spr;

    private bool isMoving = true;

    private Vector3 frameTranslocation;

    void Start()
    {
        trs = this.gameObject.GetComponent<Transform>();
        StartPos = trs.position;
        Dest = EndPos;
        Strt = StartPos;
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        if(waitForPlayer)
        {
            keepspeed = speed;
            speed = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if(waitForPlayer && !spr.isVisible)
        {
            trs.position = StartPos;
            speed = 0;
        }
      Vector2 pos = trs.position;
      if(isMoving)
      {
        if(Vector2.Distance(pos, Dest) > 0.1f)
          { 
            frameTranslocation = Vector2.MoveTowards(pos, Dest, Time.fixedDeltaTime*speed) - pos;
            trs.position += frameTranslocation;
          }
          else
          { 
            actualtime = stayTime;
            isMoving = false;
            if(Dest == StartPos)
            {
              Dest = EndPos;
              Strt = StartPos;
            }
            else
            {
              Dest = StartPos;
              Strt = EndPos;
            }
          }
      }
      else
      {
        if(actualtime > 0)
        {
          actualtime -= Time.fixedDeltaTime;
        }
        else
        { 
          actualtime = stayTime;
          isMoving = true;
        }
      }
        if(stopOnPlayerQuit && keepspeed == speed)
        {
            speed = 0;
        }
        
    }

    void OnDrawGizmos()
    { 
      Gizmos.color = Color.blue;
      Gizmos.DrawLine(StartPos, EndPos);
    }


    void OnTriggerStay2D(Collider2D other)
    { 
      
      if(other.gameObject.tag == "Player" && isMoving)
      { 
        other.gameObject.transform.position += frameTranslocation;
            if(waitForPlayer)
            {
                speed = keepspeed;
            }
      }
      
    }
}





public class TerrainTypes : MonoBehaviour
{
    private ContactPoint2D[] contacts;
    private Tilemap tmap;
    public Tilemap othertmap;
    public float baseSpeed = 2;



    void Start()
    { 
      baseSpeed = GameObject.FindWithTag("Player").GetComponent<MasterController>().maxspeed;
      tmap = this.gameObject.GetComponent<Tilemap>();
      othertmap = othertmap.gameObject.GetComponent<Tilemap>(); 
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionStay2D(Collision2D other)
  {
    if(other.gameObject.tag == "Player")
    { 

      if(othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.GetContact(0).point)) != null)
      { 
        string t_tiled = othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.GetContact(0).point)).texture.name;
        if( t_tiled == "slipper liquid")
        {
          other.gameObject.GetComponent<MasterController>().dragforce = 0.01f;
          if(other.gameObject.GetComponent<MasterController>().dashed)
          {
            other.gameObject.GetComponent<MasterController>().maxspeed = baseSpeed+2f;
          }
          
        }
      }
      else
      {
        other.gameObject.GetComponent<MasterController>().dragforce = 1f;
        other.gameObject.GetComponent<MasterController>().maxspeed = baseSpeed;
      }
    }
  }

  private void OnTriggerStay2D(Collider2D other)
  { 
    if(other.gameObject.tag == "Player")
    { 
      if(othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.gameObject.transform.position)) != null)
      { 
        string t_tiled = othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.gameObject.transform.position)).texture.name;
        if(t_tiled == "Watertop" || t_tiled == "Water")
        {
          other.gameObject.GetComponent<MasterController>().inWater = 1f;
          other.gameObject.GetComponent<MasterController>().gScale = 1f;
        }
      }
    }
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    if(other.gameObject.tag == "hitable" && !other.isTrigger)
    { 
      if(other.gameObject.GetComponent<SwimmingTypes>())
      {
        other.gameObject.GetComponent<SwimmingTypes>().SetinWater(true);
      }

    }
  }
  private void OnTriggerExit2D(Collider2D other)
  { 
    if(other.gameObject.tag == "Player")
    { 
      other.gameObject.GetComponent<MasterController>().inWater = 0f;
      other.gameObject.GetComponent<MasterController>().gScale = 2f;

    }
    else if(other.gameObject.tag == "hitable" && !other.isTrigger)
    { 
      if(other.gameObject.GetComponent<SwimmingTypes>())
      {
        other.gameObject.GetComponent<SwimmingTypes>().SetinWater(false);
      }

    }
  }
}




public class Parallax : MonoBehaviour
{ 
  [SerializeField] private Vector2 limit_up_right;
  [SerializeField] private Vector2 limit_down_left;
  [SerializeField] private Vector2 parallax_factor;
    [SerializeField] private Vector2 compensation = Vector2.zero;
    
  [SerializeField] private Transform camera_trs;
  private Vector3 last_cam_position;

    [SerializeField] private SpriteRenderer spr;

    public bool parallaxing = false;

    // Start is called before the first frame update
    void Start()
    {   
        camera_trs = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        last_cam_position = camera_trs.position;
        spr = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = camera_trs.position - last_cam_position;

        if(camera_trs.position.x + compensation.x >= limit_down_left.x && camera_trs.position.x + compensation.x <= limit_up_right.x )
        {   
            parallaxing = true;
          transform.position += new Vector3(delta.x*parallax_factor.x, 0);
        }
        else
        {
            parallaxing = false;
        }
        if(transform.position.x < limit_down_left.x)
        {
            transform.position = new Vector3(limit_down_left.x, transform.position.y, transform.position.z);
        }
        if(transform.position.x > limit_up_right.x)
        {
            transform.position = new Vector3(limit_up_right.x, transform.position.y, transform.position.z);
        }

        if(camera_trs.position.y >= limit_down_left.y && camera_trs.position.y <= limit_up_right.y)
        { 
          transform.position += new Vector3(0, delta.y * parallax_factor.y);  
        }

        last_cam_position = camera_trs.position;
    }
}




public class rotatefur : Parallax
{ 
  [SerializeField] public Rigidbody2D Player;
  [SerializeField] private Animator anim;
  [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    { 

      if(parallaxing)
      {
        speed = Player.velocity.x/48;
      }
      else
      {
        speed = 0f;
      }
      anim.SetFloat("rotatespeed", speed);
        
    }
}




[RequireComponent(typeof(Collider2D))]
public class AirChamber : MonoBehaviour
{   
    public bool stop;
    public Vector2Int relevance = new Vector2Int(1, 1);
    [SerializeField] private float forceScale;
    [SerializeField] private float actualforce;
    [SerializeField] private float timePerState;
    [SerializeField] private float actualtime = 0;
    [SerializeField] private float t = 0;
    public bool changestate = true;
    private MasterController player;
    private Animator anim;
    private SpriteRenderer spr;
    private Transform trs;

    void Start()
    {   
        anim = this.gameObject.GetComponent<Animator>();
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<MasterController>();
        trs = this.gameObject.GetComponent<Transform>();
        anim.SetFloat("speed", Mathf.Sign(forceScale));
        spr.color = new Color(0.5f + t*Mathf.Sign(forceScale)/2, 0, 0.5f - t*Mathf.Sign(forceScale)/2, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {   

      if(spr.isVisible)
        {
            if(actualtime < timePerState && changestate)
            {
                actualtime += Time.deltaTime;
            }
            else
            {       
                actualforce = Mathf.Abs(forceScale)*t*Mathf.Sign(forceScale);
                anim.SetFloat("speed", t*Mathf.Sign(forceScale));
                spr.color = new Color(0.5f + t*Mathf.Sign(forceScale)/2, 0, 0.5f - t*Mathf.Sign(forceScale)/2, 0.7f);
                if(changestate)
                { 

                    t += Time.deltaTime;
                    if(t >= 1)
                    {
                        forceScale = -forceScale;
                        actualtime = 0;
                        t = 0;
                       
                    }
                }
                else
                {
                    t = 1;
                }

            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {   
        if(other.gameObject.tag =="Player")
        {   
          player.gScale = 1 + trs.up.normalized.y*Mathf.Sign(forceScale);
          player.trs.position += (actualforce/3f) * new Vector3(trs.up.normalized.x, 0, 0)*relevance.x;
          
          player.rigb.velocity += (actualforce * 5/4) * new Vector2(0, trs.up.normalized.y)*relevance.y;
            player.axis = player.axis/2;
            player.ayis = player.ayis/2;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    { 
      if(other.gameObject.tag =="Player")
      {
        player.flytime = 0;
        player.gScale = 2;      
      }

    }
  
}




public class Blood : MonoBehaviour {
  private SpriteRenderer spr;
  private Transform trs;
  public int dmg;
  public float length;
  public float yflipct;
  public bool FlipY;

  // Use this for initialization
  void Start () {
    spr = GetComponent<SpriteRenderer> ();
    trs = GetComponent<Transform> ();
  }
  
  // Update is called once per frame
  void Update () {
    spr.size = new Vector2(spr.size.x, length*2);
    if(FlipY)
    {
      trs.position = new Vector3 (trs.position.x, yflipct, trs.position.z);
      spr.flipY = FlipY;
    }
  }

  void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player"))
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(dmg);
    }

  }
}




public class BleedFallBehavior : MonoBehaviour {
  private Animator anim;
  public float BleedTime;
  public float BleedDelay;
  private float ActualTime;
  public bool IsBleeding;
  public  bool laststate;
  public GameObject Blood;
  private Transform trs;
  private GameObject blfalling;
  private Blood bloodmain;
  public float MaxRange = 1;

  private SpriteRenderer spr;

  public ParticleSystem bldparticles;
  // Use this for initialization
  void Start () {
    
    spr = this.gameObject.GetComponent<SpriteRenderer>();
    anim = GetComponent<Animator> ();
    trs = GetComponent<Transform> ();
    ActualTime = BleedDelay;
    IsBleeding = false;
    laststate = false;
  }
  
  // Update is called once per frame
  void Update () 
  {
    if(spr.isVisible)
    {
      ActualTime -= Time.deltaTime;

      if (ActualTime <= 0)
      {
        if(IsBleeding)
        {
          ActualTime = BleedDelay;
          IsBleeding = false;
          
        }
        else
        {
          ActualTime = BleedTime;
          IsBleeding = true;
          
        }
      }

    }





    if(laststate == false && IsBleeding)
    {
        anim.Play("Start");
        
        blfalling = Instantiate(Blood, new Vector3(trs.position.x, trs.position.y-1f, trs.position.z), trs.rotation);
        bloodmain = blfalling.GetComponent<Blood>();

    }
    if(IsBleeding)
    { 
      if(bloodmain.length < MaxRange)
      {
        bloodmain.length += 0.1f;
      }
      else
      {
        bloodmain.length = MaxRange;
      }
      laststate = true;
      bldparticles.Stop();
    }

    else
    { 
      if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Clean"))
      {
        anim.Play("Stop");
        bldparticles.Play();
      }

      
    }
    
    if(!IsBleeding && laststate == true)
    {
      if(bloodmain.length > 0.0f)
      {
        bloodmain.FlipY = true;
        bloodmain.length -= 0.1f;
        bloodmain.yflipct = trs.position.y - 2*MaxRange;
      }
      else
      { 
        laststate = false;
        Destroy(blfalling);
      }
    }


  }
}





public class HitDmgApplyerBehavior : MonoBehaviour {
  public int dmg;
  public bool abyss;
  // Use this for initialization
  void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
    
  }
  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player"))
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      if(abyss)
      {
        hited.fall(dmg);
      }
      else
      {
        hited.takedamage(dmg);
      }
    }
    else if(other.gameObject.tag.Equals("Hitable") && abyss)
    { 
      GameEvents.ScreamEvent("EnemyKilled");
      Destroy(other.gameObject);
    }
  }
}



[ExecuteInEditMode]
public class TiledReact : MonoBehaviour
{ 
  private Animator anim;
    private SpriteRenderer spr;
    private BoxCollider2D bx;
    private Transform trs;
    private ParticleSystem.ShapeModule sg;
    void Awake()
    {
        bx = this.gameObject.GetComponent<BoxCollider2D>();
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        trs = this.gameObject.GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    { 
      bx = this.gameObject.GetComponent<BoxCollider2D>();
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        trs = this.gameObject.GetComponent<Transform>();
      trs.position = new Vector3(Mathf.RoundToInt(trs.position.x), Mathf.RoundToInt(trs.position.y), Mathf.RoundToInt(trs.position.z));
      spr.size = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
        bx.size = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
        ParticleSystem part = this.gameObject.GetComponent<ParticleSystem>();
        if(part != null)
        {
            sg = part.shape;
            sg.enabled = true;
        }
        sg.scale = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
    }
}




public class SlimeDrop : MonoBehaviour
{
    // Start is called before the first frame update

  private Transform trs;
  [SerializeField] private float speed;
  [SerializeField] private int damage;
  [SerializeField] private AudioInterface a;

  void Start()
  { 
    trs = this.gameObject.GetComponent<Transform>();
    a = this.gameObject.GetComponent<AudioInterface>();
  } 
    // Update is called once per frame
    void Update()
    {
        trs.Translate(Vector3.down*speed*Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
      a.PlaySound("slime");
      if(other.gameObject.tag.Equals("Player"))
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(damage);
    }
    if(other.gameObject.tag.Equals("Scenary"))
    { 
      var hited = other.gameObject.GetComponent<Animator>();
      if(hited)
      { 
        hited.Play("Slimed");
      }
      Destroy(this.gameObject);
    }
    
    }
}




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




public class AxeBehavior : MonoBehaviour
{ 
  public GameObject Owner;
  public Vector2 bossPosition;

    public Vector2 target;

    public float searchTime = 1f;

    private Transform trs;

    void Start()
    {
        trs = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
      Vector2 axeposition = trs.position;
      searchTime -= Time.fixedDeltaTime;
      if(searchTime <= 0 || axeposition == target)
      {
        target = bossPosition;
      }
      this.gameObject.transform.position = Vector2.MoveTowards(this.gameObject.transform.position, target, 15 * Time.fixedDeltaTime);
        this.gameObject.transform.RotateAround(target, Vector3.forward, -360 * Time.fixedDeltaTime);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject == Owner && target == bossPosition)
    {
      var hited = other.gameObject.GetComponent<InfectedNKBehavior>();
      if(hited)
      {
        hited.CatchAxe();
        Destroy(this.gameObject);
      }
      
    }
    } 
}






public class InfectedNKBehavior : MonoBehaviour
{ 
    public ParticleSystem Particles;

  public float speedFault = 1;

  public Color R_Clone_Color;

  public Color L_Clone_Color;

  public bool isMain = false;

  public GameObject MainClones;

  public BossCore bossCore;

  [SerializeField] private GameObject Axe;

  private TrailRenderer trail;

  [SerializeField] private float activeTime;

  [SerializeField] private Animator anim;

  [SerializeField] private Transform trs;

  [SerializeField] private MasterController player;

  [SerializeField] private float timebtwattacks;

  public bool isIdle, isEntry, isPinch, isAtk;

  [SerializeField] private Vector2 pPosition = Vector2.zero;

  [SerializeField] private Vector2 destination = Vector2.zero;

  [SerializeField] private float pd;

  [SerializeField] private Vector2 startpos;

  [SerializeField] private bool atkAfterTP = false;

  [SerializeField] private bool Whatatk; //verdadeiro para dash, falso para jogar machado

    void OnEnable()
  {
    GameEvents.StartListening("BossDie", Die);
        GameEvents.StartListening("BossDamaged", PlayParticles);
  }
  void OnDisable()
  {
    GameEvents.StopListening("BossDie", Die);
        GameEvents.StopListening("BossDamaged", PlayParticles);
  }

    void Start()
    { 

      trail = this.gameObject.GetComponent<TrailRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>();
        trs = GetComponent<Transform>();
        startpos = transform.position;
        isIdle = true;
        isAtk = false;
        pPosition = Vector2.zero;
        bossCore = trs.parent.gameObject.GetComponent<BossCore>();
        this.GetComponent<SpriteRenderer>().material.color = this.gameObject.GetComponent<SpriteRenderer>().color;
        Particles.startColor = this.gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    { 
      if(!isEntry)
      {
        activeTime += Time.deltaTime;
        if(bossCore.life <= bossCore.maxlife*2/5 && isMain && !isPinch)
        { 
          isPinch = true;
          Debug.Log(this.gameObject);
          SummonClones(); 
        }
        int i = 1;
          pd = PlayerXDistanceX(out pPosition);

          if(isIdle)
          { 

            destination = new Vector2(startpos.x + (Mathf.Sin(activeTime*2.5f)*10f)/speedFault, startpos.y + (Mathf.Sin(activeTime*5)*3 -2.5f)/speedFault);
            if(!isAtk)
            {
              trs.position = destination;
              if(Mathf.Abs(pd) > 7.5f)
              {
                i = Mathf.RoundToInt(Mathf.Sign(pd)) + 1;
              }
              anim.Play("Idle" + i);
              if(Mathf.Repeat(Time.time, timebtwattacks) < 0.03f)
              {
                Attack();
              }
            }
          }       
      }

    }




    public void Attack()
    { 
      isAtk = true;
      Whatatk = Convert.ToBoolean(UnityEngine.Random.Range(0, 2)); //verdadeiro para dash, falso para jogar machado
      bool willTp = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));

      if(willTp)
      { 
        atkAfterTP = true;
        Teleport();
        isIdle = true;
      }
      else
      { 
        atkAfterTP = false;
      Atkcheck();
      }
      
    }


    public void Atkcheck()
    {
      if(Whatatk)
      { 
        isIdle = false;
        Dash(); 
      }
      else
      {
        PrepareAxe();
      }
    }


    public void Dash()
    { 
      
      if(Mathf.Sign(pd) > 0)
      { 
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash1"))
        {
          anim.Play("Dash0");
          trs.eulerAngles = new Vector3(0, 0, -Vector2.Angle((new Vector2(trs.position.x, trs.position.y) - pPosition).normalized, Vector2.left));
        }
      }
      else
      { 
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash0"))
        {
          anim.Play("Dash1");
          trs.eulerAngles = new Vector3(0, 180, -Vector2.Angle((new Vector2(trs.position.x, trs.position.y) - pPosition).normalized, Vector2.right));
          trs.localScale = Vector3.left + Vector3.up + Vector3.forward;
        }
      }

    }

    public void StopDash()
    { 
      Vector2 stp = trs.position;

      Vector2 finalpos = trs.right.normalized*20 + trs.position;
      trs.position = finalpos;
      RaycastHit2D hit = Physics2D.Raycast(stp, trs.right.normalized, 20);
      if(hit)
      { 
        Debug.Log(hit.transform.gameObject);
        if(hit.transform.gameObject.tag.Equals("Player"))
        { 
          Debug.Log("hit");
          var hited = hit.transform.gameObject.GetComponent<MasterController>();
        if(hited)
        {
          hited.takedamage(bossCore.damage);
        }
        }
      }
      trs.localScale = new Vector3(1, 1, 1);
      trs.eulerAngles = Vector3.zero;
      isIdle = false;
      activeTime = 0.03f;
      atkAfterTP = false;

      

    }

    public void TPBack()
    {
      Teleport(new Vector2(startpos.x + Mathf.Sin(0.03f*2.5f)*10f, startpos.y + Mathf.Sin(0.03f*5)*2 -1.5f));
    }
    public void Teleport()
    { 
      isIdle = false;
      
      anim.Play("Teleport");
      destination = startpos + UnityEngine.Random.insideUnitCircle*4.5f;
    }
    public void Teleport(Vector2 dest)
    { 
      isIdle = false;
      anim.Play("Teleport");
      destination = dest;
    }

    public void InstanteTranslocate()
    { 
      trail.Clear();
      activeTime = 0.03f;
      trs.position = destination;
      trail.Clear();
    }


    public void CheckTeleportAtk()
    { 
      
      if(atkAfterTP)
      { 
        isIdle = false;
      Atkcheck();
      isAtk = true;

      }
      else
      { 
        isAtk = false;
        isIdle = true;
        activeTime = 0.03f;
      } 
    }


    public void PrepareAxe()
    {
      anim.Play("Trow");
    }

    public void TrowAxe()
    {
      AxeBehavior axe = Instantiate(Axe, trs.position, trs.rotation).GetComponent<AxeBehavior>();
      axe.Owner = this.gameObject;
      axe.bossPosition = trs.position;
      axe.target = pPosition;
      anim.speed = 0;
      axe.GetComponent<ShootBehavior>().dmg = bossCore.damage;
    }

    public void CatchAxe()
    { 
      anim.speed = 1;
      isIdle = false;
      activeTime = 0.03f;
      atkAfterTP = false;
    }


    private float PlayerXDistanceX(out Vector2 pos)
    { 
      pos = player.gameObject.transform.position;
      float distance = player.gameObject.transform.position.x - trs.position.x;
      return distance;
    }

    public void SummonClones()
    { 
      timebtwattacks = timebtwattacks*3/2;
      isPinch = true;
      GameObject RightClone = Instantiate(MainClones, startpos + (Vector2.right*10 + Vector2.down*4), new Quaternion(0,0,0,0), bossCore.gameObject.transform);
      InfectedNKBehavior R_code = RightClone.GetComponent<InfectedNKBehavior>();
      R_code.isPinch = true;
      R_code.isEntry = true;
        R_code.isIdle = false;
      RightClone.GetComponent<Animator>().Play("Spawn");
      R_code.startpos = RightClone.transform.position;
      RightClone.GetComponent<SpriteRenderer>().material.color = R_Clone_Color;
      RightClone.GetComponent<TrailRenderer>().startColor = R_Clone_Color;
      RightClone.GetComponent<TrailRenderer>().endColor = R_Clone_Color;
        RightClone.GetComponent<SpriteRenderer>().color = R_Clone_Color;
        R_code.Particles.startColor = R_Clone_Color;



      GameObject LeftClone = Instantiate(MainClones, startpos + (Vector2.left*10 + Vector2.down*4), new Quaternion(0,0,0,0), bossCore.gameObject.transform);
      InfectedNKBehavior L_code = LeftClone.GetComponent<InfectedNKBehavior>();
      L_code.isPinch = true;
      L_code.isEntry = true;
        L_code.isIdle = false;
      LeftClone.GetComponent<Animator>().Play("Spawn");
      L_code.startpos = LeftClone.transform.position;
      LeftClone.GetComponent<SpriteRenderer>().material.color = L_Clone_Color;
      LeftClone.GetComponent<TrailRenderer>().startColor = L_Clone_Color;
      LeftClone.GetComponent<TrailRenderer>().endColor = L_Clone_Color;
        LeftClone.GetComponent<SpriteRenderer>().color = L_Clone_Color;
        L_code.Particles.startColor = L_Clone_Color;
    }


  private void Die()
  { 
    anim.Play("Die");
    player.TogglePlayable(false);
        this.isEntry = true;
        KillClones();
  }

  private void Dead()
  { 
    this.gameObject.SetActive(false);
    GameEvents.ScreamEvent("BossDead");
    Destroy(this.gameObject);

  }

  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && bossCore.isdangerous)
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.takedamage(bossCore.damage);
    }

  }

  public void ToggleLifebar(int state)
  { 
    if(state > 0)
    {
      bossCore.showlifebar = true;
    }
    else
    {
      bossCore.showlifebar = false;
    }
    
  }
  
  public void TransferCorePositionx(float pox)
  {
    bossCore.trs.position = new Vector2(pox, bossCore.trs.position.y);
  }
  public void TransferCorePositiony(float poy)
  {
    bossCore.trs.position = new Vector2(bossCore.trs.position.x, poy);
  }

  public void StartAttack()
  {
    this.isEntry = false;
  }

    public void PlayParticles()
    {
        Particles.Play();
    }

    public void KillClones()
    {
        if(!this.isMain)
        {   
            anim.Play("KillClones");
        }
    }

    public void disapear()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        anim.speed = 0;
    }
}




public class BossTaeniaBehavior : MonoBehaviour
{ 
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

  [SerializeField] private GameObject Boulders;
  [SerializeField] private GameObject Glues;
  [SerializeField] private ParticleSystem Fire;

  [SerializeField] private int FireDamage;

  private Transform Player;
  public float atkCD;
    private float Cd;

    private GameObject bitedColun;

    private Vector3 originalposition;


    private Transform trs;  
    private Animator anim;
    private float life;
    private Vector3 destination = Vector3.zero;
    [SerializeField] private float speed = 50f;
  [SerializeField] private List<string> Attacks = new List<string>{"Bite","GlueSpit", "RockTrow", "Flame"};

    // Start is called before the first frame update
    void Start()
    { 
      Fire = Fire.GetComponent<ParticleSystem>();
      Fire.Stop();
        Player = GameObject.FindWithTag("Player").gameObject.GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
        anim = this.gameObject.GetComponent<Animator>();
        destination = new Vector3(724, -56);
        trs.position = new Vector3(724, -90);
        originalposition = destination;
        anim.Play("EntryAnimation");
    }

    // Update is called once per frame
    void Update()
    { 
      if(this.gameObject.GetComponent<BossCore>().life <= 0)
      {
        Fire.Stop();
        anim.Play("Die");
            Die();
      }
      if(Fire.emission.rateOverTimeMultiplier > 18.0f && Fire.isPlaying)
      {   
        Burner();
      }
      if(destination != trs.position)
      { 

        trs.position = Vector3.MoveTowards(trs.position, destination, Time.deltaTime*speed);
      }
        if(Cd < atkCD)
        { 
          if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
          {
            Cd += Time.deltaTime;
          }
          
        }
        else
        { 
          Cd = 0;

          anim.Play(Attacks[Mathf.RoundToInt(Random.Range(0, Attacks.Count))]);
        }
    }

    public void Spawn_boulders()
    {
      for (int i = 0; i < 3; i++ )
        {   
            Vector3 spawnlocal = new Vector3(Random.Range(688f, 760f), -9, 0f); 
            if(i == 0)
            {
                spawnlocal = new Vector3(Player.position.x, -9, 0f);
            }
            Instantiate(Boulders, spawnlocal, new Quaternion(0,0,0,0));
        }
    }
    public void SpitGlue()
    {
      for (int i = 0; i < 5; i++ )
        {   
            Vector3 spawnlocal = new Vector3(688+12*Mathf.RoundToInt(Random.Range(0f, 7f)), 0, 0f); 
            if(i == 3)
            {
                spawnlocal = new Vector3(Player.position.x, 0, 0f);
            }
            var glue = Instantiate(Glues, spawnlocal, new Quaternion(0,0,0,0));
        }
    }

    public void Positionate(float P_Y)
    {
      int startposition = Mathf.RoundToInt(Mathf.Sign(Random.Range(-1, 1)));
      speed = 25;
      destination = new Vector3(Mathf.RoundToInt((Player.position.x - 688)/12)*12 + 688, -66.6f + P_Y, 0f);

    }
    public void OriginReturn()
    {
      destination = originalposition;
    }
    public void Inflamate()
    { 
      if(Fire.isPlaying)
      {
        Fire.Stop();
      }
      else
      {
        Fire.Play();
      }
    }
    public void BurstFire(float spd)
    { 
      speed = spd;
      int dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - (trs.position.x -3.5f)));
      destination = new Vector3(destination.x + (30*dir), destination.y, 0f);
      if(destination.x > 760)
      {
        destination = new Vector3(760, destination.y, 0f);
      }
      else if(destination.x < 696)
      {
        destination = new Vector3(696, destination.y, 0f);
      }
    }


    public void Bite()
    {
      bitedColun = Physics2D.Raycast(trs.position, Vector2.down, 50f).collider.gameObject;
      if(bitedColun != null)
      {
        if(bitedColun.tag.Equals("Player"))
        {
          bitedColun.GetComponent<MasterController>().takedamage(20);
        }
      }
      
    }
    public void Pull(float Intesity)
    {
      if(bitedColun != null)
      {
        if(bitedColun.tag.Equals("Scenary"))
        {
          bitedColun.GetComponent<BossMovableTerrain>().GetPulled(Intesity);
        }
      }
    }
    public void Drop()
    {
      if(bitedColun != null)
      {
        bitedColun = null;
      }
    }
    public void Burner()
    { 

      var FireHit = Physics2D.Raycast(Fire.gameObject.GetComponent<Transform>().position + Vector3.down, Vector3.down, 50f).collider.gameObject;
      Debug.DrawLine(Fire.gameObject.GetComponent<Transform>().position + Vector3.down, Fire.gameObject.GetComponent<Transform>().position + Vector3.down*5, Color.red);
      if(FireHit != null)
      {
        if(FireHit.tag.Equals("Player"))
        {
          FireHit.GetComponent<MasterController>().takedamage(FireDamage);
        }
      }
    }
    public void Die()
    { 
      Fire.Play();
      destination += Vector3.down*50;
      speed = 12;
        CardIndex enemyCard = new CardIndex(enemyID, realImg, inGameImg, encounterLocal, enemyBehavior, enemyName, realInfo);
        BestiaryElements.AddCardEnemy(enemyCard);
        PontuationCounter.AddScore(3000);
    }
    public void Dead()
    {
      GameEvents.ScreamEvent("FinalBossIsDead");
        GameEvents.ScreamEvent("BossDead");
        Debug.Log("BossDead");
      Destroy(this.gameObject);
    }
}




public class BoulderFall : MonoBehaviour
{
    public float falltime = 1;

    public float speed = 5;

    [SerializeField] private bool grounded = false;

    private ParticleSystem particles;

    private Animator anim;

    [SerializeField] private GameObject dangerSign;

    private GameObject sign_instance;

    [SerializeField] private List<GameObject> SpawnableMonsters;

    [SerializeField] private AudioInterface a;

    void Start()
    {   
        a = this.gameObject.GetComponent<AudioInterface>();
      anim = this.gameObject.GetComponent<Animator>();
        sign_instance = Instantiate(dangerSign, new Vector3(this.gameObject.GetComponent<Transform>().position.x, -65, 0), new Quaternion(0,0,0,0));
    }

    // Update is called once per frame
    void Update()
    { 
      if(grounded)
      {      
        anim.Play("BoulderCrash");
      }
        else if(falltime >= 0)
        {
          falltime -= Time.deltaTime;
        }
        else
        {
          if(sign_instance != null)
          {
            Destroy(sign_instance);
          }
          this.gameObject.GetComponent<Transform>().Translate(Vector3.down * Time.deltaTime * speed, Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
      if (other.gameObject.tag.Equals("Player")) //vai ver se o objeto que entrou na caixa de colisão é o jogador e se o inimigo não está atordoado
    {   
            
            a.PlaySound("blast");
      var hited = other.gameObject.GetComponent<MasterController>();//vai receber a classe de controle geral, que tem em todos os jogadores
      hited.takedamage(20); // chama o método que faz o jogador tomar dano, dentro do código dele
    }
    if(!other.gameObject.tag.Equals("Boss") && !other.gameObject.tag.Equals("hitable"))
    {
      grounded = true;
            a.PlaySound("blast");
      if(Random.Range(0f, 1f) > 0.75f && SpawnableMonsters.Count > 0)
      {    
                
        GameObject monster = Instantiate(SpawnableMonsters[Mathf.RoundToInt(Random.Range(0, SpawnableMonsters.Count))], this.gameObject.GetComponent<Transform>().position + Vector3.up*1.5f, new Quaternion(0,0,0,0));
        monster.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5, 5), -5), ForceMode2D.Impulse);
      }
    }


      
    }

    void end()
    {
      Destroy(this.gameObject);
    }
}




public class MiniBossTaenia : MonoBehaviour
{     
    public float atkCD;
    private float Cd;
    private Animator anim;
    private Transform trs;
    private Transform Player;
    [SerializeField] private GameObject Body;
    private Transform b_trs;

    [SerializeField] private GameObject Boulders;

    public int atkq = 0;
    // Start is called before the first frame update
    void Start()
    {      
        Cd = -1; 
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
        anim = this.gameObject.GetComponent<Animator>();
        //Player.GetComponentInChildren(typeof(SpriteRenderer)).gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.625f, 0.625f);
        b_trs = Body.GetComponent<Transform>();

    }
    void OnEnable()
    {   
        transform.position += Vector3.down * 20;

    }
    // Update is called once per frame
    void Update()
    {   

        if(trs.position.y < -55)
        {
            trs.Translate(Vector3.up*5*Time.deltaTime);
            b_trs.localPosition = Vector3.down * 60;
            
        }
        else
        {
            Body.SetActive(true);
            if(b_trs.localPosition.y < -15)
            {   
                Body.GetComponent<Animator>().Play("wall-sand");
                b_trs.Translate(Vector3.up*30*Time.deltaTime);
            }
            else if(!Body.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("notsand"))
            {   
                Body.GetComponent<Animator>().Play("wall-staytrans");
            }
            else if(Body.GetComponent<CombatEnemy>().life > 0)
            {
                if(Cd < atkCD)
                {
                    Cd += Time.deltaTime;
                }
                else
                {   
                    Cd = 0;
                    anim.Play("Atk");
                }
            }
        }
    }

    void Spawn_boulders()
    {
        for (int i = 0; i < atkq; i++ )
        {   
            Vector3 spawnlocal = new Vector3(Random.Range(450f, 478f), -29, 0f); 
            if(i == 0)
            {
                spawnlocal = new Vector3(Player.position.x, -29, 0f);
            }
            Instantiate(Boulders, spawnlocal, new Quaternion(0,0,0,0));
        }
    }
}




public class HitableWalls : MonoBehaviour
{
    // Start is called before the first frame update
  private Animator anim;
  private CombatEnemy master;

  void Start()
  {
    anim = this.gameObject.GetComponent<Animator>();
    master = this.gameObject.GetComponent<CombatEnemy>();
  }

    // Update is called once per frame
    void Update()
    {
        if(master.life <= 0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("wall-dead"))
        {
          anim.Play("wall-dead");
        }
        if(master.stuncheck() && !anim.GetCurrentAnimatorStateInfo(0).IsName("wall-dead"))
        {
          anim.Play("wall-damage");
        }
    }
}




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
  {               //^^^^^ other é o componente do colisor que entrou no gatilho do inimigo

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
        { //                                 v
          rigb.velocity = new Vector2(0.0f, rigb.velocity.y); //fica parado, corno
        }
      }

      if(loadtime <= 0.0f) //se o tempo de carregamento do ataque acabou
      { 
        AudioInterface a = this.gameObject.GetComponent<AudioInterface>();

        a.PlaySound("dash");
        rigb.AddForce(new Vector2(speed*4*dir, 0.0f), ForceMode2D.Impulse);//empurre o inimigo na direção do jogador, muito rápido

        anim.SetBool("Charge", false); //"já parou de carregar, não precisa mais tocar a animação de carregamento"

        loadtime = load; // reinicia o tempo de carregamento do ataque

        alratack = true;// o inimigo acabou de atacar, não dá pra ele começar a carregar o ataque de novo até ele ficar parado e na distância certa(linha 116)
      }

  }
  
}







public class Staph2Behavior : MonoBehaviour {



  private Transform Player;
  public Transform ShootLocal;
  private Transform trs;
  private Animator anim;
  private Vector3 PlayerDistance;
  public float range;
  private float dir;
  private Vector2 PdMod;
  public bool Charge = false;
  private float loadtime;
  public float load;
  public float reload;
  private float reloadtime;
  public bool stuned;
  public GameObject shoot;





  // Use this for initialization
  void Start () {

    
    trs = GetComponent<Transform> ();
    loadtime = load;
    reloadtime = 0f;
    anim = GetComponent<Animator> ();


    
  }
  
  // Update is called once per frame
  void Update () {
    Player = GameObject.FindGameObjectWithTag("Player").transform;
    var combat = GetComponent<CombatEnemy>();
    stuned = combat.stuncheck();
    anim.SetBool("Stunned", stuned);

    PlayerDistance = Player.transform.position - trs.position;

    PdMod = new Vector2 (Mathf.Abs(PlayerDistance.x), PlayerDistance.y);
    if (!Charge){dir = PlayerDistance.x/PdMod.x;}

    if(reloadtime > 0f){reloadtime -= Time.deltaTime;}
    


    if(PdMod.x < range && !stuned)
    {
      if(PdMod.x < range && PdMod.y < range/4 && reloadtime <= 0f){Attack();}
      else{
        Charge = false;
        loadtime = load;
        anim.SetBool("Charge", false);
        }
      
    }
    

    trs.rotation = new Quaternion(trs.rotation.x, 90f-(dir*90), 0, trs.rotation.w);
    
    }
  

  void Attack(){
      Charge = true;
      loadtime -= Time.deltaTime;
      anim.SetBool("Charge", true);
      if(stuned){loadtime = load; anim.SetBool("Charge", false); }
      if(loadtime <= 0.0f)
      {
        anim.SetBool("Charge", false);
        loadtime = load;
        reloadtime = reload;
        Instantiate(shoot, ShootLocal.position, ShootLocal.rotation);

      }

  }
  
}




public class Staph3Behavior : MonoBehaviour {



  private Transform Player;
  private Transform trs;
  private float PlayerDistance;
  public float range;
  public float speed;
  private float dir;
  private Vector2 PdMod;
  private Vector2 POriginal;


  // Use this for initialization
  void Start () {

    Player = GameObject.FindGameObjectWithTag("Player").transform;
    trs = GetComponent<Transform> ();
    POriginal = trs.position;

    
  }
  
  // Update is called once per frame
  void Update () {


    PlayerDistance = Vector2.Distance(Player.position, trs.position);

    dir = PlayerDistance/Mathf.Abs(PlayerDistance);
    


    if(PlayerDistance < range)
    {
      trs.position = Vector2.MoveTowards(trs.position, Player.position, speed* Time.deltaTime);
    }
    else 
    {
      trs.position = Vector2.MoveTowards(trs.position, POriginal, speed* Time.deltaTime);     
    }
      


    

    trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
    
    }
  


  
}







public class AmebaBehavior : MonoBehaviour {

  //declaração das variáveis:

  public float walktime;

  private float wkt = 0f;

  public float speed;

  public Transform Eye; //declaração para receber a classe das coordenadas do jogador

  private Transform trs; //declaração para receber a classe das coordenadas do inimigo
  
  private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

  private Animator anim; //variável que receberá o valor do componente de animação (a Classe Animator) do inimigo

  private CombatEnemy combat;

  [SerializeField] private bool ststill;


  private RaycastHit2D detectedobj;

  // Use this for initialization
  void Start () //método padrão do unity que roda no início da fase/cena
  { 
    wkt = walktime;

    
    trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

    rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
    
    anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

    combat = GetComponent<CombatEnemy>(); //vai buscar aquele script genérico para todos os inimigos, que tem os métodos pra tomar dano e oscar alho

  }
  
  
  void Update () // Update is called once per frame (uma vez por frame, esse método aqui é chamado)
  { 

    if(combat.life < 1)
    {
      anim.Play("Dead");
      if(Mathf.Abs(rigb.velocity.x/5) >= 0.6)
      {
        rigb.constraints = RigidbodyConstraints2D.FreezeRotation;
      }
      else
      {
        rigb.constraints = RigidbodyConstraints2D.None;
      }
      
      combat.KnBackIntensity = new Vector2(2f, 2f);
      anim.SetFloat("RollSpeed", -1*rigb.velocity.x/5);
    }
    else
    { 
      detectedobj = Physics2D.Raycast(new Vector2(Eye.position.x, Eye.position.y) , Vector2.left * Mathf.Sign(trs.localScale.x), Mathf.Infinity, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);

      if(combat.stuncheck())
      {
        anim.Play("Stun");
      }

      if(detectedobj.distance <= 2f && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over"))
      {
        if(detectedobj.collider.gameObject.CompareTag("Player"))
        {
          anim.Play("Attack");
          rigb.velocity = new Vector2(0f, rigb.velocity.y);
        }
        else if(((detectedobj.collider.gameObject.CompareTag("Scenary") || detectedobj.collider.gameObject.CompareTag("hitable")) && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over")) || ststill)
        {
          //Flip();
          rigb.velocity = new Vector2(0f, rigb.velocity.y);
          anim.Play("Idle");
          wkt -= Time.deltaTime;
        }
      }
      else if(wkt >= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over") && !ststill)
      { 
        rigb.velocity = new Vector2(-Mathf.Sign(trs.localScale.x) * speed, rigb.velocity.y);

        anim.Play("Walk");
        wkt -= Time.deltaTime;
      }

      if(wkt <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Over") && !ststill)
      { 
        
        Flip();
        wkt = walktime;
      }

    }
    

    //stuned = combat.stuncheck(); //vai verificar se o inimigo tomou atordoamento
  
  }

  void OnCollisionEnter2D(Collision2D other)//método de ataque
  {
    if(other.gameObject.tag.Equals("hitable") && rigb.velocity.x > 20 && combat.life <= 0)
    {
        var hited = other.gameObject.GetComponent<CombatEnemy>();
        hited.takedamage(50, rigb.velocity);
    }
  }
  public void Flip()
  {
    trs.localScale = trs.localScale = new Vector2 (-trs.localScale.x, trs.localScale.y);
  }
}




public class BabyTaeniaBehavior : MonoBehaviour
{
    private Rigidbody2D rigb;
    private RaycastHit2D hitb;

    [SerializeField] private Transform trs;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spr;
  [SerializeField] private LayerMask solid;
  
  [SerializeField] private Transform Player;

  [SerializeField] private Vector2 localupside = Vector2.up;
  [SerializeField] private Vector2 dir = new Vector2(1, 1);

  private float wtime = 3f;
  private bool llwalk; 



  [SerializeField] public float atk_delay;
  public float speed;
  public float range;



    void Start()
    { 
      Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rigb = this.gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    { 
      if(wtime <= 0f)
      {
        wtime = 3f;
        llwalk = (Random.value > 0.5f);

      }
      else
      {
        wtime -= Time.deltaTime;
        
      }
      
        rigb.velocity -= localupside*1f;
      

        if(Mathf.Abs(Player.position.x - trs.position.x) <= range && Mathf.Abs(Player.position.y - trs.position.y) <= range && spr.isVisible)
        { 
          if(!Physics2D.Linecast(Player.position, trs.position, solid) )
        {
          dir = new Vector2(Mathf.Sign(trs.position.x - Player.position.x), Mathf.Sign(Player.position.y - trs.position.y));
        }
        else
        {
          dir = new Vector2(Mathf.Sign(trs.position.x - Player.position.x), 1);
        }
          trs.localScale = new Vector3(Mathf.Abs(trs.localScale.x)*dir.x, trs.localScale.y, 0f);

          if(atk_delay <= 0)
          {
            if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
            {
              anim.Play("attack");
            }
          }
          else if(llwalk == false)
          { 
            atk_delay -= Time.deltaTime;
              anim.Play("Idle");
            rigb.velocity = Vector2.zero;
          }
          else
          { 
            atk_delay -= Time.deltaTime;
            anim.Play("walk");
            if(Mathf.Abs(rigb.velocity.x) < speed && Mathf.Abs(rigb.velocity.y) < speed)
            {
              rigb.velocity += Vector2.Scale(Vector2.Perpendicular(localupside), dir)*speed;
            }
          }         
        }
        else
        {
          anim.Play("Idle");
          rigb.velocity = Vector2.zero;
        }
       
    }


    void OnCollisionExit2D(Collision2D col)
    {
      if(col.gameObject.tag == "Scenary")
      { 
        rigb.velocity -= localupside*5f;
        //localupside = Vector2.up;
        //trs.eulerAngles = new Vector3(0, 0, 0);
      }
    }
    public void OnCollisionStay2D(Collision2D col)
    {
      if(col.gameObject.tag == "Scenary")
      {
        localupside = col.GetContact(col.contactCount-1).normal;
        trs.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(col.GetContact(col.contactCount-1).normal, Vector2.up));
      }
    }

    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, range);
  }
}




public class SmallerTaeniaShot : MonoBehaviour
{ 
  private Transform trs;
  public GameObject Shot;
  [SerializeField] private Transform Player;
  [SerializeField] private BabyTaeniaBehavior maincode;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
  {
    GameObject shoot = Instantiate(Shot, trs.position, trs.rotation);
    maincode.atk_delay = 4;
    shoot.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, Player.position - trs.position));

    shoot.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    shoot.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;

    shoot.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 7;
    shoot.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;

    shoot.transform.localScale = new Vector3(12, 12, 0);

    shoot.gameObject.GetComponent<ShootBehavior>().speed = 15;

    shoot.gameObject.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.02f, 0f);
    shoot.gameObject.GetComponent<CapsuleCollider2D>().size = new Vector2(0.05f, 0.06f);


  }
}




public class Detector : MonoBehaviour
{ 
  public string enteredobject = "";

      private void OnTriggerEnter2D(Collider2D other)
      {
        enteredobject = other.gameObject.tag;
      }
      private void OnTriggerExit2D(Collider2D other)
      {
        enteredobject = "";
      }


}




public class DeathBehavior : MonoBehaviour
{
    private Transform trs;
    private Vector3 startPos;
    public float range;

    private Transform Player;
    private bool retreating;

    private Vector2 destination;
    public float speed;

    private Animator anim;

    void Start()
    { 
      anim = this.gameObject.GetComponent<Animator>();
      Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
        startPos = trs.position;
        destination = startPos;
    }

    // Update is called once per frame
    void Update()
    { 
      float dir = Player.position.x - trs.position.x;
      float pdistance = Vector2.Distance(trs.position, Player.position);

      trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*Mathf.Sign(dir), trs.localScale.y);

      if(pdistance < range && !retreating)
      {
        MoveAdvance();
        anim.Play("attack");
      }
      if(trs.position == startPos)
      {
        retreating = false;
      }
      else if(retreating)
      {
        MoveBack();
      }
      trs.position = destination;

    }
    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, range);
  }

  void MoveAdvance()
  {
    destination = Vector2.MoveTowards(trs.position, Player.position + Vector3.right*Player.gameObject.GetComponent<MasterController>().movSen, speed* Time.deltaTime);
  }
  void MoveBack()
  { 
    retreating = true;
    destination = Vector2.MoveTowards(trs.position, startPos, 5*speed* Time.deltaTime);
  }
}




public class Taenia1Behavior : MonoBehaviour
{
  private Transform Player; //declaração para receber a classe das coordenadas do jogador

  private Transform trs; //declaração para receber a classe das coordenadas do inimigo
  
  private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

  private Animator anim; //variável que receberá o valor do componente de animação (a Classe Animator) do inimigo

  private Vector3 PlayerDistance; //vetor resultante do cálculo para achar a distância entre o jogador e o inimigo

  public Vector2 range; 

  public float speed; //velocidade de movimento desse inimigo

  private float dir = -1; //direção em que o inimigo está olhando

  private CombatEnemy combat;

  public bool hooked;

  [SerializeField] private float hookcd = 0f;





  // Use this for initialization
  void Start () //método padrão do unity que roda no início da fase/cena
  {

    Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
    
    trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

    rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
    
    anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

    combat = this.gameObject.GetComponent<CombatEnemy>();
  }

    // Update is called once per frame

    void Update()
    { 
      if(!combat.stuncheck() )
      { 
        PlayerDistance = Player.transform.position - trs.position;

        if(Mathf.Abs(PlayerDistance.y) < range.y)
        {
          if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk"))
          {
            trs.localScale = new Vector3(Mathf.Abs(trs.localScale.x) * -dir, trs.localScale.y, trs.localScale.z);
            dir = Mathf.Sign(PlayerDistance.x);
          


          if(Mathf.Abs(PlayerDistance.x) < range.x*1/4 )
          { 
              Debug.Log("atk");

              anim.Play("lil taenia Melee");
              rigb.velocity = new Vector2(0f, 0f);
          }
          else if(Mathf.Abs(PlayerDistance.x) < range.x/2)
          { 
            if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("atk") && hookcd <= 0f)
            { 
              Debug.Log("hook");

              anim.Play("lil taenia Hook");
            }
            
          }
          else if(Mathf.Abs(PlayerDistance.x) < range.x)
          { 
            Debug.Log("walk");
            anim.Play("lil taenia Walk");
            rigb.velocity = new Vector2(speed*dir, rigb.velocity.y);  
          }
      
          else
          { 
            GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>().TogglePlayable(true);
            anim.Play("lil taenia Idle");
            rigb.velocity = new Vector2(0f, rigb.velocity.y);
            
          }
          hookcd = Mathf.Abs(hookcd) - Time.deltaTime;
        }
        }
      }
      else
      { GameObject.FindWithTag("Player").gameObject.GetComponent<MasterController>().TogglePlayable(true);
        anim.Play("lil taenia takedamage");
 
      }
    }


    public void grabPlayer()
    {
      RaycastHit2D hit;

      if(hooked)
      {
        hooked = false;
      hookcd = 2;
      }
      else
      {
        hit = Physics2D.Raycast(new Vector2(this.transform.position.x + 4*dir, this.transform.position.y - 3), new Vector2(dir, 0), 7f);
        
        if(hit)
        { 

          if(hit.collider.CompareTag("Player"))
          { 
            MasterController hited = hit.collider.gameObject.GetComponent<MasterController>();
            hited.trs.position = new Vector2(this.transform.position.x + 3*dir, this.transform.position.y - 3);
            hited.anim.Play("takedmg");
          hited.isGrounded = false;
            hooked = true;
            
          }
          }

      }
    }
}




public class SkeletonBehavior : MonoBehaviour
{ 
    private Transform Player;
  private Transform trs;
  private Animator anim;
  private Rigidbody2D rigb;
  [SerializeField] private Transform ShootLocal;

  private bool isGrounded;
  private int dir;
  [SerializeField] private float jumpforce;

  [SerializeField] private float range;
  public GameObject shoot;
    // Start is called before the first frame update
    void Start()
    {
    Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();  
    trs = this.gameObject.GetComponent<Transform>();
    anim = GetComponent<Animator>(); 
    rigb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - trs.position.x));
        float hdif = Mathf.Abs(Player.position.y - trs.position.y);
        float pdistance = Vector2.Distance(trs.position, Player.position);
        trs.localScale = new Vector2 (Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
        if(isGrounded)
        { 

          if(Mathf.Abs(pdistance) < range && hdif < 5)
          { 

            if(pdistance >= range/4)
            {
              atk();
            }
            else
            {
              jump();
            }
          }
          else
          {
            idle();
          }
        }
       
    }
    void atk()
    {
      if(isGrounded)
      {
        anim.Play("attack");
        rigb.velocity = new Vector2(0, rigb.velocity.y);      
      }

    }
    void jump()
    { 
        AudioInterface a = this.gameObject.GetComponent<AudioInterface>();
      if((!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck()) && isGrounded)
      { 
        rigb.AddForce(new Vector2(-dir*jumpforce/2, jumpforce), ForceMode2D.Impulse);
        isGrounded = false;
            a.PlaySound("jump");
      }
    }
    void idle()
    { 
      if((!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck()) && isGrounded)
      { 
        anim.Play("Idle");
        rigb.velocity = new Vector2(0, rigb.velocity.y);
      }
    }

    void shot()
    {
      GameObject projectile = Instantiate(shoot, ShootLocal.position, ShootLocal.rotation);
      projectile.transform.eulerAngles = new Vector3(0 , 90  -dir*90 , 0);
    }

    void OnCollisionStay2D(Collision2D col)
    { 
      if(this.gameObject.GetComponent<SpriteRenderer>().isVisible)
      {
        ContactPoint2D[] contacts = new ContactPoint2D[3];
          if(col.GetContacts(contacts) > 0)
          {
              foreach(ContactPoint2D cn in contacts)
              {
                if(cn.normal.y >= 0.5)
                {
                  isGrounded = true;
                }
              }
          }       
      }

    }
    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(this.transform.position + Vector3.right*Mathf.Sign(this.transform.localScale.x)*range/2, new Vector3(range, 5, 0));
  }
}




public class ZombieBlast : MonoBehaviour
{
    private CombatEnemy thisZombie;

    [SerializeField] private GameObject blast;
    void Start()
    {
      thisZombie = GetComponent<CombatEnemy>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(thisZombie.life <= 0)
        {
          Instantiate(blast, this.gameObject.transform.position + Vector3.down*this.gameObject.transform.localScale.y*0.3f, this.gameObject.transform.rotation);
          Destroy(this.gameObject);
        }
    }
}




public class ZombieScript : MonoBehaviour
{ 
    private Transform Player;
  private Transform trs;
  private Animator anim;
  private Rigidbody2D rigb;

  private Transform ptrs;

  [SerializeField] private float speed;

  [SerializeField] private float range;
  [SerializeField] private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
    Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();  
    trs = this.gameObject.GetComponent<Transform>();
    anim = GetComponent<Animator>(); 
    rigb = GetComponent<Rigidbody2D>();
    ptrs = particles.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        int dir = Mathf.RoundToInt(Mathf.Sign(Player.position.x - trs.position.x));
        float hdif = Mathf.Abs(Player.position.y - trs.position.y);
        float pdistance = Vector2.Distance(trs.position, Player.position);
        trs.localScale = new Vector2 (-Mathf.Abs(trs.localScale.x)*dir, trs.localScale.y);
        ptrs.localScale = new Vector2 (-dir, 1);
        if(pdistance <= 3 && hdif < 3)
        {
          atk();
        }
        else if(pdistance < range && hdif < 3)
        {
          walk(dir);
        }
        else
        {
          idle();
        }
    }
    void atk()
    {
      anim.Play("attack");
      rigb.velocity = new Vector2(0, rigb.velocity.y);
    }
    void walk(int dir)
    { 
      if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
      {
        anim.Play("walk");
        rigb.velocity = Vector2.right*dir*speed + new Vector2(0, rigb.velocity.y);
      } 
    }
    void idle()
    { 
      if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") || this.gameObject.GetComponent<CombatEnemy>().stuncheck())
      {
        anim.Play("Idle");
        rigb.velocity = new Vector2(0, rigb.velocity.y);
      }
    }

    public void PlayParticles()
    {
      particles.Play();
    }
    public void StopParticles()
    {
      particles.Stop();
    }
    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(this.transform.position + Vector3.left*Mathf.Sign(this.transform.localScale.x)*range/2, new Vector3(range, 3, 0));
  }
}




public class FlyTrypBehavior : MonoBehaviour
{
  [SerializeField] private float atkdelay;

  [SerializeField] private float ad;

  private CombatEnemy mainCode;

  private Transform Player; //declaração para receber a classe das coordenadas do jogador

  private Transform trs; //declaração para receber a classe das coordenadas do inimigo
  
  private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

  private Animator anim;

  private SpriteRenderer spr;

  [SerializeField] private Vector3 stPos;

    [SerializeField] private float speed;

    [SerializeField] private float range;

  [SerializeField] private float flydistance;

  [SerializeField] private bool advancing;

    void Start()
    { 

        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
    
    trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

    rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
    
    anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

    spr = GetComponent<SpriteRenderer> ();

    mainCode = GetComponent<CombatEnemy>();

    stPos = trs.position;
    } 

    // Update is called once per frame
    void Update()
    { 
      bool stun = mainCode.stuncheck();
      
      if(!stun)
      { 
        rigb.velocity = Vector2.zero;

        if(ad > 0)
        {
          ad -= Time.deltaTime;
        }
        float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
        if(PlayerDistance <= range && anim.GetCurrentAnimatorStateInfo(0).IsName("Fly") && ad <= 0)
        {
          advancing = true;
          anim.Play("Dash");
        }
        Vector3 frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*flydistance, Time.fixedDeltaTime*speed);
        if(advancing)
        {
          frameTranslocation = Vector2.MoveTowards(trs.position, Player.position, Time.deltaTime*speed*3);
          trs.localEulerAngles = Vector2.up * (90 - 90*Mathf.Sign(trs.position.x - Player.position.x));
        }
        else
        { 
          if(trs.position.y == stPos.y)
          {
            anim.Play("Fly");
          }
          else
          {
            frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*flydistance, Time.fixedDeltaTime*speed*3);
          }
        }
      trs.position = frameTranslocation;
      if(stPos - trs.right*flydistance - trs.position == Vector3.zero && anim.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
      { 
                      
        trs.localEulerAngles += Vector3.up*180;

        trs.position = Vector2.MoveTowards(trs.position, stPos + trs.right*flydistance, Time.deltaTime*speed*4);
        }
    }
    else
    {
      anim.Play("Stun");
    }
    }

    public void Retreat()
    {
      advancing = false;
      ad = atkdelay+1;
    } 
    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(this.transform.position, range);
    Gizmos.color = Color.blue;
    if(stPos != Vector3.zero)
    {
      Gizmos.DrawLine(stPos + this.transform.right*flydistance, stPos - this.transform.right*flydistance);
    }
    else
    {
      Gizmos.DrawLine(this.transform.position + this.transform.right*flydistance, this.transform.position - this.transform.right*flydistance);
    }
  }
}




public class JpTrypBehavior : MonoBehaviour
{ 

  [SerializeField] private LayerMask solid;

  [SerializeField] private CombatEnemy mainCode;

  private Transform Player; //declaração para receber a classe das coordenadas do jogador

  private Transform trs; //declaração para receber a classe das coordenadas do inimigo
  
  private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

  private Animator anim;

  private SpriteRenderer spr;

  private CircleCollider2D col;

    [SerializeField] private float jumpForce;

    [SerializeField] private float range;

    private float dir = 0;

    [SerializeField] private bool Grounded;

    private AudioInterface a;

 // Start is called before the first frame update
    void Start()
    { 
      a = this.gameObject.GetComponent<AudioInterface>();
        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
    
    trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

    rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
    
    anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

    spr = GetComponent<SpriteRenderer> ();

    mainCode = GetComponent<CombatEnemy>();

    col = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
      bool stuned = this.mainCode.stuncheck();
      if(!stuned)
      {
        Grounded = Physics2D.OverlapCircle(col.bounds.center, col.radius*6f, solid) && rigb.velocity.y <= 0;
          
          if(Grounded)
          { 
            dir = searchPlayer();
            trs.eulerAngles = Vector3.zero;

            if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("trans"))
            { 
              if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
              {
                anim.Play("land");
                a.PlaySound("slime");
              }
              else
              { 
                anim.Play("Idle");
              }
            }
            else
            {
              rigb.velocity = rigb.velocity.x/2*Vector2.right + Vector2.up*rigb.velocity.y;
            }

          }
          else
          { 
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
              anim.Play("Jump");
              a.PlaySound("slime");
            }
            
            trs.eulerAngles = -dir*Vector3.forward*60 + Vector3.forward* Mathf.Clamp(30, -45, rigb.velocity.y*4)*dir;
          }
      }
      else
      {
        anim.Play("Stun");
      }

    }

    public float searchPlayer()
    {
      float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
      if(PlayerDistance <= range)
    { 
      anim.Play("JumpStill");
        return Mathf.Sign(Player.transform.position.x -trs.position.x);
    }
    return 0;
    }


    public void jumpGo()
    {
      anim.Play("Jump");
      rigb.velocity = Vector2.up*jumpForce + Vector2.right*dir*jumpForce/2;
    }

    void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, range);
  }
}




public class SwimingTrypBehavior : MonoBehaviour
{ 
  private float stunedTime;

  private SwimmingTypes swCode;

  private CombatEnemy mainCode;

  private Transform Player; //declaração para receber a classe das coordenadas do jogador

  private Transform trs; //declaração para receber a classe das coordenadas do inimigo
  
  private Rigidbody2D rigb; //variável que receberá o valor do componente de física (a Classe RigidBody2D) do inimigo

  private Animator anim;

  private SpriteRenderer spr;

  [SerializeField] private Vector3 stPos;

    [SerializeField] private float speed;

    [SerializeField] private float range;

    [SerializeField] private bool insideWater = false;

  [SerializeField] private float swimdistance;

  [SerializeField] private bool jumperEnemy;

  [SerializeField] private bool grabed;

    void Start()
    { 

        Player = GameObject.FindGameObjectWithTag("Player").transform; //busca por um objeto com a tag "Player" (tag = marcador/categoria) e coloca suas cordenadas na variável Player.
    
    trs = GetComponent<Transform> (); //acessa as coordenadas do inimigo e coloca dentro da variável trs

    rigb = GetComponent<Rigidbody2D> ();//this.gameObject vai acessar o objeto em que esse script foi colocado e, com o método GetComponent<RigidBody2D>() vai pegar o componente de física dele e jogar para a variável rigb;
    
    anim = GetComponent<Animator> ();// acessa o componente de animação (a Classe Animator) do inimigo e joga pra dentro da variável "anim"

    spr = GetComponent<SpriteRenderer> ();

    mainCode = GetComponent<CombatEnemy>();

    swCode = GetComponent<SwimmingTypes>();

    stPos = trs.position;
    } 
    // Update is called once per frame
    void FixedUpdate()
    { 

      if(!mainCode.stuncheck())
      { 
        stunedTime = 0;
        if(!grabed)
        { 
          spr.sortingOrder = 5;
          insideWater = swCode.Getinwater();
          if(!insideWater)
          { 

            rigb.isKinematic = false;
            rigb.gravityScale = 40;
              anim.SetFloat("Rspeed", -rigb.velocity.x);
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("swim"))
              {
                anim.Play("RollEntry");
              }
              else if(anim.GetCurrentAnimatorStateInfo(0).IsName("stun"))
              {
                anim.Play("Roll");
              }
            float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
            float speedFactor = (1 - Mathf.Abs(rigb.velocity.x)/(speed*10));
              if(PlayerDistance <= range)
              {
                rigb.velocity += Vector2.right * Mathf.Sign(Player.transform.position.x -trs.position.x) * speed * speedFactor;


              }
              else if(Mathf.Abs(rigb.velocity.x) > 0)
              {
                rigb.velocity = Vector2.zero + Vector2.up*rigb.velocity.y;
              }  
              trs.eulerAngles -= Vector3.forward*rigb.velocity.x*2 + Vector3.up*trs.localEulerAngles.y;   
          }
          else
          { 
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("stun"))
            { 
              trs.localEulerAngles -= Vector3.forward*trs.localEulerAngles.z;
            rigb.isKinematic = true;
              rigb.gravityScale = 0;
              float PlayerDistance = new Vector2(Player.transform.position.x - trs.position.x, Player.transform.position.y - trs.position.y).magnitude;
              if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swim"))
              { 
                rigb.velocity = Vector2.zero;
                anim.Play("swim");
              }
              
              Vector3 frameTranslocation = Vector2.MoveTowards(trs.position, stPos - trs.right*swimdistance, Time.fixedDeltaTime*speed*4);
                trs.position = frameTranslocation;
                if(stPos - trs.right*swimdistance - trs.position == Vector3.zero)
                { 
                  
                  trs.localEulerAngles += Vector3.up*180;

                  trs.position = Vector2.MoveTowards(trs.position, stPos + trs.right*swimdistance, Time.fixedDeltaTime*speed*4);
                }

                if(PlayerDistance <= range*2/3 && jumperEnemy && Player.position.y > trs.position.y)
                { 
                  trs.localEulerAngles -= Vector3.forward*90;
                  rigb.velocity = Vector2.up * 30;
                }
            }
            
            }
        
        }
        else
        { 

          rigb.isKinematic = true;
          trs.position = Player.position + Vector3.down;
          anim.Play("Grab");
          spr.sortingOrder = 55;
          if(InputManager.instance.GetButtonDown("spec"))
          { 
            spr.sortingOrder = 5;
            grabed = false;
            Player.gameObject.GetComponent<MacroBehavior>().hand.GetComponent<MacroHand>().SearchEnemies(this.gameObject);
          }

        }
      }
      else
      { 
        rigb.isKinematic = false;
        
        
        
        if(stunedTime > 1.5f)
        {
          mainCode.SetStuned(0);
          if(Player.gameObject.GetComponent<MacroBehavior>().grabed)
          {
            Player.gameObject.GetComponent<MacroBehavior>().ToggleGrabOn();
            grabed = true; 
          }

        } 
        else
        { 
          trs.eulerAngles -= trs.eulerAngles;
          stunedTime += Time.fixedDeltaTime;
          
          grabed = false;
          anim.Play("stun");
        }
      }
    }

    public void DamageDeal()
    {
    Player.gameObject.GetComponent<MasterController>().takedamage(mainCode.attackdmg);//vai receber a classe de controle geral, que tem em todos os jogadores
    } 



    void OnDrawGizmosSelected()
  {

    
    if(insideWater)
    { 

      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(this.transform.position, range*2/3);
      
      if(stPos != Vector3.zero)
      { 
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(stPos + this.transform.right*swimdistance, stPos - this.transform.right*swimdistance);
      }
      else
      { 
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position + this.transform.right*swimdistance, this.transform.position - this.transform.right*swimdistance);
      }
    }
    else
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(this.transform.position, range);
    }
      
    
  }
}




public class SwimmingTypes : MonoBehaviour
{ 
    [SerializeField] private bool insideWater = false;
    // Start is called before the first frame update
    public void SetinWater(bool value)
    {
        insideWater = value;
    }

    // Update is called once per frame
    public bool Getinwater()
    { 
        return insideWater;
    }
}




public class CloseButton : MonoBehaviour
{ 
  public GameObject Pannel;
    // Start is called before the first frame update

  public void ClosePannel()
  {
    Pannel.GetComponent<Animator>().Play("Pannel out");
  }
  public void fullClose()
  {
    Pannel.SetActive(false);
  }
}




public class MusicChanger : MonoBehaviour
{
    // Start is called before the first frame update
  void OnEnable()
  {
    GameEvents.StartListening("BossAreaEntered", ChangeMusic);
  }
  void OnDisable()
  {
    GameEvents.StopListening("BossAreaEntered", ChangeMusic);
  }


  void ChangeMusic()
  {
    AudioInterface a = this.gameObject.GetComponent<AudioInterface>();
    a.PlaySound("boss");
  }

}




public class AudioController : MonoBehaviour
{ 

  public static AudioController instance;

    public static Dictionary<string, float> soundVolumes = new Dictionary<string, float>(){{"sfx", 1f}, {"music", 1f}};

    void Start()
    {
        if(instance == null)
        {
            instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
    }

    public static float GetSoundVol(string tag)
    {
      float vol = 0f;

      soundVolumes.TryGetValue(tag, out vol);

        
      return 0 + vol;
    }

    public static void SetSoundVol(string tag, float vol)
    {
        
      if(!soundVolumes.ContainsKey(tag))
      {
        soundVolumes.Add(tag, vol);
      }
      else
      {
        soundVolumes[tag] = vol;
      }

      
    }
}




public interface AudioInterface 
{
  void PlaySound(string clipName);

  void PlaySound(AudioClip clip);

  void Pause();

  void UnPause();
}






public class BestiaryElements : MonoBehaviour
{ 
  public static BestiaryElements instance;
  public static List<CardIndex> Bestiary = new List<CardIndex>();
  

  void Awake()
  {
    if(instance == null)
        {
            instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
  }


  
  public static void AddCardEnemy(CardIndex note)
    { 
        bool alrNoted = false;
        int noteID = note.GetID();
        int Foundindex = -1;
        foreach(CardIndex nt in BestiaryElements.Bestiary)
        {   
            alrNoted = (nt.GetID() == noteID);
            if(alrNoted)
            {   
                Foundindex = BestiaryElements.Bestiary.IndexOf(nt);
                break;
            }
        }
        if(!alrNoted)
        {   
            BestiaryElements.Bestiary.Capacity ++;
            BestiaryElements.Bestiary.Add(note);
        }
        else
        { 
          note.TimesKilled ++;
            BestiaryElements.Bestiary[Foundindex] = note;
            Debug.Log(noteID);
        }
        Debug.Log(BestiaryElements.Bestiary.Count);
    }
}





public class VolumeSetter : MonoBehaviour
{ 

  public string thistag;

  public Slider slider;

  public void OnEnable()
  {
    slider = this.gameObject.GetComponent<Slider>();
    slider.value = AudioController.GetSoundVol(thistag);
  }
    public void SetVolume(float sliderValue)
    {
      AudioController.SetSoundVol(thistag, sliderValue);
      GameEvents.ScreamEvent("changevolume");
      
    }
}





public class BestiaryPage : MonoBehaviour
{
    [SerializeField] private Text tname;//Nome científico
    [SerializeField] private Text C_info;//Informações Científicas
    [SerializeField] private Text levelmeet;//Level em que aparece
    [SerializeField] private Text G_info;//Comportamentos In-game
    [SerializeField] private Text TimesKilled;//vezes que foi abatido pelo jogador
    [SerializeField] private Image C_image;//Pixelart da imagem em microscópio do bixo
    [SerializeField] private Image G_image;//Sprite In-game
    [SerializeField] private int Pagenum = 0; //número da página
    [SerializeField] public List<CardIndex> Bestiary = new List<CardIndex>();

    void Awake()
    {
        Bestiary = BestiaryElements.Bestiary;
    }

    void setPage()
    {
        tname.text = BestiaryElements.Bestiary[Pagenum].RealName;
        C_info.text = BestiaryElements.Bestiary[Pagenum].CInfo;
        levelmeet.text = BestiaryElements.Bestiary[Pagenum].LevelName;
        G_info.text = BestiaryElements.Bestiary[Pagenum].IngameBehavior;
        C_image.sprite = BestiaryElements.Bestiary[Pagenum].RealImage;
        G_image.sprite = BestiaryElements.Bestiary[Pagenum].Ingame;
    }

    public void setPagenum(int n)
    {      
        
        if(BestiaryElements.Bestiary.Count > n && n >= 0)
        {
            Pagenum = n;
            setPage();
        }
        
    }
    public void nextPage(int sentido)
    {    
        if(Pagenum + sentido <= BestiaryElements.Bestiary.Count -1  && (Pagenum + sentido >= 0))
        {
            Pagenum += sentido;
            setPage();
        }

    }
}






[RequireComponent(typeof(PlayableDirector))]
public class CutsceneChanger : MonoBehaviour
{ 
  public List<TimelineAsset> cutscenes;
    // Start is called before the first frame update
    void PlayCutscene(int index)
    { 
      TimelineAsset sel_Cutscene;
      if(cutscenes.Count > index)
      {
        sel_Cutscene = cutscenes[index];
        
      }
      else
      {
        sel_Cutscene = cutscenes[cutscenes.Count -1];
      }
      this.gameObject.GetComponent<PlayableDirector>().Play(sel_Cutscene);
    }


}




public class CardIndex
{
  public Sprite RealImage;
  public Sprite Ingame;
  public string LevelName;
  public string IngameBehavior;
  public string RealName;
  public string CInfo;
  public int TimesKilled;
  public int ID;


  public CardIndex(int id, Sprite ri, Sprite ii, string l, string ib, string n, string rb)
  {
    ID = id;

    RealImage = ri;
    Ingame = ii;
    LevelName = l;
    IngameBehavior = ib;
    RealName = n;
    CInfo = rb;
  }

  public CardIndex GetEnemyCardByID(int id)
  {
    if(id == ID)
    {
      return this;
    }
    return null;
  }

  public int GetID()
  {
    return ID;
  }

  public override string ToString()
  {
    return "ID: " + this.ID + ", Name: " + this.RealName + ", Image: " + this.RealImage;
  }
}






[Serializable]
public class DialogContent
{ 
  [SerializeField] private string ownerName;
  [SerializeField] private Sprite ownerPhoto;
  [TextArea]
  [SerializeField] private string dialogText;
  [SerializeField] private UnityEvent endAction;
  public string GetOwnerName()
  {
    return ownerName;
  }
  public Sprite GetOwnerPhoto()
  {
    return ownerPhoto;
  }
  public string GetMainText()
  {
    if(dialogText != null)
    {
      return dialogText;
    }
    return "";
  }

  public void FinalAction()
  {
    if(endAction != null)
    { 

      endAction.Invoke();
    }
  }

  public string ToString()
  {
    return "Falante: " + ownerName + "; Texto: " + dialogText + ";";  
  }

}




public class Effects : MonoBehaviour {

  // Use this for initialization
  void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
    
  }
  public void Vanish(){
    Destroy(gameObject);
  }
}




public class EnemyOptimizer : MonoBehaviour
{
  [SerializeField] private Transform p;

  [SerializeField] private List<GameObject> e = new List<GameObject>();

  [SerializeField] private float screensize;
  
  void Start()
  { 
    p = GameObject.FindWithTag("Player").GetComponent<Transform>();
    foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("hitable"))
    { 
      if(enemy.activeSelf)
      {
        e.Add(enemy);
      }   
      
    }
    screensize = Mathf.Pow(Screen.width, 1);
  }
  void FixedUpdate()
  { 
    foreach(GameObject enemy in e)
    { 
      if(enemy == null)
      {
        e.Remove(enemy);
        break;
      }
      else
      {
        float a = ((Vector2)p.position - (Vector2) enemy.transform.position).sqrMagnitude;
        if(enemy.activeSelf)
        {
          if(screensize < a)
          {
            enemy.SetActive(false);
          }
        }
        else
        {
          if(screensize > a)
          {
            enemy.SetActive(true);
          }
        }
      }

    }
  }
}




public class EventTriggers : MonoBehaviour {

  private GameObject Player;
  [SerializeField] private string eventname;
  [SerializeField] private GameObject Activable_obj;
  [SerializeField] private Vector3 CameraPosition;


  void FixedUpdate()
  { 
    Player = GameObject.FindWithTag("Player");
    if(Player.transform.position.x > transform.position.x)
    {
      GameEvents.ScreamEvent(eventname);
      if(Activable_obj)
      {
        Activable_obj.SetActive(!Activable_obj.activeSelf);
      }
      if(CameraPosition != Vector3.zero)
      {
        CameraBehavior cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>();
        cam.posx = CameraPosition.x;
        cam.posy = CameraPosition.y;
        cam.camscale = CameraPosition.z;
      }
      
      Destroy(gameObject);

    } 
  }

}





public class EventPlay : MonoBehaviour
{
  [SerializeField] private string eventTolisten;

  [SerializeField] private UnityEvent eventToPlay;


  void OnEnable()
  {
    GameEvents.StartListening(eventTolisten, PlayEvent);
  }
  void OnDisable()
  {
    GameEvents.StopListening(eventTolisten, PlayEvent);
  }

  void PlayEvent()
  { 
    if(eventToPlay != null)
    {
      eventToPlay.Invoke();
    }
    
  }
}




public class TutorialTrigger : MonoBehaviour
{
  [SerializeField] private TutorialBehavior banner;

  [SerializeField] private GameObject bannerobj;

  [SerializeField] private Sprite backImage;

  [SerializeField] private Sprite frontImage;

  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player"))
    {
      ActivateBanner();
    }

  }
  private void OnTriggerExit2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player") && !other.isTrigger)
    {
      banner.CloseTutorial();
    }

  }

  public void ActivateBanner()
  {
      if(banner)
      {
        banner.gameObject.SetActive(true);
      }
      else
      {
        banner = Instantiate(bannerobj, this.gameObject.transform.position + Vector3.up*5, bannerobj.transform.rotation).GetComponent<TutorialBehavior>();
      }
      if(frontImage)
      {
        banner.AddTutorialImages(backImage, frontImage);
      }
      else
      {
        banner.AddTutorialImages(backImage);
      }
      
  }
}





public class GameEvents : MonoBehaviour {

  private  Dictionary<string, UnityEvent> eventDict;
  private static GameEvents gameEvents;

  public static GameEvents curnt_instance
  {
    get
    {
      if(!gameEvents)
      {
        gameEvents = FindObjectOfType(typeof(GameEvents)) as GameEvents;
        if(!gameEvents)
        {
          Debug.LogError("You will need to put this script in a active object to it work");
        }
        else
        {
          gameEvents.Initiate();
        }
      }
      return gameEvents;
    }
  }



  void Initiate()
  {
    if(eventDict == null)
    {
      eventDict = new Dictionary<string, UnityEvent>();
    }
  }

  public static void StartListening(string eventName, UnityAction listen)
  {
    UnityEvent thisEvent = null;
    if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
    {
      thisEvent.AddListener(listen);
    }
    else
    {
      thisEvent = new UnityEvent();
      thisEvent.AddListener(listen);
      curnt_instance.eventDict.Add(eventName, thisEvent);
    }
  }
  public static void StopListening(string eventName, UnityAction listen)
  {
    if(gameEvents == null)
    {
      return;
    }
    UnityEvent thisEvent = null;
    if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
    {
      thisEvent.RemoveListener(listen);
    }

  }
  public static void ScreamEvent(string eventName)
  {
    UnityEvent thisEvent = null;
    if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
    {
      thisEvent.Invoke();
    }
  }

  /*{
  public static GameEvents current;


  void Awake () {
    current = this;
  } 
  
  public event Action OnBossAreaEnter;

  public void BossAreaEnter()
  {
    if(OnBossAreaEnter != null)
    {
      OnBossAreaEnter();
    }
  }
  public event Action OnBossDie;

  public void BossDie()
  {
    if(OnBossDie != null)
    {
      OnBossDie();
    } 
  }
  public event Action OnBossDead;

  public void BossDead()
  {
    if(OnBossDead != null)
    {
      OnBossDead();
    } 
  }

  //}
  //tudo dentro dessa chave acima faz parte de um sistema antigo que será substituido em breve. Ele controla alguns comandos relacionados à ativação do Boss da fase*/

  
}




public class HealPoint : MonoBehaviour {

  public int lifeheal;

  void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
    
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.tag.Equals("Player"))
    {
      var player = other.gameObject.GetComponent<MasterController>();
      this.gameObject.GetComponent<AudioInterface>().PlaySound("heal");
      player.gainlife(lifeheal);

      this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
      this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
  }
}




public class Anotation
{
  public Texture pic;
  public int ID;
  public string text;

  public Anotation(int i, Texture p, string t)
  {
    pic = p;
    ID = i;
    text = t;
  }

  public Anotation(CardIndex cd)
  {
    pic = cd.RealImage.texture;
    ID = cd.ID;
    text = cd.CInfo;
  }

  public Anotation GetAnotationById(int id)
  {
    if(id == ID)
    {
      return this;
    }
    return null;
  }

  public int GetID()
  {
    return ID;
  }

  public override string ToString()
  {
    return "ID: " + this.ID + ", Texture Name: " + this.pic + ", Description: " + this.text;
  }
}




public class AnotationHotBar : MonoBehaviour
{
  [SerializeField] private Transform Hotbar;
  [SerializeField] private GameObject AnHotBar;

    public void SpawnNotes()
    { 
      foreach(Anotation note in AnotationManager.Notes)
      {
        if(note.pic)
            {
                GameObject pic = Instantiate(AnHotBar, Hotbar);
                pic.GetComponent<PhisicNote>().isText = false;
            pic.GetComponent<PhisicNote>().SetInfo(note);
        }
            
            if(note.text != "")
            {
                GameObject tex = Instantiate(AnHotBar, Hotbar);
                tex.GetComponent<PhisicNote>().isText = true;
                tex.GetComponent<PhisicNote>().SetInfo(note);
            } 
      }


        List<CardIndex> best = new List<CardIndex>();

        foreach(CardIndex cd in BestiaryElements.Bestiary)
        {
            Anotation note = new Anotation(cd);

            bool arlspawned = false;

            foreach(CardIndex c in best)
            {   
                if(c.CInfo == cd.CInfo)
                {
                    arlspawned = true;
                    break;
                }

            }
            
            if(!arlspawned)
            {      
                best.Add(cd);
                if(note.pic)
                {        
                    GameObject pic = Instantiate(AnHotBar, Hotbar);
                    pic.GetComponent<PhisicNote>().isText = false;
                    pic.GetComponent<PhisicNote>().SetInfo(note);
                }

                if(note.text != "")
                {
                    GameObject tex = Instantiate(AnHotBar, Hotbar);
                    tex.GetComponent<PhisicNote>().isText = true;
                    tex.GetComponent<PhisicNote>().SetInfo(note);
                }
                    
            }  
        }
    }

    public void DestroyNotes()
    {
        for (int i = 0; i < Hotbar.childCount; i++)
        {
            Destroy(Hotbar.GetChild(i).gameObject);
        }
    }

    public void RemoveNote(Anotation a)
    {

    }
    public void AddNote(Anotation a)
    {
      
    }
}




public class AnotationManager : MonoBehaviour
{   
    public static AnotationManager instance;
    [SerializeField] public static List<Anotation> Notes = new List<Anotation>();


    void Awake()
    {   
        if(instance == null)
        {
            instance = this;
            Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }
        

    }
    

    public static void AddNote(Anotation note)
    {
        bool alrNoted = false;
        int noteID = note.GetID();
        int Foundindex = -1;
        foreach(Anotation nt in AnotationManager.Notes)
        {   
            alrNoted = (nt.GetID() == noteID);
            if(alrNoted)
            {   
                Foundindex = Notes.IndexOf(nt);
                break;
            }
        }
        if(!alrNoted)
        {   
            AnotationManager.Notes.Capacity ++;
            Notes.Add(note);
        }
        else
        {
            AnotationManager.Notes[Foundindex] = note;
        }
    }

}




public class NoteLigator : MonoBehaviour
{
    public GameObject Ob1;
    public GameObject Ob2;

    private Camera mainc;

    [SerializeField] private LineRenderer lRend;

    void Start()
    { 
      mainc = Camera.main;
        lRend = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Ob1)
        {
          lRend.SetPosition(0, Ob1.transform.position);
        }

        if(!Ob2)
        {
          lRend.SetPosition(1, mainc.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
          lRend.SetPosition(1, Ob2.transform.position);
        }
    }

    public string GetLigationIDS()
    {
      string LigId = "-1,-1";
      if(Ob1 && Ob2)
      {
        LigId = Mathf.Min(Ob1.GetComponent<PhisicNote>().ID, Ob2.GetComponent<PhisicNote>().ID) + "," + Mathf.Max(Ob1.GetComponent<PhisicNote>().ID, Ob2.GetComponent<PhisicNote>().ID);
      }
      return LigId;
    }

}





public class PhisicNote : MonoBehaviour
{ 
  [SerializeField] private Texture BlankNote;

  public bool isText;

    [SerializeField] public Anotation InfoBase;

    [SerializeField] public GameObject ImageObj;

    [SerializeField] private RawImage spr;

    [SerializeField] private SpriteRenderer spR;

    [SerializeField] private TextMesh label;

    [SerializeField] private TMP_Text label2;

    public int ID = -1;


    void Awake()
    {
      spr = ImageObj.GetComponent<RawImage>();
        if(!spr)
        {
           spR = ImageObj.GetComponent<SpriteRenderer>();
        }   
    }



    public void SetInfo(Anotation i)
    { 
      InfoBase = i;
      if(spR)
        {
            spR.enabled = false;
        }
        else
        {
            spr.enabled = false;
        }
      if(isText)
      { 
      if(label != null)
        {
          

          label.text = i.text;
        }
        else if(label2 == null)
        { 
                if(spr)
                {
                    spr.enabled = true;
                }
          
          spr.texture = BlankNote;
        }
            else
            {   
                label2.text = i.text;
            }

      }
      else
      { 
        if(label)
        {
          label.gameObject.SetActive(false);
        }
            if(label2)
            {
                label2.gameObject.SetActive(false);
            }

            if(spr)
            {
                spr.enabled = true;
                spr.texture = i.pic;
            }
            else
            {

                spR.enabled = true;
                Debug.Log("Width: " + i.pic.width + "; Height: " + i.pic.height);
                spR.sprite = Sprite.Create((Texture2D)i.pic, new Rect(0.0f, 0.0f, i.pic.width, i.pic.height), Vector2.one/2);

                float spc = (spR.sprite.pixelsPerUnit/(i.pic.width/4.8f)) * 0.2f ;

                spR.gameObject.GetComponent<RectTransform>().localScale = Vector2.one * spc;
            }
        
      }
      ID = i.ID;
      
    }

    



}




public class StoredNote : MonoBehaviour
{
  [SerializeField] private PhisicNote thisNote;
  [SerializeField] private GameObject fNote;
  [SerializeField] private bool hovered;
  [SerializeField] private GameObject hovernote;

  public void CreateNote(bool h)
  { 
    hovered = h;
    GameObject note = Instantiate(fNote, gameObject.transform.position -Vector3.forward*gameObject.transform.position.z, gameObject.transform.rotation , this.gameObject.transform.parent.parent.parent);
    note.GetComponent<PhisicNote>().isText = thisNote.isText;
    note.GetComponent<PhisicNote>().SetInfo(thisNote.InfoBase);

    if(hovered)
    {
      hovernote = note;
    }
    else
    { 
      Destroy(hovernote);

      hovernote = note;
    }
  }
  public void DeleteNote()
  {
    if(hovered && !hovernote.GetComponent<FlyingNote>().isDrag)
    {
      Destroy(hovernote);
    }
      hovered = false;
    
      
  }
}




public class TableManager : MonoBehaviour
{

    public List<string> TrueAwnsers = new List<string>();

    public int pontuation;

    public void GetAllCombinations()
    { 
      List<string> TA = new List<string>(TrueAwnsers);
    

        Object[] combinations = Object.FindObjectsOfType<NoteLigator>();
        Debug.Log(combinations.Length);

        foreach(NoteLigator l in combinations)
        { 

          string i = l.GetLigationIDS();
          Debug.Log(i);
          foreach(string t_awnser in TrueAwnsers)
          {
            if(i == t_awnser && TA.Contains(i))
            {
              pontuation += 1;
              TA.Remove(t_awnser);
            }
          }
            Destroy(l);
        }
        Debug.Log(pontuation);
        GameEvents.ScreamEvent("ClosedTable");
        PontuationCounter.AddScore(pontuation*1500);
    }
}



public class InputManager : MonoBehaviour
{
  public static InputManager instance;

  public int Vaxis;

  public int Haxis;

  public KeyBindings keybindings;
  void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(this.gameObject);
    }
    else if(instance != this)
    {
      Destroy(this.gameObject); 
    }
  }

  // Update is called once per frame
  public bool GetButton(string key)
  {
    if(Input.GetKey(keybindings.CheckKey(key)))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool GetButtonDown(string key)
  {
    if(Input.GetKeyDown(keybindings.CheckKey(key)))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool GetButtonUp(string key)
  {
    if(Input.GetKeyUp(keybindings.CheckKey(key)))
    {
      return true;
    }
    else
    {
      return false;
    }
  }
  public float GetAxisRaw(string axis)
  { 
    
    
    if(axis == "Horizontal")
    { 
      float x = 0;
      if(Input.GetKey(keybindings.CheckKey("right")))
      {
        x++;
        return x;
      }
      if (Input.GetKey(keybindings.CheckKey("left")))
      {
        x--;
        return x;
      }
      
    }
    if(axis == "Vertical")
    { 
      float y = 0;
      if(Input.GetKey(keybindings.CheckKey("up")))
      {
        y++;
        return y;
      }
      if (Input.GetKey(keybindings.CheckKey("down")))
      {
        y--;
        return y;
      }
      
    }
    return 0;

  }
}






public class InputMenu : MonoBehaviour
{

  public bool selected = false;

  public string innerName;

  public KeyCode thiscode;

  public Text keytext;

  public Event keyEvent;

  public AudioInterface a;

  void OnEnable()
  {
    GameEvents.StartListening("selectKeyButton", Unselect);

    thiscode = InputManager.instance.keybindings.CheckKey(innerName);
    keytext.text = thiscode.ToString();
    Unselect();
    a = this.gameObject.GetComponent<AudioInterface>();

  }
  void OnDisable()
  {
    GameEvents.StopListening("selectKeyButton", Unselect);
  }
  

  public void Unselect()
  { 
    selected = false;
  }
  public void Select()
  { 
    if(a != null)
    {
      a.PlaySound("click");
      GameEvents.ScreamEvent("selectKeyButton");
      selected = true;
    }

  }

  void OnGUI()
  {
    keyEvent = Event.current;

    if(keyEvent.isKey && selected)
    { 
      Debug.Log("selected");
      GameEvents.ScreamEvent("selectKeyButton");
      thiscode = keyEvent.keyCode;
      SetKeyBinding();
    }
    else if(keyEvent.type == EventType.MouseDown && selected)
    {
      GameEvents.ScreamEvent("selectKeyButton");
    }
  }
  public void SetKeyBinding()
  {
    InputManager.instance.keybindings.SetKey(thiscode, innerName);
    keytext.text = thiscode.ToString();
  }
}




public class PitfallReturner : MonoBehaviour {


  private Transform trs;
  // Use this for initialization
  void Start()
  {
    trs = GetComponent<Transform> ();
  }
  private void OnTriggerEnter2D(Collider2D other)
  {

    if (other.gameObject.tag.Equals("Player"))
    {
      var hited = other.gameObject.GetComponent<MasterController>();
      hited.GroundSave(new Vector2(trs.position.x, trs.position.y));
    }

  }
}




[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBindings : ScriptableObject 
{
  public KeyCode right, left, up, down, jump, attack, spec, pause;

  public KeyCode CheckKey(string key)
  {
    switch(key)
    {
      case "right":
        return right;
      case "left":
        return left;
      case "up":
        return up;
      case "down":
        return down;
      case "Jump":
        return jump;
      case "Attack":
        return attack;
      case "Spec":
        return spec;
      case "pause":
        return pause;

      default:
        return KeyCode.None;
        break;
    }
  }

  public void SetKey(KeyCode code, string key)
  {
    switch(key)
    {
      case "right":
        right = code;
        break;
      case "left":
        left = code;
        break;
      case "up":
        up = code;
        break;
      case "down":
        down = code;
        break;
      case "Jump":
        jump = code;
        break;
      case "Attack":
        attack = code;
        break;
      case "Spec":
        spec = code;
        break;
      case "pause":
        pause = code;
        break;
      default:
        break;
    }
    Debug.Log("KeySet");
  }


}





public class SceneLoader : MonoBehaviour
{

    public void LoadScene(string s)
    {
      SceneChanger.Load(s);
    }
}




public class CameraBehavior : MonoBehaviour {


  public bool lvlselection = false;
  
  private Vector2 velocity;
  private Vector2 refer;

  public static Transform target;
  public Camera cam;

  public float camsensex;
  public float camsensey;
  public float edgeleft;
  public float edgeright;
  public float edgeup;
  public float edgedown;
  public float camscale;
  

  //Efeitos de status
    // tremor de camera
  public bool shaking;
  public float shkintenX;
  public float shkintenY;
  private int shkX = 1;
  private int shkY = 1;
    // camera livre(false) ou presa a um objeto(true)
  public bool targeted;

  public float posx;
  public float posy;
  public float desx;
  public float desy;
  // Update is called once per frame



  void OnEnable()
  {
    GameEvents.StartListening("BossAreaEntered", ToggleToBossCamera);
    GameEvents.StartListening("BossAreaExited", ToggleToPlayerCamera);
    refer = new Vector2(1.0f , 1.0f);
    

  }
  void OnDisable()
  {
    GameEvents.StopListening("BossAreaEntered", ToggleToBossCamera);
    GameEvents.StopListening("BossAreaExited", ToggleToPlayerCamera);
  }
  void FixedUpdate () {

    if(!target)
    { 
      target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, camscale, ref refer.x, (camsensex+camsensey)/2);



    if(targeted)
    {
      desx = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, camsensex);
      desy = Mathf.SmoothDamp(transform.position.y, target.position.y+1, ref velocity.y, camsensey);
      if(desx < edgeleft)
      {
        desx = edgeleft;
      }
      if (desx > edgeright)
      {
        desx = edgeright;
      }
      if (desy > edgeup)
      {
        desy = edgeup;
      }
      if (desy < edgedown)
      {
        desy = edgedown;
      }
    }

    else
    {
      desx = Mathf.SmoothDamp(transform.position.x, posx, ref velocity.x, camsensex*15);
      desy = Mathf.SmoothDamp(transform.position.y, posy, ref velocity.y, camsensey*15);
    }
    



    if(shaking)
    {
      shkX = shkX * -1;
      shkY = shkY * -1;
      desx += shkX * shkintenX;
      desy += shkY * shkintenY;
    }

    transform.position = new Vector3 (desx, desy, transform.position.z);
    

  }

  private void ToggleToBossCamera()
  {
    targeted = false;
  }
  private void ToggleToPlayerCamera()
  {
    targeted = true;
  }
  public void ToggleShake(bool state, float x, float y)
  {
    shaking = state;
    shkintenY = y;
    shkintenX = x;
  }

}






public class AnotationPage : MonoBehaviour
{
  [SerializeField] private Text Info;//Informações Científicas
  [SerializeField] private Image Photo;//Sprite In-game
  [SerializeField] private int Pagenum = 0; //número da página
  [SerializeField] public List<Anotation> Notes;

  void Awake()
  {
    Notes = AnotationManager.Notes;
  }

    void setPage()
    { 
        RectTransform r = Photo.gameObject.GetComponent<RectTransform>();
        Rect s = Rect.zero;
        Texture2D t = (Texture2D) Notes[Pagenum].pic;
        s.size = Vector2.one*t.width;
        Debug.Log(s.ToString());
      Info.text = Notes[Pagenum].text;
      Photo.sprite = Sprite.Create(t, s, Vector2.one/2, 64, 5);

    }

    public void setPagenum(int n)
    { 
      if(Notes != AnotationManager.Notes)
      {
        Notes = AnotationManager.Notes;
      }
      if(AnotationManager.Notes.Count >= n && n >= 0)
      {
        Pagenum = n;
        setPage();
      }
      
    }
    public void nextPage(int sentido)
    {  
      if(Pagenum + sentido <= Notes.Count -1  && (Pagenum + sentido >= 0))
      {
        Pagenum += sentido;
        setPage();
      }

    }
}




public class KillCounter : MonoBehaviour
{
  public static int EnemiesKilled = 0;

  [SerializeField] private Text Countertxt;
    void OnEnable()
  {
    GameEvents.StartListening("EnemyKilled", UpdateScore);
    EnemiesKilled = 0;
  }
  void OnDisable()
  {
    GameEvents.StopListening("EnemyKilled", UpdateScore);
  }
  void UpdateScore()
  {
    EnemiesKilled ++;
  }
  void LateUpdate()
  {
    Countertxt.text = EnemiesKilled.ToString();
  }
}





public class LifeFramePhoto : MonoBehaviour
{
  public List<Sprite> photos;

  [SerializeField] private Image frame;

  void Start()
  {
    ChangeTexture();
  }

  private void ChangeTexture()
  {
    GameObject Player = GameObject.FindWithTag("Player");

    switch(Player.name)
    {
      case "Neutrophil":
        frame.sprite = photos[0];
        break;
      case "Eosinophil":
        frame.sprite = photos[1];
        break;
      case "Linph NK":
        frame.sprite = photos[2];
        break;
      case "Macrophage":
        frame.sprite = photos[3];
        break;

    }
  }
}




public class QuestionMaker : MonoBehaviour
{ 
  [SerializeField] private GameObject Answer; 
    // Start is called before the first frame update
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
      if(other.gameObject.tag.Equals("Player"))
      {
        Answer.SetActive(true);
        GameEvents.ScreamEvent("QuestFound");
      }
    }
}




public class SearchIndicator : MonoBehaviour
{ 
  [SerializeField] private GameObject Indicator;
  [SerializeField] private GameObject player;
  [SerializeField] private float min_distance = 0;
  [SerializeField] private float Player_distance;

    private int bpspeed = 1;
    private float actscale;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
      Indicator = GameObject.FindWithTag("Indicator");
      player = GameObject.FindWithTag("Player");
      
    }
    void Awake()
    {
      Indicator.GetComponent<SpriteRenderer>().enabled = true;
    }
    void Update()
    {
        Player_distance = Vector3.Distance(player.transform.position, transform.position);
        
        if(Player_distance < min_distance)
        {
          bpspeed = 5;
        }
        else if(Player_distance < min_distance * 1.75f)
        {
          bpspeed = 3;
        }
        else if (Player_distance < min_distance * 3.5f)
        {
          bpspeed = 1;
        }
        else
        {
          bpspeed = 0;
        }

        actscale = (Mathf.PingPong(Time.time*bpspeed, 0.5f)+1);
        Indicator.transform.localScale = new Vector3(30*actscale, 30*actscale, 1);

        if(actscale >= 1.4f) Indicator.gameObject.GetComponent<AudioSource>().Play();

    }
    void OnTriggerEnter2D(Collider2D other)
    {
      if(other.gameObject.tag.Equals("Player"))
      {
        Indicator.GetComponent<SpriteRenderer>().enabled = false;
      }
    }

}




public class ScoreBoard : MonoBehaviour
{ 
  [SerializeField] private Text ScoreShower;

    void OnEnable()
  {
    GameEvents.StartListening("GamePaused", UpdateScore);
  }
  void OnDisable()
  {
    GameEvents.StopListening("GamePaused", UpdateScore);
  }
    void UpdateScore()
    { 
      Debug.Log(PontuationCounter.GetScoreString());
      if(!ScoreShower)
      {
        ScoreShower = this.gameObject.GetComponent<Text>();
      }
        
        ScoreShower.text = PontuationCounter.GetScoreString();
    }
}

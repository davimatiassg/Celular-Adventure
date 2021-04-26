using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

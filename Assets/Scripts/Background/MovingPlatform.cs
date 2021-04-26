using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

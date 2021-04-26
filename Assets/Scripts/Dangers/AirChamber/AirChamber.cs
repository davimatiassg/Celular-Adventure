using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

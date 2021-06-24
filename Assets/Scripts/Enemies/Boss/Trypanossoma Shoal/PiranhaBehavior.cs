using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

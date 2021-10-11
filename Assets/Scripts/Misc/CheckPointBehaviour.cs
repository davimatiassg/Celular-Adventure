using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

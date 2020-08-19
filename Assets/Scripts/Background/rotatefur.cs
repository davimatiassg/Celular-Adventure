using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

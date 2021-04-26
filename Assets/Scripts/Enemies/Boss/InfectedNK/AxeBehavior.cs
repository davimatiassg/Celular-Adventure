using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

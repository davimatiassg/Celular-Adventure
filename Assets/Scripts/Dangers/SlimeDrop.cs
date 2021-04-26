using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

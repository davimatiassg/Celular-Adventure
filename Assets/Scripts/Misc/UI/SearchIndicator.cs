using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchIndicator : MonoBehaviour
{	
	[SerializeField] private GameObject Indicator;
	[SerializeField] private GameObject player;
	[SerializeField] private float min_distance;
	[SerializeField] private float Player_distance;
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
        	Indicator.transform.localScale = new Vector3((Mathf.PingPong(Time.time*5, 0.5f)+1)*30, (Mathf.PingPong(Time.time*5, 0.5f)+1)*30, 1);
        }
        else if(Player_distance < min_distance * 1.75f)
        {
        	Indicator.transform.localScale = new Vector3((Mathf.PingPong(Time.time*3, 0.5f)+1)*30, (Mathf.PingPong(Time.time*3, 0.5f)+1)*30, 1);
        }
        else if (Player_distance < min_distance * 3.5f)
        {
        	Indicator.transform.localScale = new Vector3((Mathf.PingPong(Time.time, 0.5f)+1)*30, (Mathf.PingPong(Time.time, 0.5f)+1)*30, 1);
        }
        else
        {
        	Indicator.transform.localScale = new Vector3(30, 30, 1);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
   	{
   		if(other.gameObject.tag.Equals("Player"))
   		{
   			Indicator.GetComponent<SpriteRenderer>().enabled = false;
   		}
   	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundTrigger : MonoBehaviour
{	
	private Transform obj;
	private Transform Player;
	public int startbg = 0;
	private bool arin = false;
    public Vector2 BackgLeft_Right;
    public Color[] BgCollors;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        obj = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {	
        

        if(1f > Mathf.Abs(Player.position.x - obj.position.x) && arin == false)
        {   
            GameObject[] bgn = GameObject.FindGameObjectsWithTag("Parallax");
        	setBackground(bgn);
        	arin = true;
        }
        else if(1f < Mathf.Abs(Player.position.x - obj.position.x))
        {
        	arin = false;
        }
        
    }

    void setBackground(GameObject[] bglist)
    {	 
        Debug.Log("Bg settado");
        if(Player.position.x < obj.position.x)
        {
            startbg = Mathf.RoundToInt(BackgLeft_Right.y);
            cam.backgroundColor =  BgCollors[1];
        }
        else if(Player.position.x > obj.position.x)
        {
            startbg = Mathf.RoundToInt(BackgLeft_Right.x);
            cam.backgroundColor =  BgCollors[0];
        }
    	foreach(GameObject b in bglist)
    	{
    		b.GetComponent<NewParallax>().ChangeBackground(startbg);
    	}
    }
}

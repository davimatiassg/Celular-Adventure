using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{	
	[SerializeField] private Vector2 limit_up_right;
	[SerializeField] private Vector2 limit_down_left;
	[SerializeField] private Vector2 parallax_factor;
    
	[SerializeField] private Transform camera_trs;
	private Vector3 last_cam_position;

    [SerializeField] private SpriteRenderer spr;

    public bool parallaxing = false;

    // Start is called before the first frame update
    void Start()
    {   
        camera_trs = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        last_cam_position = camera_trs.position;
        spr = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = camera_trs.position - last_cam_position;

        if(camera_trs.position.x >= limit_down_left.x && camera_trs.position.x <= limit_up_right.x)
        {   
            parallaxing = true;
        	transform.position += new Vector3(delta.x*parallax_factor.x, 0);
        }
        else
        {
            parallaxing = false;
        }
        if(transform.position.x < limit_down_left.x)
        {
            transform.position = new Vector3(limit_down_left.x, transform.position.y, transform.position.z);
        }
        if(transform.position.x > limit_up_right.x)
        {
            transform.position = new Vector3(limit_up_right.x, transform.position.y, transform.position.z);
        }

        if(camera_trs.position.y >= limit_down_left.y && camera_trs.position.y <= limit_up_right.y)
        {	
        	transform.position += new Vector3(0, delta.y * parallax_factor.y);	
        }

        last_cam_position = camera_trs.position;
    }
}

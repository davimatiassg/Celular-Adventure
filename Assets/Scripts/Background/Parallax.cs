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

    // Start is called before the first frame update
    void Start()
    {
        last_cam_position = camera_trs.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 delta = camera_trs.position - last_cam_position;

        if(camera_trs.position.x > limit_down_left.x && camera_trs.position.x < limit_up_right.x)
        {
        	transform.position += new Vector3(delta.x*parallax_factor.x, 0);
        }
        if(camera_trs.position.y > limit_down_left.y && camera_trs.position.y < limit_up_right.y)
        {	

        	transform.position += new Vector3(0, delta.y * parallax_factor.y);	
        }
        
        last_cam_position = camera_trs.position;
    }
}

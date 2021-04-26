using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBehavior : MonoBehaviour
{
    [SerializeField] private LineRenderer ln;

    private Transform trs;

    [SerializeField] private bool isMoving;

    [SerializeField] private int c_point;

    [SerializeField] private List<float> H_points = new List<float>();

    private MasterController player;

    void Start()
    {
        trs = this.gameObject.GetComponent<Transform>();
        ln = this.gameObject.GetComponent<LineRenderer>();
        ln.SetPosition(0, Vector3.right*trs.position.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	if(isMoving)
    	{
    		
			trs.position = Vector2.MoveTowards(trs.position, H_points[c_point]*Vector2.up + trs.position.x*Vector2.right, 30*Time.fixedDeltaTime);
    		player.trs.position -= Vector3.up*Mathf.Sign(trs.position.y - H_points[c_point])*30*Time.fixedDeltaTime; 
    		if(Mathf.Abs(H_points[c_point] - trs.position.y) <= 0.05f)
    		{
				isMoving = false;
    			player.playable = true;
    		}
    		ln.SetPosition(1, trs.position + Vector3.up*2);
     	}  
    }

    public void OnTriggerStay2D(Collider2D other)
    {
    	if(other.gameObject.tag == "Player")
    	{
    		if(!isMoving)
    		{	

    			int i = -(int)Input.GetAxisRaw("Vertical");
    			Debug.Log(i);
    			if(c_point + i < H_points.Count && c_point + i >= 0 && i != 0)
    			{
					c_point += i;
					player = other.gameObject.GetComponent<MasterController>();
					player.playable = false;
					player.rigb.velocity = Vector2.zero;
					isMoving = true;

    			}
    			
    		}
    	}
    }
}

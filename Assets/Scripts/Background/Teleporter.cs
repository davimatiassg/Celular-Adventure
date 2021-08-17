using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
   	[SerializeField] private bool interactable;

   	[SerializeField] private Vector3 exit;

   	[SerializeField] private Vector3 dest = Vector3.zero;

   	[SerializeField] private GameObject exitObj;

   	[SerializeField] private Collider2D Player;

   	[SerializeField] private Collider2D ThisCol; 

   	[SerializeField] private int framecount;

   	public InputManager InPut;

   	void OnEnable()
   	{
   		GameEvents.StartListening("CheckTP", CheckTP);
   	}

    void OnDisable()
   	{
   		GameEvents.StopListening("CheckTP", CheckTP);
   	}

   	void Start()
   	{
   		if(exitObj)
   		{
   			ThisCol = this.gameObject.GetComponent<Collider2D>();
   			Player = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
   			exit = exitObj.transform.position + exit;
   			InPut = InputManager.instance;
		}
   	}
	

	void Update()
	{
		if(dest != Vector3.zero)
		{   
			if(framecount == 0)
			{

				Player.transform.position = dest;
				dest = Vector3.zero;
			}
			else if(framecount > 0)
			{
				framecount --;
			}
		}

		
		if(InPut.GetButtonDown("Attack"))
		{	
			GameEvents.ScreamEvent("CheckTP");

		}
	}

	public void CheckTP()
	{
		if(ThisCol.Distance(Player).isOverlapped)
		{
			framecount = 5;
			dest = exit + Player.transform.position.z*Vector3.forward;
			
		}
	}

}

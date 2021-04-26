using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
   	[SerializeField] private bool interactable;

   	[SerializeField] private Vector3 exit;

   	[SerializeField] private GameObject exitObj;

   	void Start()
   	{
   		if(exitObj)
   		{
   			exit = exitObj.transform.position;
		}
   	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if((!interactable || Input.GetButtonDown("Fire1")) && other.gameObject.tag == "Player")
		{	
			GameObject.FindWithTag("Player").transform.position = exit + GameObject.FindWithTag("Player").transform.position.z*Vector3.forward;
			GameObject.FindWithTag("MainCamera").transform.position = exit + GameObject.FindWithTag("MainCamera").transform.position.z*Vector3.forward;
		}
	}
}

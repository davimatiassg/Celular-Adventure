using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{	
	public string enteredobject = "";

    	private void OnTriggerEnter2D(Collider2D other)
   		{
    		enteredobject = other.gameObject.tag;
   		}
   		private void OnTriggerExit2D(Collider2D other)
    	{
    		enteredobject = "";
   		}


}

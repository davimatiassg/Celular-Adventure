using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMaker : MonoBehaviour
{	
	[SerializeField] private GameObject Answer; 
    // Start is called before the first frame update
    // Update is called once per frame
   	void OnTriggerEnter2D(Collider2D other)
   	{
   		if(other.gameObject.tag.Equals("Player"))
   		{
   			Answer.SetActive(true);
        GameEvents.ScreamEvent("QuestFound");
   		}
   	}
}

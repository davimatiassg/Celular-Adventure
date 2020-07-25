using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SummaryButtons : MonoBehaviour
{

	public GameObject BestButton;

    void Awake()
    {
    	GameEvents.StartListening("GamePaused", BestiaryButton);
    }
	void OnDisable()
	{
		GameEvents.StopListening("GamePaused", BestiaryButton);
	}


    public void BestiaryButton()
    {	
    	if(GameObject.FindWithTag("Bestiary").GetComponent<BestiaryElements>().Bestiary.Count <= 0)
    	{
    		BestButton.GetComponent<Button>().interactable = false;  	 	
   		}
   		else
   		{
   			BestButton.GetComponent<Button>().interactable = true;
   		}

    }
}

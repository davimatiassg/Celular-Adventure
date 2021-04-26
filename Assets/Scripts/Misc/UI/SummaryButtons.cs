using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SummaryButtons : MonoBehaviour
{

	public GameObject BestButton;
  public GameObject AnButton;

    void Awake()
    {
    	GameEvents.StartListening("GamePaused", ToggleButtons);
    }
  	void OnDisable()
  	{
  		GameEvents.StopListening("GamePaused", ToggleButtons);
  	}


    public void ToggleButtons()
    {	
    	if(BestiaryElements.Bestiary.Count <= 0)
    	{
    		BestButton.GetComponent<Button>().interactable = false;  	 	
   		}
   		else
   		{
   			BestButton.GetComponent<Button>().interactable = true;
   		}

      if(AnotationManager.Notes.Count <= 0)
      {
        AnButton.GetComponent<Button>().interactable = false;     
      }
      else
      {
        AnButton.GetComponent<Button>().interactable = true;
      }

    }
}

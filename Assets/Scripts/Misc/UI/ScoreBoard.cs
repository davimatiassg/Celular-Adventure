using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreBoard : MonoBehaviour
{	
	[SerializeField] private Text ScoreShower;

    void OnEnable()
	{
		GameEvents.StartListening("GamePaused", UpdateScore);
	}
	void OnDisable()
	{
		GameEvents.StopListening("GamePaused", UpdateScore);
	}
    void UpdateScore()
    {	
    	Debug.Log(PontuationCounter.GetScoreString());
    	if(!ScoreShower)
    	{
    		ScoreShower = this.gameObject.GetComponent<Text>();
    	}
        
        ScoreShower.text = PontuationCounter.GetScoreString();
    }
}

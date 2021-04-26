using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontuationCounter : MonoBehaviour
{

    [SerializeField] private static int score;



    // Update is called once per frame
    public static void AddScore(int s)
    {	

        score += s;
        GameEvents.ScreamEvent("ScoreUp");
       
    }

    public static int GetScore()
    {
    	return score;
    }

    public static string GetScoreString()
    {	

    	return "Pontuação: " + score; 
    }
}

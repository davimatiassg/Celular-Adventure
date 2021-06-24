using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontuationCounter : MonoBehaviour
{

    [SerializeField] private static int score;


    void OnEnable()
    {
        GameEvents.StartListening("FadeOut", SaveScore);
    }
    void OnDisable()
    {
        GameEvents.StopListening("FadeOut", SaveScore);
    }


    public void Start()
    {
        if(score <= 0 && PlayerPrefs.HasKey("score"))
        {
            score = PlayerPrefs.GetInt("score");
        }
    }
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

    public void SaveScore()
    {
        PlayerPrefs.SetInt("score", GetScore());
        PlayerPrefs.Save();
    }
}

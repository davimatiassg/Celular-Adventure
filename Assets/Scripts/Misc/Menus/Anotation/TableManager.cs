using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{

    public List<string> TrueAwnsers = new List<string>();
    private List<string> TA;

    public int pontuation;

    private string str;

    void Start()
    {
        TA = new List<string>(TrueAwnsers);
    }

    public void VerifyIDString(string Idstr)
    {
        str = Idstr;
        GetAllCombinations();
    }

    public void GetAllCombinations()
    {	
        char[] delimit = new char[]{','};
        string[] splited = str.Split(delimit);
        if(TA.Contains(str) || splited[0] == splited[1])
        {
        	pontuation += 1;
        	TA.Remove(str);
            GameEvents.ScreamEvent("CorrectAwnser");
        }
        else
        {
            GameEvents.ScreamEvent("WrongAwnser");
        }
        PontuationCounter.AddScore(pontuation*1500);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{

    public List<string> TrueAwnsers = new List<string>();

    public int pontuation;

    public void GetAllCombinations()
    {	
    	List<string> TA = new List<string>(TrueAwnsers);
    

        Object[] combinations = Object.FindObjectsOfType<NoteLigator>();
        Debug.Log(combinations.Length);

        foreach(NoteLigator l in combinations)
        {	

        	string i = l.GetLigationIDS();
        	Debug.Log(i);
        	foreach(string t_awnser in TrueAwnsers)
        	{
        		if(i == t_awnser && TA.Contains(i))
        		{
        			pontuation += 1;
        			TA.Remove(t_awnser);
        		}
        	}
            Destroy(l);
        }
        Debug.Log(pontuation);
        GameEvents.ScreamEvent("ClosedTable");
        PontuationCounter.AddScore(pontuation*1500);
    }
}

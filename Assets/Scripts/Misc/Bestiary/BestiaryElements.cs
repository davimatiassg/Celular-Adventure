using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BestiaryElements : MonoBehaviour
{	
	public static BestiaryElements instance;
	public static List<CardIndex> Bestiary = new List<CardIndex>();
	

	void Awake()
	{
		if(instance == null)
        {
            instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
	}


	
	public static void AddCardEnemy(CardIndex note)
    {	
        bool alrNoted = false;
        int noteID = note.GetID();
        int Foundindex = -1;
        foreach(CardIndex nt in BestiaryElements.Bestiary)
        {   
            alrNoted = (nt.GetID() == noteID);
            if(alrNoted)
            {   
                Foundindex = BestiaryElements.Bestiary.IndexOf(nt);
                break;
            }
        }
        if(!alrNoted)
        {   
            BestiaryElements.Bestiary.Capacity ++;
            BestiaryElements.Bestiary.Add(note);
        }
        else
        {	
        	note.TimesKilled ++;
            BestiaryElements.Bestiary[Foundindex] = note;
            Debug.Log(noteID);
        }
        Debug.Log(BestiaryElements.Bestiary.Count);
    }
}


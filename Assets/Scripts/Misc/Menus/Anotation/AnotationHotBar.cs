using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotationHotBar : MonoBehaviour
{
	[SerializeField] private Transform Hotbar;
	[SerializeField] private GameObject AnHotBar;

    public void SpawnNotes()
    {	
    	foreach(Anotation note in AnotationManager.Notes)
    	{
    		if(note.pic)
            {
                GameObject pic = Instantiate(AnHotBar, Hotbar);
                pic.GetComponent<PhisicNote>().isText = false;
        		pic.GetComponent<PhisicNote>().SetInfo(note);
    		}
            
            if(note.text != "")
            {
                GameObject tex = Instantiate(AnHotBar, Hotbar);
                tex.GetComponent<PhisicNote>().isText = true;
                tex.GetComponent<PhisicNote>().SetInfo(note);
            }	
    	}


        List<CardIndex> best = new List<CardIndex>();

        foreach(CardIndex cd in BestiaryElements.Bestiary)
        {
            Anotation note = new Anotation(cd);

            bool arlspawned = false;

            foreach(CardIndex c in best)
            {   
                if(c.CInfo == cd.CInfo)
                {
                    arlspawned = true;
                    break;
                }

            }
            
            if(!arlspawned)
            {      
                best.Add(cd);
                if(note.pic)
                {        
                    GameObject pic = Instantiate(AnHotBar, Hotbar);
                    pic.GetComponent<PhisicNote>().isText = false;
                    pic.GetComponent<PhisicNote>().SetInfo(note);
                }

                if(note.text != "")
                {
                    GameObject tex = Instantiate(AnHotBar, Hotbar);
                    tex.GetComponent<PhisicNote>().isText = true;
                    tex.GetComponent<PhisicNote>().SetInfo(note);
                }
                    
            }  
        }
    }

    public void DestroyNotes()
    {
        for (int i = 0; i < Hotbar.childCount; i++)
        {
            Destroy(Hotbar.GetChild(i).gameObject);
        }
    }

    public void RemoveNote(Anotation a)
    {

    }
    public void AddNote(Anotation a)
    {
    	
    }
}

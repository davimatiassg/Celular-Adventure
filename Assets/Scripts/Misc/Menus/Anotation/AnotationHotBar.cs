using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotationHotBar : MonoBehaviour
{
	[SerializeField] private Transform Hotbar;
	[SerializeField] private GameObject AnHotBar;
    [SerializeField] private RelTableList ListInstance;

    public void SpawnNotes()
    {	
        Debug.Log("Notes Spawned");
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
                ListInstance.UpdateNotes(note);
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
                    ListInstance.UpdateNotes(note);
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

    public void TryRespawnNote()
    {
        Transform Cursor = GameObject.FindWithTag("Indicator").transform;

        if(Cursor.childCount > 0)
        {
            Anotation n = Cursor.GetChild(0).gameObject.GetComponent<PhisicNote>().InfoBase;
            GameObject pic = Instantiate(AnHotBar, Hotbar);
            pic.GetComponent<PhisicNote>().isText = false;
            pic.GetComponent<PhisicNote>().SetInfo(n);
            Destroy(Cursor.GetChild(0).gameObject);
        }
    }
}

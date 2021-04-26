using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotationManager : MonoBehaviour
{   
    public static AnotationManager instance;
    [SerializeField] public static List<Anotation> Notes = new List<Anotation>();


    void Awake()
    {   
        if(instance == null)
        {
            instance = this;
            Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }
        

    }
    

    public static void AddNote(Anotation note)
    {
        bool alrNoted = false;
        int noteID = note.GetID();
        int Foundindex = -1;
        foreach(Anotation nt in AnotationManager.Notes)
        {   
            alrNoted = (nt.GetID() == noteID);
            if(alrNoted)
            {   
                Foundindex = Notes.IndexOf(nt);
                break;
            }
        }
        if(!alrNoted)
        {   
            AnotationManager.Notes.Capacity ++;
            Notes.Add(note);
        }
        else
        {
            AnotationManager.Notes[Foundindex] = note;
        }
    }

}

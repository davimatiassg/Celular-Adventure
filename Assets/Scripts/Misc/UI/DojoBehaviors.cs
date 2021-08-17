using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DojoBehaviors : MonoBehaviour
{
    public GameObject Table;

	public void ToggleTableOn()
    {   
        GameObject.FindWithTag("Player").GetComponent<MasterController>().playable = false;        
        Table.SetActive(true);
        this.gameObject.GetComponent<AnotationHotBar>().SpawnNotes();
        GameEvents.ScreamEvent("ToogleCursor");
        
    }
    public void ToggleTableOff()
    {   
        GameEvents.ScreamEvent("ToogleCursor");   
        GameObject.FindWithTag("Player").GetComponent<MasterController>().playable = true;     
        Time.timeScale = 1f;
        Table.SetActive(false);
        this.gameObject.GetComponent<AnotationHotBar>().DestroyNotes();
    }
}

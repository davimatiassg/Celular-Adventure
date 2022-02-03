using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelTableList : MonoBehaviour
{
    public List<Anotation> Texts = new List<Anotation>();
    public List<Anotation> Notes = new List<Anotation>();
    public List<string> IDAwnser = new List<string>();

    [SerializeField] private int page = 0;

    public UnityEngine.UI.Text Label;
    public Transform Frame;
    public TableManager Verifier;

    private Transform Cursor;


    [SerializeField] private GameObject fNote;

    void OnEnable()
    {
        GameEvents.StartListening("CorrectAwnser", RemoveNote);
        
    }
    void OnDisable()
    {
        GameEvents.StopListening("CorrectAwnser", RemoveNote);
    }
    void Start()
    {
        Cursor = GameObject.FindWithTag("Indicator").transform;


    }

    public void UpdateNotes(Anotation a)
    {
        Texts.Add(a);
        Notes.Add(null);
       	ChangePage(0);
    }


    public void ChangePage(int i)
    {	
    	bool completed = false;
    	foreach(Anotation x in Texts)
    	{
    		if(x != null)
    		{
    			completed = true;
    		}
    	}
    	
    	if(page + i == Texts.Count)
    	{
    		page = 0;
    	}
    	if(page + i < 0)
    	{
    		page = Texts.Count - 1;
    	}
    	else
    	{
    		page += i;
    	}
    	
    	Label.text = Texts[page].text;
    	if(Texts[page] == null && completed == false)
    	{
    		

    		ChangePage(i);
    	}
    	if(Frame.childCount > 0)
    	{
    		Destroy(Frame.GetChild(0).gameObject);
    	}
    	SummonNote(Frame, Notes[page]);

    }

    public Anotation CatchCursorNote()
    {	
    	
    	if(Cursor.childCount > 0)
    	{	

    		Transform n = Cursor.GetChild(0);
    		Notes[page] = n.gameObject.GetComponent<PhisicNote>().InfoBase;
    		return Notes[page];
    	}
    	Notes[page] = null;
    	return null;
    }
    
    public Anotation CatchFrameNote()
    {	
    	
    	if(Frame.childCount > 0)
    	{	
    		Transform n = Frame.GetChild(0);
    		return n.gameObject.GetComponent<PhisicNote>().InfoBase;
    	}
    	return null;
    }

    void SummonNote(Transform trs, Anotation a)
    {
    	if(a != null)
    	{
			GameObject note = Instantiate(fNote, trs);
			note.GetComponent<PhisicNote>().isText = false;
    		note.GetComponent<PhisicNote>().SetInfo(a);   		
    	}
    	
    }

    public void TradeNotes()
    {	
    	Anotation f = CatchFrameNote();
    	Anotation n = CatchCursorNote();

    	if(n != null)
    	{
    		Destroy(Cursor.GetChild(0).gameObject);
    	}
    	SummonNote(Cursor, f);

    	if(f != null)
    	{
    		Destroy(Frame.GetChild(0).gameObject);
    	}
    	SummonNote(Frame, n);
    }

    public void GetLigationStr()
    {
    	string LigId = "-1,-1";
    	if(Notes[page] != null && Texts[page] != null)
    	{
    		LigId = Mathf.Min(Notes[page].ID, Texts[page].ID) + "," + Mathf.Max(Notes[page].ID, Texts[page].ID);
    	}
    	Debug.Log("A string dos ID's é " + LigId);
    	Verifier.VerifyIDString(LigId);
    }

    public void RemoveNote()
    {
    	if(Notes[page] != null)
    	{
    		Texts.RemoveAt(page);
    		Notes.RemoveAt(page);
    		ChangePage(1);
    	}

    }
}

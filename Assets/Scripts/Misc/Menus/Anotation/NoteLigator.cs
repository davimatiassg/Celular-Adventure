using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLigator : MonoBehaviour
{
    public GameObject Ob1;
    public GameObject Ob2;

    private Camera mainc;

    [SerializeField] private LineRenderer lRend;

    void Start()
    {	
    	mainc = Camera.main;
        lRend = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Ob1)
        {
        	lRend.SetPosition(0, Ob1.transform.position);
        }

        if(!Ob2)
        {
        	lRend.SetPosition(1, mainc.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
        	lRend.SetPosition(1, Ob2.transform.position);
        }
    }

    public string GetLigationIDS()
    {
    	string LigId = "-1,-1";
    	if(Ob1 && Ob2)
    	{
    		LigId = Mathf.Min(Ob1.GetComponent<PhisicNote>().ID, Ob2.GetComponent<PhisicNote>().ID) + "," + Mathf.Max(Ob1.GetComponent<PhisicNote>().ID, Ob2.GetComponent<PhisicNote>().ID);
    	}
    	return LigId;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiResScaler : MonoBehaviour
{
private RectTransform trs;
    private Transform trs2;


    [SerializeField] private Vector2 pos = Vector2.zero;

    [SerializeField] private Vector2 sc = Vector2.zero;
    [SerializeField] private Vector2 sd = Vector2.one;

    [SerializeField] private bool defaultTRS = true;
    [SerializeField] private bool texted = false;

	[SerializeField] private bool w_x_h = true;
    [SerializeField] private bool orthographic = true;

    private Camera c;
    private Vector2 img_dim = Vector2.one;
    [SerializeField] private float modvalue = 1; 



    void Start()
    {
    	trs = this.gameObject.GetComponent<RectTransform>(); 
        trs2 = this.gameObject.GetComponent<Transform>();
        c = Camera.main;
  
        

    	if(pos == Vector2.zero)
    	{
			if(defaultTRS)
		    {
		        pos = trs2.position;
		        sc = trs2.localScale;
		    }
		    else
		    {
		    	pos = trs.anchoredPosition; 
		    	sc = trs.localScale;
		    }
    	}

        if(orthographic)
        {
            AdaptToCurrentResolution(1f, 1f);
        }
        else
        {
            AdaptToCurrentResolution(sd.x, sd.y);
        }
        

    }


    void AdaptToCurrentResolution(float wid = 1, float hei = 1)
    {
    	w_x_h = (c.aspect > 16/9);
    	Vector2 res = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

		if(w_x_h)
	    {
	    	
	    	modvalue = res.x/Screen.width;
	    }
	    else
	    {
	    	modvalue = res.y/Screen.height;

	    }
	    //modvalue = modvalue/Mathf.Floor(modvalue);
	    Debug.Log("MOD VALUE: " + modvalue);

    	if(defaultTRS)
    	{
    		trs2.localScale =  new Vector3(sc.x*modvalue/wid, sc.y*modvalue/hei, 1f);
    	}
    	else
        {

            trs.anchoredPosition = new Vector3((pos.x/modvalue)/wid, (pos.y/modvalue)/hei, 1f);
            if(texted)
            {
                trs.localScale = new Vector3((sc.x/modvalue)/wid, (sc.y/modvalue)/hei, 1f);
            }
            else
            {
                trs.sizeDelta = trs.sizeDelta*modvalue;
            }
                     
        }   

    }
}

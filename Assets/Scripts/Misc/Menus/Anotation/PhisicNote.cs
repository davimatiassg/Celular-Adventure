using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhisicNote : MonoBehaviour
{	
	[SerializeField] private Texture BlankNote;

	public bool isText;

    [SerializeField] public Anotation InfoBase;

    [SerializeField] public GameObject ImageObj;

    [SerializeField] private RawImage spr;

    [SerializeField] private SpriteRenderer spR;

    [SerializeField] private TextMesh label;

    [SerializeField] private TMP_Text label2;

    public int ID = -1;


    void Awake()
    {
    	spr = ImageObj.GetComponent<RawImage>();
        if(!spr)
        {
           spR = ImageObj.GetComponent<SpriteRenderer>();
        }   
    }



    public void SetInfo(Anotation i)
    {	
    	InfoBase = i;
    	if(spR)
        {
            spR.enabled = false;
        }
        else
        {
            spr.enabled = false;
        }
    	if(isText)
    	{	
			if(label != null)
    		{
    			

    			label.text = i.text;
    		}
    		else if(label2 == null)
    		{ 
                if(spr)
                {
                    spr.enabled = true;
                }
    			
    			spr.texture = BlankNote;
    		}
            else
            {   
                label2.text = i.text;
            }

    	}
    	else
    	{	
    		if(label)
    		{
    			label.gameObject.SetActive(false);
    		}
            if(label2)
            {
                label2.gameObject.SetActive(false);
            }

            if(spr)
            {
                spr.enabled = true;
                spr.texture = i.pic;
            }
            else
            {

                spR.enabled = true;
                Debug.Log("Width: " + i.pic.width + "; Height: " + i.pic.height);
                spR.sprite = Sprite.Create((Texture2D)i.pic, new Rect(0.0f, 0.0f, i.pic.width, i.pic.height), Vector2.one/2);

                float spc = (spR.sprite.pixelsPerUnit/(i.pic.width/4.8f)) * 0.2f ;

                spR.gameObject.GetComponent<RectTransform>().localScale = Vector2.one * spc;
            }
    		
    	}
    	ID = i.ID;
    	
    }

    



}

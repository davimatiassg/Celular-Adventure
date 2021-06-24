using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PannelElements : MonoBehaviour
{
    private RectTransform trs;
    private Transform trs2;


    [SerializeField] private Vector2 pos = Vector2.zero;

    [SerializeField] private Vector2 sc = Vector2.zero;

    [SerializeField] private bool basedOnCamSize = false;

    private Camera c;



    void Start()
    {
    	trs = this.gameObject.GetComponent<RectTransform>(); 
        trs2 = this.gameObject.GetComponent<Transform>();
        c = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {   


    	if(basedOnCamSize)
        {
            trs2.localScale = sc*c.orthographicSize*5/9; 
        }
        else
        {
            trs.anchoredPosition = pos*Screen.width;
            trs.sizeDelta = sc*Screen.width; 
        }

        
    }
}

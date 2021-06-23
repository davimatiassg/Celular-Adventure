using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PannelElements : MonoBehaviour
{
    private RectTransform trs;

    [SerializeField] private Vector2 pos = Vector2.zero;

    [SerializeField] private Vector2 sc = Vector2.zero;



    void Start()
    {
    	  trs = this.gameObject.GetComponent<RectTransform>(); 
    }

    // Update is called once per frame
    void Update()
    {

    		
        trs.anchoredPosition = pos*Screen.width;
        trs.sizeDelta = sc*Screen.width;
    }
}

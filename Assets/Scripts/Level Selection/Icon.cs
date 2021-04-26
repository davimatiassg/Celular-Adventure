using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Icon : MonoBehaviour
{	
	public GameObject Selecter;
	public Sprite SplashScreen;
    public Sprite Player;
	[SerializeField] public UnityEngine.UI.Image Screen;
	public Text label;
    [TextArea] 
	public string text; 
	public GameObject Pannel;
	public string scene;
	public MainMenu startb;
	[SerializeField] private bool isselected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !Pannel.activeSelf && isselected)
        {
        	Screen.sprite = SplashScreen;
        	label.text = text;
        	startb.scenename = scene;
        	Pannel.SetActive(true);
            GameEvents.StartListening("ContentUpdate", ToggleContent);

        }
        else if(Input.GetMouseButtonDown(0) && !Pannel.activeSelf)
        {
            GameEvents.StopListening("ContentUpdate", ToggleContent);
        }
    }
    void OnMouseEnter()
    {
    	Selecter.GetComponent<Transform>().position = this.gameObject.transform.position;
    	Selecter.GetComponent<Cursor>().Attached = true;
    	isselected = true;

        
    }
    void OnMouseExit()
    {
    	Selecter.GetComponent<Cursor>().Attached = false;
    	isselected = false;
        
    }

    public void ToggleContent()
    {
        if(Screen.sprite == SplashScreen)
        {
            Screen.sprite = Player;
        }
        else
        {
            Screen.sprite = SplashScreen;
        }
        
    }
}

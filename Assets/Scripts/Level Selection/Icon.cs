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
    [SerializeField] private bool selectable = true;
    [SerializeField] private int level;
    // Start is called before the first frame update
    void Start()
    {
        
        int l = 0;
        SpriteRenderer s= this.gameObject.GetComponent<SpriteRenderer>();
        if(PlayerPrefs.HasKey("level"))
        {
          l = PlayerPrefs.GetInt("level");
        }
        else
        {
            PlayerPrefs.SetInt("level", 0);
        }
        if(l < level -1)
        {
            selectable = false;
            s.color = new Color(1, 0, 0, 0.3f); 
        }
        else
        {
            selectable = true;
            s.color = new Color(1, 1, 1, 1f); 
        }
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
        if(selectable)
        {
            Selecter.GetComponent<Transform>().position = this.gameObject.transform.position;
            Selecter.GetComponent<Cursor>().Attached = true;
            isselected = true;
        }


        
    }
    void OnMouseExit()
    {
        if(selectable)
        {
        	Selecter.GetComponent<Cursor>().Attached = false;
        	isselected = false;
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
	public List<string>  phrases = new List<string>();
	[SerializeField]
	public List<Texture> photo = new List<Texture>(); 
	[SerializeField]
    public List<string>  name = new List<string>();
    [SerializeField]
    private List<string> showask = new List<string>();

	[SerializeField]
    private bool visible;
    [SerializeField]
    private bool interactable;
    [SerializeField]
	private bool pause;
	[SerializeField]
	private bool destroyonread;

    [SerializeField]
	public DialogBox TextBox;

	[SerializeField]
	private bool showing = false;

	private int counterframes = 0;

    // Update is called once per frame
    void Update()
    {	
    	if(counterframes == 0)
    	{
    		TextBox.runmsg = showing;
    	}
        else
        {
        	counterframes -= 1;
        }
    }
    void  OnTriggerEnter2D(Collider2D other)
    {
    	if(other.gameObject.tag.Equals("Player"))
    	{	
    		if(!interactable)
    		{
    			showmsg();
    		}
    		else
    		{	
    			showing = false;
    			TextBox.gameObject.SetActive(true);
    			TextBox.sentences = showask;
    			TextBox.msgnum = 0;
    		}
    	}
    }
    void OnTriggerStay2D(Collider2D other)
    {
    	if(other.gameObject.tag.Equals("Player"))
    	{
    		if(interactable && Input.GetKeyDown(KeyCode.Space) && !showing)
    		{	

    			showmsg();
    			TextBox.msgnum = 0;
    			counterframes += 2;
    		}
    	}
    }
    void OnTriggerExit2D(Collider2D other)
    {
    	if(other.gameObject.tag.Equals("Player") && !showing)
    	{
    		TextBox.anim.Play("Dialog Out");
    		TextBox.msgnum = 0; 
    	}
    }


    void showmsg()
    {

    	TextBox.gameObject.SetActive(true);
   		TextBox.sentences = phrases;
   		TextBox.character = photo;
   		TextBox.charname = name;
   		TextBox.stop = pause;
   		TextBox.msgnum = 0; 
   		showing = true; 		
   		if(destroyonread)
   		{	
   			Destroy(this.gameObject);
   		}
   			
    }
}

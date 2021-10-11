using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    // Start is called before the first frame update
    public List<DialogContent> dialogSequence = new List<DialogContent>();

    private DialogContent currentDialog;

    [SerializeField] private List<GameObject> buttons = new List<GameObject>(){null, null};

    [SerializeField] private GameObject Box;

    [SerializeField] private Text label;

    public InputManager InPut;

    [SerializeField] private Text namelabel;

    [SerializeField] private Image picture;
    
    public int msgnum = 0;
   
    public Animator anim;


    void Awake()
    {   
        anim = this.gameObject.GetComponent<Animator>();
        
    }
    void OnEnable()
    {
        GameEvents.StartListening("GetDialogBox", SetDialogBox);
        InPut = InputManager.instance;

    }
    void OnDisable()
    {
        GameEvents.StopListening("GetDialogBox", SetDialogBox);
    }

    // Update is called once per frame
    void Update()
    {   
        if(Box.activeSelf)
        {   
            if(InPut.GetButtonDown("Jump")|| InPut.GetButtonDown("up"))
            {      

                sendNextMsg();
            }
        }    
    }

    void sendNextMsg()
    {   
        if(msgnum <= dialogSequence.Count)
        {
           

            
            if(msgnum == dialogSequence.Count)
            {
            	Debug.Log("Dialog Number " + (msgnum +1) + "/" + (dialogSequence.Count) + " completed - Exiting dialogSequence");
                Time.timeScale = 1f;
                anim.Play("Dialog Out");
                dialogSequence = new List<DialogContent>();
                msgnum = 0;
                currentDialog.FinalAction();
                currentDialog = null;
            }
            else
            {
				Debug.Log("Dialog Number " + (msgnum +1) + "/" + (dialogSequence.Count));
            	currentDialog = dialogSequence[msgnum];

            	label.text = currentDialog.GetMainText();
            	namelabel.text = currentDialog.GetOwnerName();
            	picture.sprite = currentDialog.GetOwnerPhoto();
            	currentDialog.FinalAction();
            	msgnum ++;
            }
            
        }
        else
        {
            Time.timeScale = 1f;
            anim.Play("Dialog Out");
            dialogSequence = new List<DialogContent>();
            msgnum = 0;
            currentDialog.FinalAction();
            currentDialog = null;        	
        }

    }
    void EnterChildren()
    {   
        if(dialogSequence.Count != 0)
        {   
            namelabel.gameObject.SetActive(true);
            label.gameObject.SetActive(true);
            picture.gameObject.SetActive(true);
            label.text = dialogSequence[0].GetMainText();
            namelabel.text = dialogSequence[0].GetOwnerName();
            picture.sprite = dialogSequence[0].GetOwnerPhoto();
        }
    }

    void ExitChildren()
    {   
        namelabel.gameObject.SetActive(false);
        label.gameObject.SetActive(false);
        picture.gameObject.SetActive(false);
    }
    void Exit()
    {
        Box.SetActive(false);
    }

    void SetDialogBox()
    {
        DialogInitializer.TextBox = this;
        msgnum = 0;
    }

    public void Activate(bool state)
    {   
    	if(state)
    	{	
    		Box.SetActive(true);
	    	anim.Play("Dialog In");	
    	}
    	else
    	{	
    		anim.Play("Dialog Out");
    	}

    }
}

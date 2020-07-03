using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogBox : MonoBehaviour
{
    // Start is called before the first frame update

    public List<string> sentences = new List<string>();
	public List<Texture> character = new List<Texture>(); 
    public List<string> charname = new List<string>(); 
    public bool stop;

    public bool runmsg = false;

    [SerializeField]
    private Text label;
    [SerializeField]
    public int msgnum = 0;
    public Animator anim;

    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        label.text = sentences[msgnum];
        //falta ainda repetir a linha acima para as listas dos nomes dos personagens e para as imagens deles.
        if(stop)
        {
        	Time.timeScale = 0f;
        }
        if(Input.GetKeyDown(KeyCode.Space) && runmsg)
        {
        	NextMsg();
        }
    }
    void NextMsg()
    {
    	
    	if(msgnum >= sentences.Count-1)
    	{
    		anim.Play("Dialog Out");
    		
    		Time.timeScale = 1f;
    		stop = false;

    		label.gameObject.SetActive(false);
    		//falta ainda repetir a linha acima para as listas dos nomes dos personagens e para as imagens deles.
    	}
    	else
    	{
    		msgnum ++;
    		
    	}
    }
    void EnterChildren()
    {
    	label.gameObject.SetActive(true);
    	//falta ainda repetir a linha acima para as listas dos nomes dos personagens e para as imagens deles.
    }
    void Exit()
    {
    	this.gameObject.SetActive(false);
    }
}

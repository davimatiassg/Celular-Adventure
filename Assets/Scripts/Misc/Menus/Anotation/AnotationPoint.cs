	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotationPoint : MonoBehaviour
{	
	[SerializeField] private bool Attached = false;
   	[SerializeField] private AnotationManager AnBook;

   	[SerializeField] private ParticleSystem part;

   	[SerializeField] private Texture sp;
   	[SerializeField] private int id;

   	[SerializeField] public GameObject ao;
   	[SerializeField] public AudioInterface a;
    
    [TextArea]
    [Tooltip("Nome do inimigo (Identificador)")]
    [SerializeField] private string txt;

   	[SerializeField] private bool destroyonread = true;

    void Awake()
    {
      AnBook = GameObject.Find("AnotationBook").GetComponent<AnotationManager>();
      a = ao.GetComponent<AudioInterface>();
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D other)
    {
    	if(!Attached)
    	{
	    	if(other.gameObject.tag == "Player" && InputManager.instance.GetAxisRaw("Vertical") > 0)
	    	{	
	    		AdcthisNote();
	    		if(destroyonread)
	    		{	
	    			a.PlaySound("got");
	    			part.Stop();
	    			this.gameObject.transform.DetachChildren();
            this.gameObject.SetActive(false);
	    			Destroy(this.gameObject);
	    		}
	    	}
	    }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    	if(!Attached)
    	{	
    		if(other.gameObject.tag == "Player")
    		{	
    			a.PlaySound("see");
    			part.Play();
    		}
    	}
    }

    void OnTriggerExit2D(Collider2D other)
    {	
    	if(!Attached)
    	{
	    	if(other.gameObject.tag == "Player")
	    	{	
	    		part.Stop();
	    	}
	    }
    }

    public void AdcthisNote()
    {
    	AnotationManager.AddNote(new Anotation(id, sp, txt));
      PontuationCounter.AddScore(1000);
    }
}

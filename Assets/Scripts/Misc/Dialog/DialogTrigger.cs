using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogInitializer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DialogTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private DialogInitializer logStarter;

    [SerializeField]
    private bool interactable;
    [SerializeField]
	private bool pause;
	[SerializeField]
	private bool destroyonread;

	[SerializeField]
	private bool showing = false;

    void Start()
    {
        logStarter = this.gameObject.GetComponent<DialogInitializer>();
    }
    void OnTriggerStay2D(Collider2D other)
    {
    	if(other.gameObject.tag.Equals("Player"))
    	{      
            if(!interactable || Input.GetAxisRaw("Vertical") > 0 || Input.GetButtonDown("Jump"))
            {   
                logStarter.OpenDialogBox();
                showing = true;
                if(pause)
                {
                    Time.timeScale = 0f;
                }
                if(destroyonread)
                {
                    Destroy(this.gameObject);
                }

            }

        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
    	if(other.gameObject.tag.Equals("Player") && showing && interactable)
    	{
            logStarter.CloseDialogBox();
            showing = false;
            
    	}
    }
}

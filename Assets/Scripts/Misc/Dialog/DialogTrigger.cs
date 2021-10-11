using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogInitializer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DialogTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float range;
    private DialogInitializer logStarter;
    private InputManager InPut;
    private Transform p;
    private Vector2 t_pos;
    private bool inside = false;
    private bool showing = false ;

    [SerializeField]
    private bool interactable;
    [SerializeField]
	private bool pause;
	[SerializeField]
	private bool destroyonread;

	[SerializeField]


    void Start()
    {
        logStarter = this.gameObject.GetComponent<DialogInitializer>();
        InPut = InputManager.instance;
        p = GameObject.FindWithTag("Player").transform;
        t_pos = this.gameObject.transform.position;
    }
    void Update()
    {
        float d = ((Vector2)p.position - t_pos).sqrMagnitude;
        bool i = (d <= range*range);

        if(inside != i)
        {      
            inside = i;
            if(!inside)
            {  
                showing = false;
                ExitRange();
            }
            
        }
        if(inside)
        {
            EnterRange();
        }
        
    }

    void ExitRange()
    {
        Debug.Log("OutRange");
        logStarter.CloseDialogBox();
    	
    }
    void EnterRange()
    {
        Debug.Log("Inrange");
        if((!interactable || InPut.GetButtonDown("Jump") || InPut.GetAxisRaw("Vertical") > 0) && !showing)
        {   
            showing = true;
            logStarter.OpenDialogBox();

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
    



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, range*range);
    }
}

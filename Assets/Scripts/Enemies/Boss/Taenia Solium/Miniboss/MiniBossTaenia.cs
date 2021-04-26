using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossTaenia : MonoBehaviour
{     
    public float atkCD;
    private float Cd;
    private Animator anim;
    private Transform trs;
    private Transform Player;
    [SerializeField] private GameObject Body;
    private Transform b_trs;

    [SerializeField] private GameObject Boulders;

    public int atkq = 0;
    // Start is called before the first frame update
    void Start()
    {      
        Cd = -1; 
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
        anim = this.gameObject.GetComponent<Animator>();
        //Player.GetComponentInChildren(typeof(SpriteRenderer)).gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.625f, 0.625f);
        b_trs = Body.GetComponent<Transform>();

    }
    void OnEnable()
    {   
        transform.position += Vector3.down * 20;

    }
    // Update is called once per frame
    void Update()
    {   

        if(trs.position.y < -55)
        {
            trs.Translate(Vector3.up*5*Time.deltaTime);
            b_trs.localPosition = Vector3.down * 60;
            
        }
        else
        {
            Body.SetActive(true);
            if(b_trs.localPosition.y < -15)
            {   
                Body.GetComponent<Animator>().Play("wall-sand");
                b_trs.Translate(Vector3.up*30*Time.deltaTime);
            }
            else if(!Body.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("notsand"))
            {   
                Body.GetComponent<Animator>().Play("wall-staytrans");
            }
            else if(Body.GetComponent<CombatEnemy>().life > 0)
            {
                if(Cd < atkCD)
                {
                    Cd += Time.deltaTime;
                }
                else
                {   
                    Cd = 0;
                    anim.Play("Atk");
                }
            }
        }
    }

    void Spawn_boulders()
    {
        for (int i = 0; i < atkq; i++ )
        {   
            Vector3 spawnlocal = new Vector3(Random.Range(450f, 478f), -29, 0f); 
            if(i == 0)
            {
                spawnlocal = new Vector3(Player.position.x, -29, 0f);
            }
            Instantiate(Boulders, spawnlocal, new Quaternion(0,0,0,0));
        }
    }
}

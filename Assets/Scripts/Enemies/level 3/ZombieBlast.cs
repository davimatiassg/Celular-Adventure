using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBlast : MonoBehaviour
{
    private CombatEnemy thisZombie;

    [SerializeField] private GameObject blast;
    void Start()
    {
     	thisZombie = GetComponent<CombatEnemy>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(thisZombie.life <= 0)
        {
        	Instantiate(blast, this.gameObject.transform.position + Vector3.down*this.gameObject.transform.localScale.y*0.3f, this.gameObject.transform.rotation);
        	Destroy(this.gameObject);
        }
    }
}

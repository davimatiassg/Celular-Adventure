using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableWalls : MonoBehaviour
{
    // Start is called before the first frame update
	private Animator anim;
	private CombatEnemy master;

	void Start()
	{
		anim = this.gameObject.GetComponent<Animator>();
		master = this.gameObject.GetComponent<CombatEnemy>();
	}

    // Update is called once per frame
    void Update()
    {
        if(master.life <= 0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("wall-dead"))
        {
        	anim.Play("wall-dead");
        }
        if(master.stuncheck() && !anim.GetCurrentAnimatorStateInfo(0).IsName("wall-dead"))
        {
        	anim.Play("wall-damage");
        }
    }
}

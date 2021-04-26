using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class TiledReact : MonoBehaviour
{	
	private Animator anim;
    private SpriteRenderer spr;
    private BoxCollider2D bx;
    private Transform trs;
    private ParticleSystem.ShapeModule sg;
    void Awake()
    {
        bx = this.gameObject.GetComponent<BoxCollider2D>();
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        trs = this.gameObject.GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {	
    	bx = this.gameObject.GetComponent<BoxCollider2D>();
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        trs = this.gameObject.GetComponent<Transform>();
    	trs.position = new Vector3(Mathf.RoundToInt(trs.position.x), Mathf.RoundToInt(trs.position.y), Mathf.RoundToInt(trs.position.z));
    	spr.size = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
        bx.size = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
        ParticleSystem part = this.gameObject.GetComponent<ParticleSystem>();
        if(part != null)
        {
            sg = part.shape;
            sg.enabled = true;
        }
        sg.scale = new Vector2(Mathf.RoundToInt(spr.size.x), Mathf.RoundToInt(spr.size.y));
    }
}

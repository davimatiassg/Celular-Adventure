using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewParallax : MonoBehaviour
{	
	[SerializeField] private Vector4 limit;
	[SerializeField] private Vector2 parallax_factor;
    [SerializeField] private Vector2 compensation = Vector2.zero;
    [SerializeField] private float startpos = 0f;
    private int tex_index = 0;
    public List<Sprite> nextTextures = new List<Sprite>();
    private float lenght;
    //private float temp;
    
	[SerializeField] private Transform camera_trs;
	private Vector3 last_cam_position;

    [SerializeField] private SpriteRenderer spr;

    public bool parallaxing = false;

    // Start is called before the first frame update
    void Start()
    {   
        camera_trs = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        //last_cam_position = camera_trs.position;
        spr = this.gameObject.GetComponent<SpriteRenderer>();
        lenght = spr.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        float temp = (camera_trs.position.x * (1f - parallax_factor.x));
        float dist = camera_trs.position.x * parallax_factor.x;

        transform.position = new Vector3(startpos + camera_trs.position.x + dist, transform.position.y, transform.position.z);

        if(temp > startpos + lenght + camera_trs.position.x)
        {
        	startpos += lenght;
        }
       	else if(temp < startpos - lenght + camera_trs.position.x)
        {
        	startpos -= lenght;
        }
    }
    public void ChangeBackground(int t)
    {
    	if(t < nextTextures.Count && nextTextures[t] != null)
    	{
    		spr.sprite = nextTextures[t];
    		int i = 0;
    		while(i < transform.childCount)
    		{
    			transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = nextTextures[t];
    			i += 1;
    		}
    		tex_index = t;
    	}
    }
}

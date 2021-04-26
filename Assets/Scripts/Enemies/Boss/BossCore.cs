using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCore : MonoBehaviour
{	
	public bool showlifebar;
	public float damageMultiplicator = 1;
	public int life;
	public int maxlife;
	public float dmgtime;
	public int damage;
	public bool isinvencible = false;
	public bool isdangerous = true;

	public Transform trs;

	private float blinktime;

	public Texture hplevel, frame, bottom;
	void Start()
    {	
    	trs = this.gameObject.GetComponent<Transform>();
        life = maxlife;
    }

    // Update is called once per frame
    void Update()
    {
        if(blinktime > 0f)
        {
        	isinvencible = true;

        	blinktime -= Time.deltaTime;
        }
        else
        {
        	isinvencible = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && other.isTrigger == false && isdangerous)
		{
			var hited = other.gameObject.GetComponent<MasterController>();
			hited.takedamage(damage);
		}

	}
	public void takedamage(int dmg)
	{

		if(!isinvencible)
		{
			life -= Mathf.RoundToInt(dmg*damageMultiplicator);
			GameEvents.ScreamEvent("BossDamaged");
			blinktime = dmgtime;
			if (life <= 0)
			{	
				GameEvents.ScreamEvent("BossDie");
				Debug.Log("BossdieEvent");
			}
		}
	}

	void OnGUI ()
 	{	
 		if(showlifebar)
 		{
	 		GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), bottom);
	 		if(life > 0)
	 		{
	 			GUI.DrawTexture (new Rect (Screen.width*0.405f, Screen.height*0.874f, Screen.width*0.587f*life/maxlife, Screen.height*0.117f), hplevel);
	 		}
	 			
	 		GUI.DrawTexture (new Rect (Screen.width*0.37f, Screen.height*0.85f, Screen.width*2/3, Screen.height/6), frame); 			
 		}

 	}

 	public void sendEvent(string eventname)
 	{
 		GameEvents.ScreamEvent(eventname);
 	}
 	public void changeCamScale(float scale)
 	{
 		GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().camscale = scale;
 	}
 	public void changeCamPositionx(float positionx)
 	{	
 		GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().posx = positionx;
 	}
 	 public void changeCamPositiony(float positiony)
 	{	
 		GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>().posy = positiony;
 	}
 	public void ToggleCamTarget(float targetHeight = 5)
 	{	
 		var cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraBehavior>();
 		if(cam.targeted)
 		{
 			cam.posx = trs.position.x;
 			cam.posy = trs.position.y + targetHeight;
 		}
 		cam.targeted = !cam.targeted;
 	}




}
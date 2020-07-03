using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedFallBehavior : MonoBehaviour {
	private Animator anim;
	public float BleedTime;
	public float BleedDelay;
	private float ActualTime;
	public bool IsBleeding;
	public  bool laststate;
	public GameObject Blood;
	private Transform trs;
	private GameObject blfalling;
	private Blood bloodmain;
	public float MaxRange = 1;

	public ParticleSystem bldparticles;
	// Use this for initialization
	void Start () {
		
		anim = GetComponent<Animator> ();
		trs = GetComponent<Transform> ();
		ActualTime = BleedDelay;
		IsBleeding = false;
		laststate = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		ActualTime -= Time.deltaTime;

		if (ActualTime <= 0)
		{
			if(IsBleeding)
			{
				ActualTime = BleedDelay;
				IsBleeding = false;
				
			}
			else
			{
				ActualTime = BleedTime;
				IsBleeding = true;
				
			}
		}





		if(laststate == false && IsBleeding)
		{
				anim.Play("Start");
				
				blfalling = Instantiate(Blood, new Vector3(trs.position.x, trs.position.y-1f, trs.position.z), trs.rotation);
				bloodmain = blfalling.GetComponent<Blood>();

		}
		if(IsBleeding)
		{	
			if(bloodmain.length < MaxRange)
			{
				bloodmain.length += 0.1f;
			}
			else
			{
				bloodmain.length = MaxRange;
			}
			laststate = true;
			bldparticles.Stop();
		}

		else
		{	
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Clean"))
			{
				anim.Play("Stop");
				bldparticles.Play();
			}

			
		}
		
		if(!IsBleeding && laststate == true)
		{
			if(bloodmain.length > 0.0f)
			{
				bloodmain.FlipY = true;
				bloodmain.length -= 0.1f;
				bloodmain.yflipct = trs.position.y - 2*MaxRange;
			}
			else
			{	
				laststate = false;
				Destroy(blfalling);
			}
		}


	}
}

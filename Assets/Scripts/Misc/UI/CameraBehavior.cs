using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {


	public bool lvlselection = false;
	
	private Vector2 velocity;
	private Vector2 refer;

	public static Transform target;
	public Camera cam;

	public float camsensex;
	public float camsensey;
	public float edgeleft;
	public float edgeright;
	public float edgeup;
	public float edgedown;
	public float camscale;
	

	//Efeitos de status
		// tremor de camera
	public bool shaking;
	public float shkintenX;
	public float shkintenY;
	private int shkX = 1;
	private int shkY = 1;
		// camera livre(false) ou presa a um objeto(true)
	public bool targeted;

	public float posx;
	public float posy;
	public float desx;
	public float desy;
	// Update is called once per frame



	void OnEnable()
	{
		GameEvents.StartListening("BossAreaEntered", ToggleToBossCamera);
		GameEvents.StartListening("BossAreaExited", ToggleToPlayerCamera);
		refer = new Vector2(1.0f , 1.0f);
		

	}
	void OnDisable()
	{
		GameEvents.StopListening("BossAreaEntered", ToggleToBossCamera);
		GameEvents.StopListening("BossAreaExited", ToggleToPlayerCamera);
	}
	void FixedUpdate () {

		if(!target)
		{	
			target = GameObject.FindWithTag("Player").GetComponent<Transform>();
		}
		cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, camscale, ref refer.x, (camsensex+camsensey)/2);



		if(targeted)
		{
			desx = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, camsensex);
			desy = Mathf.SmoothDamp(transform.position.y, target.position.y+1, ref velocity.y, camsensey);
			if(desx < edgeleft)
			{
				desx = edgeleft;
			}
			if (desx > edgeright)
			{
				desx = edgeright;
			}
			if (desy > edgeup)
			{
				desy = edgeup;
			}
			if (desy < edgedown)
			{
				desy = edgedown;
			}
		}

		else
		{
			desx = Mathf.SmoothDamp(transform.position.x, posx, ref velocity.x, camsensex*15);
			desy = Mathf.SmoothDamp(transform.position.y, posy, ref velocity.y, camsensey*15);
		}
		



		if(shaking)
		{
			shkX = shkX * -1;
			shkY = shkY * -1;
			desx += shkX * shkintenX;
			desy += shkY * shkintenY;
		}

		transform.position = new Vector3 (desx, desy, transform.position.z);
		

	}

	private void ToggleToBossCamera()
	{
		targeted = false;
	}
	private void ToggleToPlayerCamera()
	{
		targeted = true;
	}
	public void ToggleShake(bool state, float x, float y)
	{
		shaking = state;
		shkintenY = y;
		shkintenX = x;
	}

}

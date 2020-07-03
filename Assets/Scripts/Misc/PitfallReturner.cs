using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallReturner : MonoBehaviour {


	private Transform trs;
	// Use this for initialization
	void Start()
	{
		trs = GetComponent<Transform> ();
	}
	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player"))
		{
			var hited = other.gameObject.GetComponent<PlayerBehavior>();
			hited.GroundSave(new Vector2(trs.position.x, trs.position.y));
		}

	}
}

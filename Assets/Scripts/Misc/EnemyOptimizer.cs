using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOptimizer : MonoBehaviour
{
	[SerializeField] private Transform p;

	[SerializeField] private List<GameObject> e = new List<GameObject>();

	[SerializeField] private float screensize;
	
	void Start()
	{	
		p = GameObject.FindWithTag("Player").GetComponent<Transform>();
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("hitable"))
		{	
			if(enemy.activeSelf)
			{
				e.Add(enemy);
			}		
			
		}
		Debug.Log(e);
		screensize = Mathf.Pow(Screen.width, 1);
	}
	void FixedUpdate()
	{	
		foreach(GameObject enemy in e)
		{	
			if(enemy == null)
			{
				e.Remove(enemy);
			}
			float a = ((Vector2)p.position - (Vector2) enemy.transform.position).sqrMagnitude;
			if(enemy.activeSelf)
			{
				if(screensize < a)
				{
					enemy.SetActive(false);
				}
			}
			else
			{
				if(screensize > a)
				{
					enemy.SetActive(true);
				}
			}
		}
	}
}

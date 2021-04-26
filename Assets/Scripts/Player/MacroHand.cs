using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroHand : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private MacroBehavior mainCode; 
    public GameObject grabedenemy;
    // Update is called once per frame
    void Update()
    {
        if(mainCode.GetGrab())
        {
        	GrabEnemy(grabedenemy);
        }
        else
        {	
        	if(grabedenemy)
        	{	
        		grabedenemy.GetComponent<CombatEnemy>().SetStuned(0.2f);
        		grabedenemy = null;
        	}
        	
        }
    }

    public void SearchEnemies()
    {	
		float distanceToClosestEnemy = radius;
		GameObject closestEnemy = null;
		GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("hitable");

		foreach (GameObject currentEnemy in allEnemies)
		{	
			float distanceToEnemy = new Vector2(Mathf.Abs(currentEnemy.transform.position.x - this.transform.position.x),Mathf.Abs(currentEnemy.transform.position.y - this.transform.position.y)).magnitude;
			
			if (distanceToEnemy < distanceToClosestEnemy) 
			{	
				distanceToClosestEnemy = distanceToEnemy;
				closestEnemy = currentEnemy;
			}
		}
		if(closestEnemy)
		{	

			GrabEnemy(closestEnemy);
			mainCode.ToggleGrabOn();
			
		}
    }

    public void SearchEnemies(GameObject enemy)
    {   
        GrabEnemy(enemy);
        mainCode.ToggleGrabOn();

    }
    public void GrabEnemy(GameObject enemy)
    {	
    	enemy.GetComponent<CombatEnemy>().SetStuned(0.5f);
    	grabedenemy = enemy;
    	enemy.transform.position = this.transform.position;
    }

    public void damageEnemy()
    {
    	grabedenemy.GetComponent<CombatEnemy>().takedamage(mainCode.mainCode.attackdmg, Vector2.up*20 + mainCode.mainCode.movSen*Vector2.right*40);
    	grabedenemy = null;
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTypes : MonoBehaviour
{
    private ContactPoint2D[] contacts;
    private Tilemap tmap;
    public Tilemap othertmap;
    public float baseSpeed = 2;



    void Start()
    {	
    	baseSpeed = GameObject.FindWithTag("Player").GetComponent<MasterController>().maxspeed;
     	tmap = this.gameObject.GetComponent<Tilemap>();
     	othertmap = othertmap.gameObject.GetComponent<Tilemap>();	
    }

    // Update is called once per frame
    void Update()
    {
    }
   	private void OnCollisionStay2D(Collision2D other)
	{
		if(other.gameObject.tag == "Player")
		{	

			if(othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.GetContact(0).point)) != null)
			{	
				string t_tiled = othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.GetContact(0).point)).texture.name;
				if( t_tiled == "slipper liquid")
				{
					other.gameObject.GetComponent<MasterController>().dragforce = 0.01f;
					if(other.gameObject.GetComponent<MasterController>().dashed)
					{
						other.gameObject.GetComponent<MasterController>().maxspeed = baseSpeed+2f;
					}
					
				}
			}
			else
			{
				other.gameObject.GetComponent<MasterController>().dragforce = 1f;
				other.gameObject.GetComponent<MasterController>().maxspeed = baseSpeed;
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{	
		if(other.gameObject.tag == "Player")
		{	
			if(othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.gameObject.transform.position)) != null)
			{	
				string t_tiled = othertmap.GetSprite(tmap.layoutGrid.WorldToCell(other.gameObject.transform.position)).texture.name;
				if(t_tiled == "Watertop" || t_tiled == "Water")
				{
					other.gameObject.GetComponent<MasterController>().inWater = 1f;
					other.gameObject.GetComponent<MasterController>().gScale = 1f;
				}
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "hitable" && !other.isTrigger)
		{	
			if(other.gameObject.GetComponent<SwimmingTypes>())
			{
				other.gameObject.GetComponent<SwimmingTypes>().SetinWater(true);
			}

		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{	
		if(other.gameObject.tag == "Player")
		{	
			other.gameObject.GetComponent<MasterController>().inWater = 0f;
			other.gameObject.GetComponent<MasterController>().gScale = 2f;

		}
		else if(other.gameObject.tag == "hitable" && !other.isTrigger)
		{	
			if(other.gameObject.GetComponent<SwimmingTypes>())
			{
				other.gameObject.GetComponent<SwimmingTypes>().SetinWater(false);
			}

		}
	}
}

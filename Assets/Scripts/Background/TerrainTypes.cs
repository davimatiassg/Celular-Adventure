using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTypes : MonoBehaviour
{
    private ContactPoint2D[] contacts;
    private Tilemap tmap;
    public Tilemap othertmap;



    void Start()
    {
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
						other.gameObject.GetComponent<MasterController>().maxspeed = 4f;
					}
					
				}

			}
			else
			{
				other.gameObject.GetComponent<MasterController>().dragforce = 1f;
				other.gameObject.GetComponent<MasterController>().maxspeed = 2f;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallerTaeniaShot : MonoBehaviour
{	
	private Transform trs;
	public GameObject Shot;
	[SerializeField] private Transform Player;
	[SerializeField] private BabyTaeniaBehavior maincode;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        trs = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   	public void Attack()
	{
		GameObject shoot = Instantiate(Shot, trs.position, trs.rotation);
		maincode.atk_delay = 4;
		shoot.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, Player.position - trs.position));

		shoot.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
		shoot.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;

		shoot.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 7;
		shoot.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;

		shoot.transform.localScale = new Vector3(12, 12, 0);

		shoot.gameObject.GetComponent<ShootBehavior>().speed = 15;

		shoot.gameObject.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.02f, 0f);
		shoot.gameObject.GetComponent<CapsuleCollider2D>().size = new Vector2(0.05f, 0.06f);


	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{	
	public GameObject Pannel;
    // Start is called before the first frame update

	public void ClosePannel()
	{
		Pannel.GetComponent<Animator>().Play("Pannel out");
	}
	public void fullClose()
	{
		Pannel.SetActive(false);
	}
}

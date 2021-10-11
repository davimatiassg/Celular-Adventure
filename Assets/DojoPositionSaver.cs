using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DojoPositionSaver : MonoBehaviour
{
    void OnEnable()
	{	
        GameEvents.StartListening("LoadingAScene", SavePlayerPosition);

	}
	void OnDisable()
	{
        GameEvents.StopListening("LoadingAScene", SavePlayerPosition);

	}

	public void SavePlayerPosition()
	{
		CheckPointManager.RefreshCheckPoint(1, GameObject.FindWithTag("Player").GetComponent<Transform>().position);
	}
	

}

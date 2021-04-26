using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    // Start is called before the first frame update
	void OnEnable()
	{
		GameEvents.StartListening("BossAreaEntered", ChangeMusic);
	}
	void OnDisable()
	{
		GameEvents.StopListening("BossAreaEntered", ChangeMusic);
	}


	void ChangeMusic()
	{
		AudioInterface a = this.gameObject.GetComponent<AudioInterface>();
		a.PlaySound("boss");
	}

}

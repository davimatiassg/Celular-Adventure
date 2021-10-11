using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScreamer : MonoBehaviour
{
	public void Scream(string EventName)
	{
		GameEvents.ScreamEvent(EventName);
	}
}

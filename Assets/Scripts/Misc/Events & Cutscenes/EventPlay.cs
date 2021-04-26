using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPlay : MonoBehaviour
{
	[SerializeField] private string eventTolisten;

	[SerializeField] private UnityEvent eventToPlay;


	void OnEnable()
	{
		GameEvents.StartListening(eventTolisten, PlayEvent);
	}
	void OnDisable()
	{
		GameEvents.StopListening(eventTolisten, PlayEvent);
	}

	void PlayEvent()
	{	
		if(eventToPlay != null)
		{
			eventToPlay.Invoke();
		}
		
	}
}

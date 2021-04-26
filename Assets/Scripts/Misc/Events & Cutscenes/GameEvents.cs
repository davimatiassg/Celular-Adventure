using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {

	private  Dictionary<string, UnityEvent> eventDict;
	private static GameEvents gameEvents;

	public static GameEvents curnt_instance
	{
		get
		{
			if(!gameEvents)
			{
				gameEvents = FindObjectOfType(typeof(GameEvents)) as GameEvents;
				if(!gameEvents)
				{
					Debug.LogError("You will need to put this script in a active object to it work");
				}
				else
				{
					gameEvents.Initiate();
				}
			}
			return gameEvents;
		}
	}



	void Initiate()
	{
		if(eventDict == null)
		{
			eventDict = new Dictionary<string, UnityEvent>();
		}
	}

	public static void StartListening(string eventName, UnityAction listen)
	{
		UnityEvent thisEvent = null;
		if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listen);
		}
		else
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listen);
			curnt_instance.eventDict.Add(eventName, thisEvent);
		}
	}
	public static void StopListening(string eventName, UnityAction listen)
	{
		if(gameEvents == null)
		{
			return;
		}
		UnityEvent thisEvent = null;
		if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listen);
		}

	}
	public static void ScreamEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		if(curnt_instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}

	/*{
	public static GameEvents current;


	void Awake () {
		current = this;
	}	
	
	public event Action OnBossAreaEnter;

	public void BossAreaEnter()
	{
		if(OnBossAreaEnter != null)
		{
			OnBossAreaEnter();
		}
	}
	public event Action OnBossDie;

	public void BossDie()
	{
		if(OnBossDie != null)
		{
			OnBossDie();
		}	
	}
	public event Action OnBossDead;

	public void BossDead()
	{
		if(OnBossDead != null)
		{
			OnBossDead();
		}	
	}

	//}
	//tudo dentro dessa chave acima faz parte de um sistema antigo que será substituido em breve. Ele controla alguns comandos relacionados à ativação do Boss da fase*/

	
}

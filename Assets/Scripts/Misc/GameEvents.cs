using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {




	public static GameEvents current;
	// Use this for initialization


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
	
}

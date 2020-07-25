using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KillCounter : MonoBehaviour
{
	public static int EnemiesKilled = 0;

	[SerializeField] private Text Countertxt;
    void OnEnable()
	{
		GameEvents.StartListening("EnemyKilled", UpdateScore);
		EnemiesKilled = 0;
	}
	void OnDisable()
	{
		GameEvents.StopListening("EnemyKilled", UpdateScore);
	}
	void UpdateScore()
	{
		EnemiesKilled ++;
	}
	void LateUpdate()
	{
		Countertxt.text = EnemiesKilled.ToString();
	}
}

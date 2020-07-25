using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BestiaryElements : MonoBehaviour
{	
	public Dictionary<string, int> Bestiary = new Dictionary<string, int>();
	public delegate void BestiaryEvents();
	public static event BestiaryEvents onKillEnemy;

	public (List<List<string>>, List<List<Sprite>>) fullpages;

	public List<string> Texts = new List<string>();
	public List<Sprite> Photos = new List<Sprite>(); 
	public List<string> EnemyName = new List<string>();

	public (List<List<string>>, List<List<Sprite>>) completepages;
	

	void OnEnable()
	{
		GameEvents.StartListening("EnemyKilled", UpdateBestiary);
		
	}
	void OnDisable()
	{
		GameEvents.StopListening("EnemyKilled", UpdateBestiary);
	}

	public void UpdateBestiary()
	{
		onKillEnemy ();
		PrintDebugDictionary();
		fullpages = FormateMatr();
	}
	public void PrintDebugDictionary()
	{
		foreach(KeyValuePair<string, int> pages in Bestiary)
		{	
			Debug.Log((pages.Key, pages.Value), this.gameObject);

		}
	}

	
	public (List<List<string>>, List<List<Sprite>>) FormateMatr()
	{
		List<List<string>> textpages = new List<List<string>>();
		List<List<Sprite>> imagepages = new List<List<Sprite>>();
		foreach(KeyValuePair<string, int> pages in Bestiary)
		{	

			for(int enemy = 0; enemy < Texts.Count; enemy ++)
			{	List<string> enemypage = new List<string>();
				List<Sprite> enemyphoto = new List<Sprite>();
				
				if(pages.Key == Texts[enemy])
				{

					enemypage.Add(Texts[enemy+1]);
					enemypage.Add(Texts[enemy+2]);
					enemypage.Add(Texts[enemy+3]);
					enemypage.Add(Texts[enemy+4]);
					enemypage.Add(pages.Value.ToString());
					textpages.Add(enemypage);
					enemyphoto.Add(Photos[enemy*2/5]);
					enemyphoto.Add(Photos[(enemy*2/5)+1]);
					imagepages.Add(enemyphoto);
				}	
			}		
		}
		completepages = (textpages, imagepages);

		return completepages;
	}
}


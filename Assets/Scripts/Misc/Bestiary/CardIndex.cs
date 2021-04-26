using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardIndex
{
	public Sprite RealImage;
	public Sprite Ingame;
	public string LevelName;
	public string IngameBehavior;
	public string RealName;
	public string CInfo;
	public int TimesKilled;
	public int ID;


	public CardIndex(int id, Sprite ri, Sprite ii, string l, string ib, string n, string rb)
	{
		ID = id;

		RealImage = ri;
		Ingame = ii;
		LevelName = l;
		IngameBehavior = ib;
		RealName = n;
		CInfo = rb;
	}

	public CardIndex GetEnemyCardByID(int id)
	{
		if(id == ID)
		{
			return this;
		}
		return null;
	}

	public int GetID()
	{
		return ID;
	}

	public override string ToString()
	{
		return "ID: " + this.ID + ", Name: " + this.RealName + ", Image: " + this.RealImage;
	}
}

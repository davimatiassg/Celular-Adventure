using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Anotation
{
	public Texture pic;
	public int ID;
	public string text;

	public Anotation(int i, Texture p, string t)
	{
		pic = p;
		ID = i;
		text = t;
	}

	public Anotation(CardIndex cd)
	{
		pic = cd.RealImage.texture;
		ID = cd.ID;
		text = cd.CInfo;
	}

	public Anotation GetAnotationById(int id)
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
		return "ID: " + this.ID + ", Texture Name: " + this.pic + ", Description: " + this.text;
	}
}

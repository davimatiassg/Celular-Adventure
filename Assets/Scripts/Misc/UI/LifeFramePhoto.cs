using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeFramePhoto : MonoBehaviour
{
	public List<Sprite> photos;

	[SerializeField] private Image frame;

	void Start()
	{
		ChangeTexture();
	}

	private void ChangeTexture()
	{
		GameObject Player = GameObject.FindWithTag("Player");

		switch(Player.name)
		{
			case "Neutrophil":
				frame.sprite = photos[0];
				break;
			case "Eosinophil":
				frame.sprite = photos[1];
				break;
			case "Linph NK":
				frame.sprite = photos[2];
				break;
			case "Macrophage":
				frame.sprite = photos[3];
				break;

		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{	
	private RectTransform rtrs;

	private bool isVisible = true;
	private Image gaugeImage;

	private void Awake()
	{
		gaugeImage = transform.Find("Gauge").GetComponent<Image>();
	}

	void Start()
	{
		rtrs = GetComponent<RectTransform>();
	}

	public void SetGaugeValue(float life, float maxlife)
	{
		gaugeImage.fillAmount = life/maxlife;
		
	}
	public void ToggleVisibility(bool x)
	{
		isVisible = x;
		gameObject.SetActive(x);

	}	


}

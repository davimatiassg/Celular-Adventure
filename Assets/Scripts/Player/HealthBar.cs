using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{	
	public static GameObject Instance; 
	private RectTransform rtrs;

	public bool isVisible = true;
	private Image gaugeImage;


    void OnEnable()
	{	
		 if(HealthBar.Instance == null)
        {
            HealthBar.Instance = this.gameObject;
        }
        else if(HealthBar.Instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }

        Time.timeScale = 1f;
		GameEvents.StartListening("GameResumed", SetVisible);
		GameEvents.StartListening("GamePaused", SetInvisible);
	}
	void OnDisable()
	{	
		GameEvents.StopListening("GamePaused", SetVisible);
		GameEvents.StopListening("GameResumed", SetInvisible);
		
	}

	private void Awake()
	{
		gaugeImage = transform.Find("Gauge").GetComponent<Image>();
	}

	void Start()
	{
		rtrs = GetComponent<RectTransform>();
	}

	public static void SetGaugeValue(float life, float maxlife)
	{
		Instance.GetComponent<HealthBar>().gaugeImage.fillAmount = life/maxlife;
		
	}
	public void ToggleVisibility(bool x)
	{
		isVisible = x;
		HealthBar.Instance.SetActive(x);
	}
	public static void SetInvisible()
	{	
		Instance.GetComponent<HealthBar>().ToggleVisibility(false);
	}
	public static void SetVisible()
	{	
		Instance.GetComponent<HealthBar>().ToggleVisibility(true);
	}


}

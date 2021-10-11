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

	public static void SetGaugeValue(float delta, float life, float maxlife)
	{
		HealthBar x = Instance.GetComponent<HealthBar>();
		float deltapertick = (delta)/10;
		Debug.Log("st life" + (life - delta));
		Debug.Log("true life" + life);
		Debug.Log("delta p/ tick" + deltapertick);


		x.StartCoroutine(lerpLife(deltapertick, maxlife, 0.02f, 10));
		
	}
	public static IEnumerator lerpLife(float a, float m, float t, int times)
	{

		yield return new WaitForSeconds(t);
		Instance.GetComponent<HealthBar>().gaugeImage.fillAmount += a/m;
		if(times > 0)
		{
			Instance.GetComponent<HealthBar>().StartCoroutine(lerpLife(a, m, t, times-1));
		}
		

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

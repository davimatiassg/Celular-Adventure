using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeSetter : MonoBehaviour
{	

	public string thistag;

	public Slider slider;

	public void OnEnable()
	{
		slider = this.gameObject.GetComponent<Slider>();
		slider.value = AudioController.GetSoundVol(thistag);
	}
    public void SetVolume(float sliderValue)
    {
    	AudioController.SetSoundVol(thistag, sliderValue);
    	GameEvents.ScreamEvent("changevolume");
    	
    }
}

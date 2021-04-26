using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour, AudioInterface
{
    [SerializeField] private AudioSource aud;

    public string thistag;

    [SerializeField]
    public List<AudioClip> clip = new List<AudioClip>();

    public List<string> cname = new List<string>();

    public Dictionary<string, AudioClip> clipstoPlay = new Dictionary<string, AudioClip>();
    
    void OnEnable()
    {
        GameEvents.StartListening("changevolume", getVolume);
		foreach(AudioClip c in clip)
       	{
            if(!clipstoPlay.ContainsKey(cname[clip.IndexOf(c)]))
            {
                clipstoPlay.Add(cname[clip.IndexOf(c)], c);
            }
       		
       	}
    }
    void OnDisable()
    {
        GameEvents.StopListening("changevolume", getVolume);
    }

    void Start()
    {
        PauseMenu.musicController = this;
       	aud = this.gameObject.GetComponent<AudioSource>();
        aud.enabled = true;
       	aud.volume = AudioController.GetSoundVol(thistag);


    }



    // Update is called once per frame
    public void UnPause()
    {
        aud.UnPause();
        getVolume();
    }
    
    public void Pause()
    {
        aud.Pause();
    }


    public void PlaySound(string clipName)
    {	
    	getVolume();
    	aud.Stop();
    	aud.clip = clipstoPlay[clipName];
    	aud.Play();
    }

    public void PlaySound(AudioClip clip)
    {	
    	getVolume();
    	aud.clip = clip;
        if(!clipstoPlay.ContainsKey(clip.name))
        {
            clipstoPlay.Add(clip.name, clip);
        }
    	aud.Play();

    }

    public void getVolume()
    {
        aud.volume = AudioController.GetSoundVol(thistag);
    }
}

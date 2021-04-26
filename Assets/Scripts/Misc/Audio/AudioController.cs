using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{	

	public static AudioController instance;

    public static Dictionary<string, float> soundVolumes = new Dictionary<string, float>(){{"sfx", 1f}, {"music", 1f}};

    void Start()
    {
        if(instance == null)
        {
            instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        }
    }

    public static float GetSoundVol(string tag)
    {
    	float vol = 0f;

    	soundVolumes.TryGetValue(tag, out vol);

        
    	return 0 + vol;
    }

    public static void SetSoundVol(string tag, float vol)
    {
        
    	if(!soundVolumes.ContainsKey(tag))
    	{
    		soundVolumes.Add(tag, vol);
    	}
    	else
    	{
    		soundVolumes[tag] = vol;
    	}

    	
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneChanger : MonoBehaviour
{	
	public List<TimelineAsset> cutscenes;
    // Start is called before the first frame update
    void PlayCutscene(int index)
    {	
    	TimelineAsset sel_Cutscene;
    	if(cutscenes.Count > index)
    	{
    		sel_Cutscene = cutscenes[index];
    		
    	}
    	else
    	{
    		sel_Cutscene = cutscenes[cutscenes.Count -1];
    	}
    	this.gameObject.GetComponent<PlayableDirector>().Play(sel_Cutscene);
    }


}

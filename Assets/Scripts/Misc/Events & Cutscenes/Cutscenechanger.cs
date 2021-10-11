using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class Cutscenechanger : MonoBehaviour
{   
    [SerializeField] private float canceltime;
    void OnEnable()
    {
        GameEvents.StartListening("CheckPointFound", CancelCutscene);
    }
    void OnDisable()
    {
        GameEvents.StopListening("CheckPointFound", CancelCutscene);
    }
    public List<TimelineAsset> cutscenes;
    // Start is called before the first frame update
    public void PlayCutscene(int index)
    {   
        if((Time.time - canceltime) > 1f)
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
            if(sel_Cutscene)
            {
                this.gameObject.GetComponent<PlayableDirector>().playableAsset = sel_Cutscene;
                this.gameObject.GetComponent<PlayableDirector>().Play(sel_Cutscene);  
            }      
        }

        
    }
    
    public void CancelCutscene()
    {
        canceltime = Time.time;
        this.gameObject.GetComponent<PlayableDirector>().Stop();
    }


}

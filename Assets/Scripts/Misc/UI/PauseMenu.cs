using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool isPaused = false;

	public GameObject MenuObject;
	public AudioSource MusicPlayer;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
        	PauseAlternate();
        }
    }

    public void Resume()
    {
    	MenuObject.SetActive(false);
    	Time.timeScale = 1f;
    	isPaused = false;
    	MusicPlayer.UnPause();
    }

    public void Menu()
    {
    	SceneManager.LoadScene("Title");
    	Time.timeScale = 1f;
    }


    public void Pause()
    {	
    	GameEvents.ScreamEvent("GamePaused");
    	MenuObject.SetActive(true);
    	Time.timeScale = 0f;
    	MusicPlayer.Pause();
    	isPaused = true;
    }

    public void PauseAlternate()
    {
    	if(isPaused)
        	{
        		Resume();
        		
        	}
        	else
        	{
        		Pause();
        		
        	}
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CallScene(string cscene)
    {
        SceneManager.LoadScene(cscene);
    }

    public void BestiaryPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Bestiary");
    }
    public void MainPauseMenu()
    {
        MenuObject.GetComponent<Animator>().Play("PMenuFadein");
    }
}

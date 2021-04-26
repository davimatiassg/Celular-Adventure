using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{   
    public static bool isPaused = false;

	public GameObject MenuObject;

	public static MusicPlayer musicController;

    public GameObject GameOverMenu;

    private InputManager InPut;

    public bool canpause = true;


    void OnEnable()
    {
        GameEvents.StartListening("GameOver", OverMenuShow);
        
    }
    void OnDisable()
    {
        GameEvents.StopListening("GameOver", OverMenuShow);
    }
    void Start()
    {
        InPut = InputManager.instance;
    }
    void Update()
    {
        if(InPut.GetButtonDown("pause") && canpause)
        {
        	PauseAlternate();
        }
    }
    public void Restart()
    {   

        MenuObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        PauseMenu.musicController.UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Resume()
    {      
        GameEvents.ScreamEvent("GameResumed");
    	MenuObject.SetActive(false);
    	Time.timeScale = 1f;
    	isPaused = false;
    	PauseMenu.musicController.UnPause();
        
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
        Debug.Log(PauseMenu.musicController);
        if(PauseMenu.musicController != null)
        {
            PauseMenu.musicController.Pause();
        }
    	
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
    public void AnotationPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Anotation");
    }
    public void OptionPlay()
    {
        MenuObject.GetComponent<Animator>().Play("Options");
    }
    public void MainPauseMenu()
    {
        MenuObject.GetComponent<Animator>().Play("PMenuFadein");
    }
    public void OverMenuShow()
    {
        GameOverMenu.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool isPaused = false;

	public GameObject MenuObject;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void Resume()
    {
    	MenuObject.SetActive(false);
    	Time.timeScale = 1f;
    	isPaused = false;
    }

    public void Menu()
    {
    	SceneManager.LoadScene("Title");
    	Time.timeScale = 1f;
    }


    void Pause()
    {
    	MenuObject.SetActive(true);
    	Time.timeScale = 0f;
    	isPaused = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CallScene(string cscene)
    {
        SceneManager.LoadScene(cscene);
    }


}

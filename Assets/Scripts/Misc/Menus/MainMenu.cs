using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : SceneLoader
{	public string scenename;

	public GameObject OtherPannel1;

	public GameObject OtherPannel2;

	public void CallScene(string cscene)
	{
		SceneManager.LoadScene(cscene);
	}

	public void CallOptions()
	{
		OtherPannel1.SetActive(true);
		gameObject.SetActive(false);
	}
	public void CallLvlS()
	{
		OtherPannel2.SetActive(true);

	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Scenecaller()
	{
		CallScene(scenename);
	}
}

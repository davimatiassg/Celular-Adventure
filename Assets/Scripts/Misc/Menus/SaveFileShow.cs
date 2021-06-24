using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SaveFileShow : MonoBehaviour
{
    [SerializeField] private int p = 0;
    [SerializeField] private int l = 0;
    [SerializeField] private float t = 0;

    void Start()
    {
        GetFiles();
        this.gameObject.GetComponent<Text>().text = FormulateString();
    }

    // Update is called once per frame
    public void GetFiles()
    {
    	if(PlayerPrefs.HasKey("level"))
    	{
    		l = PlayerPrefs.GetInt("level");
    	}
    	if(PlayerPrefs.HasKey("score"))
    	{
    		p = PlayerPrefs.GetInt("score");
    	}
    	if(PlayerPrefs.HasKey("playtime"))
    	{
    		t = PlayerPrefs.GetFloat("playtime");
    	} 	
    }
    
    public string FormulateString()
    {
    	string time = (Mathf.RoundToInt(t/3600) + "H, " + (Mathf.RoundToInt(t%3600)/60)) + "m, e " + (Mathf.RoundToInt(t%3600%60)) + "s";
    	string str = p + "\r\n \r\n" + l + "\r\n \r\n" + time + "\r\n";
    	return str;
    }

    public void DeleteAllFiles()
    {
    	p = 0;
    	t = 0;
    	l = 0;
    	this.gameObject.GetComponent<Text>().text = FormulateString();
		PlayerPrefs.SetFloat("playtime", 0);
		PlayerPrefs.SetInt("level", 0);
		PlayerPrefs.SetInt("score", 0);

    }
}

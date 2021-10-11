using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;
    public string assingedLevel;
    public static Dictionary<int, Vector2> Points = new Dictionary<int, Vector2>();


    void Awake()
    {   
    	string Scene = SceneManager.GetActiveScene().name;
    	

        if(instance == null)
        {
            instance = this;
            Object.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
			if(Scene != "")
	    	{
	        	if(instance.assingedLevel == Scene)
	        	{
	        		Destroy(this.gameObject);
	        	}
	        	else
	        	{

    				CheckPointManager.Points.Clear();
	        		Destroy(instance.gameObject);
	        		instance = this;
	            	Object.DontDestroyOnLoad(this.gameObject);
        		}
        	}
            
        }
        else
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }
        

    }
    public static void NewCheckPoint(int n, Vector2 pos)
    {
    	Points.Add(n, pos);
    }

    public static void RefreshCheckPoint(int n, Vector2 pos)
    {
    	if(Points.ContainsKey(n))
    	{
    		Points[n] = pos;
    	}
    	else
    	{
    		NewCheckPoint(n, pos);
    	}

    	
    	
    }

    public static bool GetCheckPoint(int n)
    {
    	return (Points.ContainsKey(n));
    }
   	public static Vector2 GetLastCheckPoint()
    {
    	Vector2 checkpos = Vector2.zero;
    	Points.TryGetValue(Points.Count, out checkpos);
    	return checkpos;
    }
}

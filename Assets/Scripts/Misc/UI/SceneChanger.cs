
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{	 

    public static string nextScene;
	private static GameObject instance;
    [SerializeField] private Animator anim;

    void OnEnable()
	{	
		GameEvents.StartListening("BossDead", FadeOut);
		GameEvents.StartListening("FadeOut", FadeOut);
	}
	void OnDisable()
	{
		GameEvents.StopListening("BossDead", FadeOut);
		GameEvents.StopListening("FadeOut", FadeOut);
	}
    void Start()
    {
        
        if(SceneChanger.instance == null)
        {
            SceneChanger.instance = this.gameObject;
        }
        else if(SceneChanger.instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        Time.timeScale = 1f;
    }



    public void FadeOut()
    {	
        Debug.Log("FadeOut");
    	anim.Play("FadeOut");
    }

    public void LoadScene(string s)
    {	

        if(nextScene != null)
        {	
        	Debug.Log("nextScene");
            SceneManager.LoadScene(nextScene);
            nextScene = null;
        }
        else if(s != null)
        {
        	Debug.Log("s");
            SceneManager.LoadScene(s);
        }
    	
    }

    public static void Load(string s)
    {
        SceneChanger.nextScene = s;
        SceneChanger.instance.GetComponent<SceneChanger>().FadeOut();
    }

}

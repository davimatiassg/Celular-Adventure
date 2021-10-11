
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{	 

    public static string nextScene;
	private static GameObject instance;
    [SerializeField] private Animator anim;

    void OnEnable()
	{	
        GameEvents.StartListening("FinalBossIsDead", CompleteLevel);
		GameEvents.StartListening("FadeOut", FadeOut);
	}
	void OnDisable()
	{
        GameEvents.StopListening("FinalBossIsDead", CompleteLevel);
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
        PlayerPrefs.SetFloat("playtime", Time.timeSinceLevelLoad);
        PlayerPrefs.Save();
    	anim.Play("FadeOut");
    }

    public void LoadScene(string s)
    {	

        if(nextScene != null)
        {	
            GameEvents.ScreamEvent("LoadingAScene");
            SceneManager.LoadScene(nextScene);
            nextScene = null;
        }
        else if(s != null)
        {
            GameEvents.ScreamEvent("LoadingAScene");

            SceneManager.LoadScene(s);
        }
    	
    }

    public void CompleteLevel()
    {
        int i = PlayerPrefs.GetInt("level");
        PlayerPrefs.SetInt("level", i+1);
        PlayerPrefs.Save();
        GameEvents.ScreamEvent("FadeOut");

    }
    public static void Load(string s)
    {
        GameEvents.ScreamEvent("LoadingAScene");
        SceneChanger.nextScene = s;
        SceneChanger.instance.GetComponent<SceneChanger>().FadeOut();
    }

}

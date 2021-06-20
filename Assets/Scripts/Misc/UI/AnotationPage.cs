using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnotationPage : MonoBehaviour
{
	[SerializeField] private Text Info;//Informações Científicas
	[SerializeField] private Image Photo;//Sprite In-game
	[SerializeField] private int Pagenum = 0; //número da página
	[SerializeField] public List<Anotation> Notes;

	void Awake()
	{
		Notes = AnotationManager.Notes;
	}

    void setPage()
    {	
        RectTransform r = Photo.gameObject.GetComponent<RectTransform>();
        Rect s = Rect.zero;
        Texture2D t = (Texture2D) Notes[Pagenum].pic;
        s.size = Vector2.one*t.width;
        Debug.Log(s.ToString());
    	Info.text = Notes[Pagenum].text;
    	Photo.sprite = Sprite.Create(t, s, Vector2.one/2, 64, 5);

    }

    public void setPagenum(int n)
    {	
    	if(Notes != AnotationManager.Notes)
    	{
    		Notes = AnotationManager.Notes;
    	}
    	if(AnotationManager.Notes.Count >= n && n >= 0)
    	{
    		Pagenum = n;
    		setPage();
    	}
    	
    }
    public void nextPage(int sentido)
    {	 
    	if(Pagenum + sentido <= Notes.Count -1  && (Pagenum + sentido >= 0))
    	{
    		Pagenum += sentido;
    		setPage();
    	}

    }
}

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
    	Info.text = Notes[Pagenum].text;
    	Photo.sprite = Sprite.Create((Texture2D)Notes[Pagenum].pic, new Rect(0.0f, 0.0f, Notes[Pagenum].pic.width, Notes[Pagenum].pic.height), Vector2.one/2);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryPage : MonoBehaviour
{
	[SerializeField] private Text name;//Nome científico
	[SerializeField] private Text C_info;//Informações Científicas
	[SerializeField] private Text levelmeet;//Level em que aparece
	[SerializeField] private Text G_info;//Comportamentos In-game
	[SerializeField] private Text TimesKilled;//vezes que foi abatido pelo jogador
	[SerializeField] private Image C_image;//Pixelart da imagem em microscópio do bixo
	[SerializeField] private Image G_image;//Sprite In-game

	[SerializeField] private (List<List<string>>, List<List<Sprite>>) allpages;
	[SerializeField] private int Pagenum = 0; //número da página
	[SerializeField] public BestiaryElements Bestiary;

	void Awake()
	{
		Bestiary = GameObject.FindWithTag("Bestiary").GetComponent<BestiaryElements>();
		allpages = Bestiary.fullpages;

	}

    void setPage()
    {
    	allpages = Bestiary.fullpages;

    	
    	name.text = allpages.Item1[Pagenum][0];
		C_info.text = allpages.Item1[Pagenum][1];
    	levelmeet.text = allpages.Item1[Pagenum][2];
    	G_info.text = allpages.Item1[Pagenum][3];
    	C_image.sprite = allpages.Item2[Pagenum][0];
    	G_image.sprite = allpages.Item2[Pagenum][1];
    }

    public void setPagenum(int n)
    {	
    	if(Bestiary.fullpages.Item1[Pagenum].Count >= n && n >= 0)
    	{
    		Pagenum = n;
    		setPage();
    	}
    	
    }
    public void nextPage(int sentido)
    {	 
        Debug.Log(Bestiary.fullpages.Item1.Count);
    	if(Pagenum + sentido <= Bestiary.fullpages.Item1.Count -1  && (Pagenum + sentido >= 0))
    	{
    		Pagenum += sentido;
    		setPage();
    	}

    }
}

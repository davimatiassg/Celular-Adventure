using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryPage : MonoBehaviour
{
    [SerializeField] private Text tname;//Nome científico
    [SerializeField] private Text C_info;//Informações Científicas
    [SerializeField] private Text levelmeet;//Level em que aparece
    [SerializeField] private Text G_info;//Comportamentos In-game
    [SerializeField] private Text TimesKilled;//vezes que foi abatido pelo jogador
    [SerializeField] private Image C_image;//Pixelart da imagem em microscópio do bixo
    [SerializeField] private Image G_image;//Sprite In-game
    [SerializeField] private int Pagenum = 0; //número da página
    [SerializeField] public List<CardIndex> Bestiary = new List<CardIndex>();

    void Awake()
    {
        Bestiary = BestiaryElements.Bestiary;
    }

    void setPage()
    {
        tname.text = BestiaryElements.Bestiary[Pagenum].RealName;
        C_info.text = BestiaryElements.Bestiary[Pagenum].CInfo;
        levelmeet.text = BestiaryElements.Bestiary[Pagenum].LevelName;
        G_info.text = BestiaryElements.Bestiary[Pagenum].IngameBehavior;
        C_image.sprite = BestiaryElements.Bestiary[Pagenum].RealImage;
        G_image.sprite = BestiaryElements.Bestiary[Pagenum].Ingame;
    }

    public void setPagenum(int n)
    {      
        
        if(BestiaryElements.Bestiary.Count > n && n >= 0)
        {
            Pagenum = n;
            setPage();
        }
        
    }
    public void nextPage(int sentido)
    {    
        if(Pagenum + sentido <= BestiaryElements.Bestiary.Count -1  && (Pagenum + sentido >= 0))
        {
            Pagenum += sentido;
            setPage();
        }

    }
}

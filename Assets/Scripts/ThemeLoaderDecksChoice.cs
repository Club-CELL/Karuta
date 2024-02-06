using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class ThemeLoaderDecksChoice : MonoBehaviour
{
    public Theme theme;


    public Sprite arrowBW;
    public Sprite backArrowBW;

    public Image currentPlayerPanel;
    public Text currentPlayerText;

    public Image backArrow;
    public Image arrow;
    public Text arrowText;

    public GameObject arrowParent;

    public void LoadTheme()
    {
        if (theme != null)
        {
            currentPlayerText.color = GetColorFromString(theme.colorTextCurrentPlayer, currentPlayerText.color);
            currentPlayerPanel.color = GetColorFromString(theme.colorPanelCurrentPlayer, currentPlayerPanel.color);

            backArrow.sprite = backArrowBW;
            backArrow.color = GetColorFromString(theme.colorBackArrow, backArrow.color);

            arrow.sprite = arrowBW;
            arrow.color = GetColorFromString(theme.colorDeckArrow, arrow.color);
            Text arrowText = arrow.GetComponentInChildren<Text>();
            arrowText.color = GetColorFromString(theme.colorDeckArrowText, arrowText.color);



            foreach (ChoixDecks choixDeck in arrowParent.GetComponentsInChildren<ChoixDecks>())
            {
                var image = choixDeck.GetComponent<Image>();
                image.sprite = arrowBW;
                image.color = GetColorFromString(theme.colorDeckArrow, image.color);
                Text deckArrowText = image.GetComponentInChildren<Text>();
                deckArrowText.color = GetColorFromString(theme.colorDeckArrowText, arrowText.color);
            }

            //foreach (Image deckArrow in arrowParent.GetComponentsInChildren<Image>())
            //{
            //    deckArrow.sprite = arrowBW;
            //    deckArrow.color = GetColorFromString(theme.colorDeckArrow, deckArrow.color);
            //    Text deckArrowText = deckArrow.GetComponentInChildren<Text>();
            //    deckArrowText.color = GetColorFromString(theme.colorDeckArrowText, arrowText.color);
            //}

            Camera.main.backgroundColor = GetColorFromString(theme.backgroundColorDecksChoice, Camera.main.backgroundColor);

            //string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Themes"), theme.decksChoiceBackground);

            string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Packs", theme.packId ?? "", "Themes"), theme.decksChoiceBackground);
            BackgroundHandler.UseAsBackground(path);
            if (!string.IsNullOrEmpty(theme.decksChoiceBackground) && File.Exists(theme.decksChoiceBackground))
            {
            }
        }
        

    }


    Color GetColorFromString(string s, Color defaultColor)
    {
        Color color = defaultColor;
        if(ColorUtility.TryParseHtmlString(s, out color))
        {
            return color;
        }
        return defaultColor;

    }


    void Awake()
    {
        //SerializeTheme();
        GetTheme();
    }
    void Start()
    {
        LoadTheme();
    }
    public void SerializeTheme()
    {
        Theme newTheme = new Theme();
        JsonSerialization.WriteToJsonResource<Theme>("theme", newTheme);
    }
    public void GetTheme()
    {
        /*
        theme = JsonSerialization.ReadFromJsonResource<Theme>("themeSakura");
        

        theme.Check();*/
        theme = Global.theme;
    }

    
}



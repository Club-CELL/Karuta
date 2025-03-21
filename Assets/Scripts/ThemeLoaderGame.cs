using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class ThemeLoaderGame : MonoBehaviour
{

    public Theme theme;

    public Sprite backArrowBW;
    public Sprite playButtonBW;
    public Sprite pauseButtonBW;

    public Image backArrow;

    public Image arrowFound;
    public Text arrowFoundText;

    public Image arrowNotFound;
    public Text arrowNotFoundText;

    public Image playButton;

    public PlayPause playPause;

    public Text cardsLeft;
    public Text animeTitle;

    public void LoadTheme()
    {
        if (theme != null)
        {
            backArrow.sprite = backArrowBW;
            backArrow.color = GetColorFromString(theme.colorBackArrow, backArrow.color);

            arrowFound.color = GetColorFromString(theme.colorCardFoundArrow, arrowFound.color);
            arrowNotFound.color = GetColorFromString(theme.colorCardNotFoundArrow, arrowNotFound.color);

            arrowFoundText.color = GetColorFromString(theme.colorCardFoundText, arrowFoundText.color);
            arrowNotFoundText.color = GetColorFromString(theme.colorCardNotFoundText, arrowNotFoundText.color);

            cardsLeft.color = GetColorFromString(theme.colorNumberOfCardsLeft, cardsLeft.color);
            animeTitle.color = GetColorFromString(theme.colorAnimeTitle, animeTitle.color);

            playButton.sprite = playButtonBW;
            playButton.color = GetColorFromString(theme.colorPlayPause, playButton.color);

            playPause.play = playButtonBW;
            playPause.pause = pauseButtonBW;

            Camera.main.backgroundColor = GetColorFromString(theme.backgroundColorGame, Camera.main.backgroundColor);

            //string path = Path.Combine(Path.Combine(PathManager.MainPath, "Themes"), theme.gameBackground);

            string path = Path.Combine(Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes"), theme.gameBackground);
            BackgroundHandler.UseAsBackground(path);
            if (!string.IsNullOrEmpty(theme.gameBackground) && File.Exists(theme.gameBackground))
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
        //theme = JsonSerialization.ReadFromJsonResource<Theme>("themeSakura");
        //theme.Check()
        theme = Global.theme;
    }

}



using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ThemeLoaderGame : MonoBehaviour
{
    [Header("UI Elements")]
    public Image backButton;
    public Image foundCard;
    public Image notFoundCard;
    public Image playButton;
    public PlayPause playPause;
    public List<Text> texts;

    private Theme theme;

    void Awake()
    {
        GetTheme();
    }

    void Start()
    {
        LoadTheme();
    }

    void GetTheme()
    {
        theme = Global.theme;
    }

    void LoadTheme()
    {
        if (theme != null)
        {
            foreach(Text text in texts)
            {
                text.color = GetColorFromString(theme.mainTextColor, text.color);
            }

            playButton.color = GetColorFromString(theme.buttonsColor, playButton.color);
            backButton.color = GetColorFromString(theme.buttonsColor, backButton.color);

            foundCard.color = GetColorFromString(theme.cardFoundColor, foundCard.color);
            notFoundCard.color = GetColorFromString(theme.cardNotFoundColor, notFoundCard.color);

            Text imageText = foundCard.GetComponentInChildren<Text>();
            imageText.color = GetColorFromString(theme.mainTextColor, imageText.color);
            imageText = notFoundCard.GetComponentInChildren<Text>();
            imageText.color = GetColorFromString(theme.mainTextColor, imageText.color);

            HandlerEX gameHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<HandlerEX>();
            gameHandler.SetButtonColors();

            Camera.main.backgroundColor = GetColorFromString(theme.gameBackgroundColor, Camera.main.backgroundColor);

            string path = Path.Combine(Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes"), theme.gameBackground);
            BackgroundHandler.UseAsBackground(path);
        }
        
    }

    Color GetColorFromString(string s, Color defaultColor)
    {
        if (ColorUtility.TryParseHtmlString(s, out Color color))
        {
            return color;
        }
        return defaultColor;
    }
}



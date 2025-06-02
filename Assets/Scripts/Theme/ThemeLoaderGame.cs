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
    public Text startDelay;

    public void LoadTheme()
    {
        if (theme != null)
        {
            backArrow.sprite = backArrowBW;
            backArrow.color = GetColorFromString(theme.buttonsColor, backArrow.color);

            arrowFound.color = GetColorFromString(theme.cardFoundColor, arrowFound.color);
            arrowNotFound.color = GetColorFromString(theme.cardNotFoundColor, arrowNotFound.color);

            arrowFoundText.color = GetColorFromString(theme.mainTextColor, arrowFoundText.color);
            arrowNotFoundText.color = GetColorFromString(theme.mainTextColor, arrowNotFoundText.color);

            cardsLeft.color = GetColorFromString(theme.mainTextColor, cardsLeft.color);
            animeTitle.color = GetColorFromString(theme.mainTextColor, animeTitle.color);
            startDelay.color = GetColorFromString(theme.mainTextColor, startDelay.color);

            playButton.sprite = playButtonBW;
            playButton.color = GetColorFromString(theme.buttonsColor, playButton.color);

            playPause.play = playButtonBW;
            playPause.pause = pauseButtonBW;

            Camera.main.backgroundColor = GetColorFromString(theme.gameBackgroundColor, Camera.main.backgroundColor);

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
        if (ColorUtility.TryParseHtmlString(s, out Color color))
        {
            return color;
        }
        return defaultColor;
    }

    void Awake()
    {
        GetTheme();
    }

    void Start()
    {
        LoadTheme();
    }

    public void SerializeTheme()
    {
        Theme newTheme = new();
        JsonSerialization.WriteToJsonResource<Theme>("theme", newTheme);
    }

    public void GetTheme()
    {
        theme = Global.theme;
    }
}



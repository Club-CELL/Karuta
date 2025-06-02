using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ThemeLoaderDecksChoice : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image currentPlayerPanel;
    [SerializeField] private Text currentPlayerText;
    [SerializeField] private Image returnButton;

    private Theme theme;

    void Awake()
    {
        GetTheme();
    }

    void Start()
    {
        LoadTheme();
    }

    public void GetTheme()
    {
        theme = Global.theme;
    }

    public void LoadTheme()
    {
        if (theme != null)
        {
            currentPlayerText.color = GetColorFromString(theme.mainTextColor, currentPlayerText.color);
            currentPlayerPanel.color = GetColorFromString(theme.panelsColor, currentPlayerPanel.color);
            returnButton.color = GetColorFromString(theme.buttonsColor, returnButton.color);

            Camera.main.backgroundColor = GetColorFromString(theme.deckChoiceBackgroundColor, Camera.main.backgroundColor);

            string packPath = Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes");
            string path = Path.Combine(packPath, theme.decksChoiceBackground);
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
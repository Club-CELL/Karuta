using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ThemeLoaderDecksChoice : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image currentPlayerPanel;
    [SerializeField] private Text currentPlayerText;
    [SerializeField] private Image returnButton;
    [SerializeField] private StartTrialButton startButton;

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
            currentPlayerText.color = GetColorFromString(theme.mainTextColor, currentPlayerText.color);
            currentPlayerPanel.color = GetColorFromString(theme.panelsColor, currentPlayerPanel.color);
            returnButton.color = GetColorFromString(theme.buttonsColor, returnButton.color);
            if (startButton != null) startButton.SetActiveColor(GetColorFromString(theme.buttonsColor, Color.white));

            Camera.main.backgroundColor = GetColorFromString(theme.deckChoiceBackgroundColor, Camera.main.backgroundColor);

            if (theme.decksChoiceBackground != null)
            {
                string packPath = Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes");
                string path = Path.Combine(packPath, theme.decksChoiceBackground);
                BackgroundHandler.UseAsBackground(path);
            }
        }
    }

    public void SetButtonColor(Image button)
    {
        button.color = GetColorFromString(theme.buttonsColor, button.color);
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = GetColorFromString(theme.mainTextColor, buttonText.color);

        if (button.TryGetComponent<DeckButton>(out var deck))
        {
            deck.SetColors(GetColorFromString(theme.secondaryColor, Color.white));
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
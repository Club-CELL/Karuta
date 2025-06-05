using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeLoaderTrial : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<Text> texts;
    [SerializeField] private List<Image> buttons;
    [SerializeField] private Image foundCard;
    [SerializeField] private Image notFoundCard;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private ScrollSnapRect cardsPanel;

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
            foreach (var text in texts)
            {
                text.color = GetColorFromString(theme.mainTextColor, text.color);
            }

            foreach (Image button in buttons)
            {
                button.color = GetColorFromString(theme.buttonsColor, button.color);
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null) buttonText.color = GetColorFromString(theme.mainTextColor, buttonText.color);
            }

            SetSliderColor(musicSlider, "Background", theme.sliderBackgroundColor);
            SetSliderColor(musicSlider, "Fill Area/Fill", theme.sliderFillColor);
            SetSliderColor(musicSlider, "Handle Slide Area/Handle", theme.sliderHandleColor);

            foundCard.color = GetColorFromString(theme.cardFoundColor, foundCard.color);
            notFoundCard.color = GetColorFromString(theme.cardNotFoundColor, notFoundCard.color);

            Text imageText = foundCard.GetComponentInChildren<Text>();
            imageText.color = GetColorFromString(theme.mainTextColor, imageText.color);
            imageText = notFoundCard.GetComponentInChildren<Text>();
            imageText.color = GetColorFromString(theme.mainTextColor, imageText.color);

            cardsPanel.selectedColor = GetColorFromString(theme.buttonsColor, cardsPanel.selectedColor);
            cardsPanel.unselectedColor = GetColorFromString(theme.buttonInactiveColor, cardsPanel.unselectedColor);

            Camera.main.backgroundColor = GetColorFromString(theme.gameBackgroundColor, Camera.main.backgroundColor);

            if (theme.gameBackground != null)
            {
                string path = Path.Combine(Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes"), theme.gameBackground);
                BackgroundHandler.UseAsBackground(path);
            }

        }

    }

    private void SetSliderColor(Slider slider, string part, string color)
    {
        Transform t = slider.transform.Find(part);
        if (t != null)
        {
            if (t.TryGetComponent<Image>(out var image))
            {
                image.color = GetColorFromString(color, image.color);
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
}

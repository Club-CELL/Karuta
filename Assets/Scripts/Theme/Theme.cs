using UnityEngine;

[System.Serializable]
public class Theme
{
    public string packId;
    string name;

    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetName()
    {
        return name;
    }

    public string mainColor;
    public string mainTextColor;
    public string secondaryColor;
    public string secondaryTextColor;

    public string mainMenuBackground;
    public string decksChoiceBackground;
    public string gameBackground;

    public string mainMenuBackgroundColor;
    public string deckChoiceBackgroundColor;
    public string gameBackgroundColor;

    public string panelsColor;
    public string panelBorderColor;
    public float panelAlpha = 1f;

    public string buttonsColor;
    public string buttonInactiveColor;
    public string checkBoxColor;
    public string textOutlineColor;
    public string sliderHandleColor;
    public string sliderFillColor;
    public string sliderBackgroundColor;

    public string cardFoundColor;
    public string cardNotFoundColor;

    public Theme(string name, Color main, Color text, Color second, Color secText)
    {
        this.name = name;
        mainColor = "#" + ColorUtility.ToHtmlStringRGB(main);
        secondaryColor = "#" + ColorUtility.ToHtmlStringRGB(second);
        mainTextColor = "#" + ColorUtility.ToHtmlStringRGB(text);
        secondaryTextColor = "#" + ColorUtility.ToHtmlStringRGB(secText);
    }

    public void Check()
    {
        CheckValue(ref mainTextColor, "white");
        CheckValue(ref secondaryTextColor, mainColor);

        CheckValue(ref mainMenuBackgroundColor, mainColor);
        CheckValue(ref deckChoiceBackgroundColor, mainColor);
        CheckValue(ref gameBackgroundColor, mainColor);

        CheckValue(ref mainMenuBackgroundColor, "black");
        CheckValue(ref deckChoiceBackgroundColor, "black");
        CheckValue(ref gameBackgroundColor, "black");

        CheckValue(ref panelsColor, secondaryColor, panelAlpha);
        CheckValue(ref panelBorderColor, mainColor, panelAlpha);

        CheckValue(ref buttonsColor, mainColor);
        CheckValue(ref checkBoxColor, mainColor);
        CheckValue(ref textOutlineColor, mainColor);
        CheckValue(ref sliderHandleColor, mainColor);
        CheckValue(ref sliderFillColor, mainColor);
        CheckValue(ref sliderBackgroundColor, "white");
        CheckValue(ref buttonInactiveColor, "white");

        CheckValue(ref cardFoundColor, "green");
        CheckValue(ref cardNotFoundColor, "red");
    }

    private void CheckValue(ref string s, string defaultString, float alpha = 1)
    {
        if (!ColorUtility.TryParseHtmlString(s, out _))
        {
            ColorUtility.TryParseHtmlString(defaultString, out Color newColor);
            newColor.a = alpha;
            s = "#" + ColorUtility.ToHtmlStringRGBA(newColor);
        }
    }

}

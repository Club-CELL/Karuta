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

    public string buttonsColor;
    public string buttonInactiveColor;
    public string checkBoxColor;
    public string panelsColor;
    public string panelBorderColor;
    public string textOutlineColor;
    public string sliderHandleColor;
    public string sliderBackgroundColor;
    public string sliderFillColor;

    public string cardFoundColor;
    public string cardNotFoundColor;

    public float panelAlpha = 0.5f;

    public void Check()
    {
        DefaultToColorString(ref secondaryTextColor, mainColor);
        DefaultToColorString(ref buttonsColor, mainColor);
        DefaultToColorString(ref checkBoxColor, mainColor);
        DefaultToColorString(ref panelBorderColor, mainColor);
        DefaultToColorString(ref textOutlineColor, mainColor);

        DefaultToColorString(ref sliderHandleColor, mainColor);
        DefaultToColorString(ref sliderFillColor, mainColor);
        DefaultToColorString(ref cardFoundColor, mainColor);
        DefaultToColorString(ref cardNotFoundColor, mainColor);

        DefaultToColorString(ref panelsColor, secondaryColor, panelAlpha);
        DefaultToColorString(ref sliderBackgroundColor, "white");
        DefaultToColorString(ref buttonInactiveColor, "white");
    }

    public void DefaultToColorString(ref string s, string defaultString, float alpha = 1)
    {
        if (!ColorUtility.TryParseHtmlString(s, out _))
        {
            s = defaultString;
            ColorUtility.TryParseHtmlString(s, out Color newColor);
            newColor.a = alpha;
            s = "#" + ColorUtility.ToHtmlStringRGBA(newColor);
        }
    }

}

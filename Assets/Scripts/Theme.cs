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

    public string backgroundColorMainMenu;
    public string backgroundColorDecksChoice;
    public string backgroundColorGame;

    public string mainMenuBackground;
    public string decksChoiceBackground;
    public string gameBackground;




    

    public string colorOptionButton;//
    public string colorOptionButtonText;//
    public string colorQuitButton;//
    public string colorPlayPause;//
    public string colorCheckBox;//

    public string colorTextNumberPlayersQuestion;//
    public string colorPanelNumberPlayersQuestion;//
    public string colorPanelNumberPlayers;//
    public string colorCentralPanelNumberPlayers;//
    public string colorTextNumberPlayers;//




    public string colorOptionPanel;//
    public string colorOptionPanelBorder;//
    public string colorOptionsText;//
    public string colorOptionsTextOutline;//
    public string colorSliderHandle;//
    public string colorSliderBackground;
    public string colorSliderFill;//
    public string colorUpdateButton;//
    public string colorUpdateButtonText;//

    //<Packs
    public string colorPackBannerBackground;//
    public string colorPackBannerText;//
    public string colorPackButton;//
    public string colorPackButtonText;//
    public string colorAddPackButton;
    public string colorAddPackButtonText;//
    public string colorNewPackButton;
    public string colorNewPackButtonText;
    public string colorPacksTitleText;
    public string colorPacksTitleBackground;
    public string colorPackBackButton;
    public string colorPackBackButtonText;

    public string colorNewPackFolderInputBackground;
    public string colorNewPackFolderInputText;
    public string colorNewPackFolderInputPlaceholderText;
    public string colorNewPackFolderCreateInstructionsLink;
    public string colorNewPackFolderAddInstructionsText;

    //Packs>


    public string colorContinueArrow;//
    public string colorBackArrow;//
    public string colorPanelCurrentPlayer;//
    public string colorTextCurrentPlayer;//
    public string colorDeckArrow;//
    public string colorDeckArrowText;//

    



    public string colorThemesButton;
    public string colorThemesQuitButton;
    public string colorThemesPanel;
    public string colorThemesTitleText;
    public string colorThemeSelectedIndicator;
    public string colorThemeUnselectedIndicator;

    //GAME
    public string colorNumberOfCardsLeft;//
    public string colorAnimeTitle;//

    public string colorCardFoundArrow;//
    public string colorCardFoundText;//

    public string colorCardNotFoundArrow;//
    public string colorCardNotFoundText;//



    float panelAlpha = 0.5f;
    public Theme()
    {
    }

    public void Check()
    {
        //Text
        DefaultToColorString(ref colorOptionButtonText, mainTextColor);
        DefaultToColorString(ref colorTextNumberPlayers, mainTextColor);
        DefaultToColorString(ref colorOptionsText, mainTextColor);
        DefaultToColorString(ref colorOptionsTextOutline, mainTextColor);
        DefaultToColorString(ref colorTextCurrentPlayer, mainTextColor);
        DefaultToColorString(ref colorDeckArrowText, mainTextColor);
        DefaultToColorString(ref colorCardFoundText, mainTextColor);
        DefaultToColorString(ref colorCardNotFoundText, mainTextColor);
        DefaultToColorString(ref colorTextNumberPlayersQuestion, mainTextColor);
        DefaultToColorString(ref colorUpdateButtonText, mainTextColor);
        DefaultToColorString(ref colorNumberOfCardsLeft, mainTextColor);
        DefaultToColorString(ref colorAnimeTitle, mainTextColor);
        DefaultToColorString(ref colorPackBannerText, mainTextColor);
        DefaultToColorString(ref colorPackButtonText, mainTextColor);
        DefaultToColorString(ref colorAddPackButtonText, mainTextColor);
        DefaultToColorString(ref colorNewPackButtonText, mainTextColor);
        DefaultToColorString(ref colorPacksTitleText, mainTextColor);
        DefaultToColorString(ref colorPackBackButtonText, mainTextColor);


        DefaultToColorString(ref colorNewPackFolderInputText, mainTextColor);
        DefaultToColorString(ref colorNewPackFolderInputPlaceholderText, mainTextColor);
        DefaultToColorString(ref colorNewPackFolderCreateInstructionsLink, mainTextColor);
        DefaultToColorString(ref colorNewPackFolderAddInstructionsText, mainTextColor);





    //Main Color
    DefaultToColorString(ref colorOptionButton, mainColor);
        DefaultToColorString(ref colorQuitButton, mainColor);
        DefaultToColorString(ref colorPlayPause, mainColor);
        DefaultToColorString(ref colorPanelNumberPlayersQuestion, mainColor, panelAlpha);
        DefaultToColorString(ref colorOptionPanelBorder, mainColor);
        DefaultToColorString(ref colorSliderHandle, mainColor);
        DefaultToColorString(ref colorUpdateButton, mainColor);
        DefaultToColorString(ref colorContinueArrow, mainColor);
        DefaultToColorString(ref colorBackArrow, mainColor);
        DefaultToColorString(ref colorPanelCurrentPlayer, mainColor, panelAlpha);
        DefaultToColorString(ref colorDeckArrow, mainColor);
        DefaultToColorString(ref colorCardFoundArrow, mainColor);
        DefaultToColorString(ref colorCardNotFoundArrow, mainColor);
        DefaultToColorString(ref colorSliderFill, mainColor);
        DefaultToColorString(ref colorCheckBox, mainColor);

        DefaultToColorString(ref colorThemeSelectedIndicator, mainColor);
        DefaultToColorString(ref colorThemesTitleText, mainColor);
        DefaultToColorString(ref colorThemesQuitButton, mainColor);
        DefaultToColorString(ref colorThemesButton, mainColor);

        DefaultToColorString(ref colorPackButton, mainColor);
        DefaultToColorString(ref colorAddPackButton, mainColor);
        DefaultToColorString(ref colorPackBannerBackground, mainColor);
        DefaultToColorString(ref colorNewPackButton, mainColor);
        DefaultToColorString(ref colorPackBackButton, mainColor);
        DefaultToColorString(ref colorNewPackFolderInputBackground, mainColor);





    //SecondaryColor
    DefaultToColorString(ref colorCentralPanelNumberPlayers, secondaryColor, panelAlpha);
        DefaultToColorString(ref colorOptionPanel, secondaryColor);
        DefaultToColorString(ref colorThemesPanel, secondaryColor, 0.9f);
        

        DefaultToColorString(ref colorSliderBackground, "white", 0.5f);
        DefaultToColorString(ref colorThemeUnselectedIndicator, "white");
        DefaultToColorString(ref colorPanelNumberPlayers, "black", panelAlpha);
        DefaultToColorString(ref colorPacksTitleBackground, "black", panelAlpha);



}

    public void DefaultToColorString(ref string s, string defaultString, float alpha=1)
    {
        Color color;
        if(!ColorUtility.TryParseHtmlString(s,out color))
        {
            s = defaultString;

            Color newColor;
            ColorUtility.TryParseHtmlString(s, out newColor);
            newColor.a = alpha;
            s = "#" + ColorUtility.ToHtmlStringRGBA(newColor);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class ThemeLoaderMainMenu : MonoBehaviour
{
    public Theme theme;

    [Header("Black&White sprites")]
    public Sprite checkBoxBW;
    public Sprite checkBoxOnBW;
    public Sprite quitButtonBW;
    public Sprite optionButtonBW;
    public Sprite playButtonBW;
    public Sprite pauseButtonBW;
    public Sprite continueArrowBW;

    [Header("Base sprites")]
    public Sprite checkBoxColor;
    public Sprite checkBoxOnColor;
    public Sprite quitButtonColor;
    public Sprite optionButtonColor;
    public Sprite playButtonColor;
    public Sprite pauseButtonColor;
    public Sprite continueArrowColor;

    [Header("Objects to change")]
    public List<CheckBox> checkboxes;
    public Image quitButton;

    public Image optionsButton;
    public Text optionsButtonText;
    public Image updateButton;
    public Text updateButtonText;

    public Slider sensitivitySlider;
    public Image sensitivitySliderHandle;
    public Image sensitivitySliderBackground;
    public Image sensitivitySliderFill;

    public Slider startdelaySlider;
    public Image startdelaySliderHandle;
    public Image startdelaySliderBackground;
    public Image startdelaySliderFill;

    public Image optionsMenuBorder;
    public Image optionsMenuPanel;

    public List<Text> playerNumbers;
    public Text playerNumberQuestion;

    public Image playerNumbersPanel;
    public Image playerNumberQuestionPanel;
    public Image playerNumberSelectedPanel;

    public List<Text> optionsText;

    public Image continueArrow;

    public Image themesButton;
    public Image themesQuitButton;
    public Image themesPanel;
    public Text themesTextImage;
    public Text themesTextVideo;

    public Image packButton;
    public Image addPackButton;
    public Text packButtonText;
    public Text addPackButtonText;
    public Image newPackButton;
    public Text newPackButtonText;
    public Image packsTitleBackground;
    public Text packTitleText;
    public Image packBackButton;
    public Text packBackButtonText;

    public Image newPackFolderInputBackground;
    public TMP_Text newPackFolderInputText;
    public TMP_Text newPackFolderInputPlaceholderText;
    public TMP_Text newPackFolderCreateInstructionsLink;
    public TMP_Text newPackFolderAddInstructionsText;
    
    List<Theme> themes = new();

    [Header("Theme selection")]
    public GameObject imageTheme;
    public GameObject videoTheme;
    public GameObject indicator;
    public GameObject contentHolder;

    public ScrollSnapRect themesScroll;

    //Colors
    Color classicMainColor;
    Color classicOptionPanelColor;
    Color classicOptionBorderColor;
    ColorBlock classicSliderColorblock;
    Color classicSelectedPanelColor;
    Color classicSliderHandleColor;
    Color classicSliderFillColor;
    Color classicSliderBackgroundColor;
    Color classicColorThemePanel;
    Color classicColorSelectedTheme;
    Color newPackFolderInputBackgroundColor;
    Color newPackFolderInputPlaceholderTextColor;
    Color newPackFolderCreateInstructionsLinkColor;

    string currentThemePackId;
    string currentTheme;

    public void LoadTheme()
    {
        if(theme != null)
        {

            foreach(CheckBox checkbox in checkboxes)
            {
                Image checkBoxImage = checkbox.GetComponent<Image>();
                if (checkbox.State)
                {
                    checkBoxImage.sprite = checkBoxOnBW;
                }
                else
                {
                    checkBoxImage.sprite = checkBoxBW;
                }
                checkbox.boxOff = checkBoxBW;
                checkbox.boxOn = checkBoxOnBW;
                checkBoxImage.color = GetColorFromString(theme.colorCheckBox, checkBoxImage.color);
            }
            quitButton.sprite = quitButtonBW;
            quitButton.color = GetColorFromString(theme.colorQuitButton, quitButton.color);

            optionsButton.sprite = optionButtonBW;
            optionsButton.color = GetColorFromString(theme.colorOptionButton, optionsButton.color);

            optionsButtonText.color = GetColorFromString(theme.colorOptionButtonText, optionsButtonText.color);


            updateButton.sprite = optionButtonBW;
            updateButton.color = GetColorFromString(theme.colorUpdateButton, updateButton.color);

            updateButtonText.color = GetColorFromString(theme.colorUpdateButtonText, updateButtonText.color);


            optionsMenuBorder.color = GetColorFromString(theme.colorOptionPanelBorder, optionsMenuBorder.color);
            optionsMenuPanel.color = GetColorFromString(theme.colorOptionPanel, optionsMenuPanel.color);


            ColorBlock colorblock = sensitivitySlider.colors;
            colorblock.normalColor = GetColorFromString(theme.colorSliderHandle, sensitivitySlider.colors.normalColor);
            
            sensitivitySlider.colors = colorblock;
            sensitivitySliderHandle.color = GetColorFromString(theme.colorSliderHandle, sensitivitySliderHandle.color);
            sensitivitySliderFill.color = GetColorFromString(theme.colorSliderFill, sensitivitySliderFill.color);
            sensitivitySliderBackground.color = GetColorFromString(theme.colorSliderBackground, sensitivitySliderBackground.color);

            startdelaySlider.colors = colorblock;
            startdelaySliderHandle.color = GetColorFromString(theme.colorSliderHandle, sensitivitySliderHandle.color);
            startdelaySliderFill.color = GetColorFromString(theme.colorSliderFill, sensitivitySliderFill.color);
            startdelaySliderBackground.color = GetColorFromString(theme.colorSliderBackground, sensitivitySliderBackground.color);

            foreach (Text text in playerNumbers)
            {
                text.color = GetColorFromString(theme.colorTextNumberPlayers, text.color);
            }
            playerNumberQuestion.color = GetColorFromString(theme.colorTextNumberPlayers, playerNumberQuestion.color);
            foreach (Text text in optionsText)
            {
                text.color = GetColorFromString(theme.colorOptionsText, text.color);
                text.GetComponent<Outline>().effectColor = GetColorFromString(theme.colorOptionsTextOutline, text.GetComponent<Outline>().effectColor);
            }

            playerNumbersPanel.color = GetColorFromString(theme.colorPanelNumberPlayers, playerNumbersPanel.color);
            playerNumberQuestion.color = GetColorFromString(theme.colorTextNumberPlayersQuestion, playerNumbersPanel.color);
            playerNumberQuestionPanel.color = GetColorFromString(theme.colorPanelNumberPlayersQuestion, playerNumberQuestionPanel.color);
            playerNumberSelectedPanel.color = GetColorFromString(theme.colorCentralPanelNumberPlayers, playerNumberSelectedPanel.color);

            continueArrow.sprite = continueArrowBW;
            continueArrow.color = GetColorFromString(theme.colorContinueArrow, continueArrow.color);


            themesButton.sprite = optionButtonBW;
            themesButton.color = GetColorFromString(theme.colorThemesButton, themesButton.color);

            themesQuitButton.sprite = quitButtonBW;
            themesQuitButton.color = GetColorFromString(theme.colorThemesQuitButton, themesQuitButton.color);

            themesPanel.color = GetColorFromString(theme.colorThemesPanel, themesPanel.color);


            themesTextImage.color = GetColorFromString(theme.colorThemesTitleText, themesTextImage.color);
            themesTextVideo.color = GetColorFromString(theme.colorThemesTitleText, themesTextVideo.color);

            themesScroll.selectedColor = GetColorFromString(theme.colorThemeSelectedIndicator, themesScroll.selectedColor);
            themesScroll.unselectedColor = GetColorFromString(theme.colorThemeUnselectedIndicator, themesScroll.unselectedColor);

            indicator.GetComponent<Image>().color = themesScroll.unselectedColor;

            themesScroll.RefreshIndicators();



            packButton.sprite = optionButtonBW;
            addPackButton.sprite = optionButtonBW;
            newPackButton.sprite = optionButtonBW;
            packBackButton.sprite = optionButtonBW;

            packButton.color = GetColorFromString(theme.colorPackButton, packButton.color);
            addPackButton.color = GetColorFromString(theme.colorAddPackButton, addPackButton.color);
            packButtonText.color = GetColorFromString(theme.colorPackButtonText, packButtonText.color);
            addPackButtonText.color = GetColorFromString(theme.colorAddPackButtonText, addPackButtonText.color);
            newPackButton.color = GetColorFromString(theme.colorNewPackButton, newPackButton.color);
            newPackButtonText.color = GetColorFromString(theme.colorNewPackButtonText, newPackButtonText.color);
            packsTitleBackground.color = GetColorFromString(theme.colorPacksTitleBackground, packsTitleBackground.color);
            packTitleText.color = GetColorFromString(theme.colorPacksTitleText, packTitleText.color);
            packBackButton.color = GetColorFromString(theme.colorPackBackButton, packBackButton.color);
            packBackButtonText.color = GetColorFromString(theme.colorPackBackButtonText, packBackButtonText.color);

            newPackFolderInputBackground.color = GetColorFromString(theme.colorNewPackFolderInputBackground, packBackButtonText.color);
            newPackFolderInputText.color = GetColorFromString(theme.colorNewPackFolderInputText, packBackButtonText.color);
            newPackFolderInputPlaceholderText.color = GetColorFromString(theme.colorNewPackFolderInputPlaceholderText, packBackButtonText.color);
            newPackFolderCreateInstructionsLink.color = GetColorFromString(theme.colorNewPackFolderCreateInstructionsLink, packBackButtonText.color);
            newPackFolderAddInstructionsText.color = GetColorFromString(theme.colorNewPackFolderAddInstructionsText, packBackButtonText.color);


            PackControl.bannerBackgroundColor = GetColorFromString(theme.colorPackBannerBackground, PackControl.bannerBackgroundColor);
            PackControl.bannerTextColor = GetColorFromString(theme.colorPackBannerText, PackControl.bannerTextColor);
            PackControl.updateButtonColor = GetColorFromString(theme.colorUpdateButton, PackControl.updateButtonColor);
            PackControl.updateButtonTextColor = GetColorFromString(theme.colorUpdateButtonText, PackControl.updateButtonTextColor);

            foreach (Text text in contentHolder.GetComponentsInChildren<Text>())
            {
                text.color = GetColorFromString(theme.colorThemesTitleText, text.color);
            }

            foreach (var packControl in contentHolder.GetComponentsInChildren<PackControl>())
            {
                packControl.bannerBackground.color = GetColorFromString(theme.colorPackBannerBackground, packControl.bannerBackground.color);
                packControl.nameText.color = GetColorFromString(theme.colorPackBannerText, packControl.nameText.color);
            }

            Camera.main.backgroundColor = GetColorFromString(theme.backgroundColorMainMenu, Camera.main.backgroundColor);

            string path = Path.Combine(Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "","Themes"), theme.mainMenuBackground);
            BackgroundHandler.UseAsBackground(path);
        }
        else
        {
            
            foreach (CheckBox checkbox in checkboxes)
            {
                Image checkBoxImage = checkbox.GetComponent<Image>();
                checkBoxImage.sprite = checkbox.State ? checkBoxOnColor : checkBoxColor;

                checkbox.boxOff = checkBoxColor;
                checkbox.boxOn = checkBoxOnColor;
                checkBoxImage.color = Color.white;
            }
            quitButton.sprite = quitButtonColor;
            quitButton.color = Color.white;

            optionsButton.sprite = optionButtonColor;
            optionsButton.color = Color.white;
            optionsButtonText.color = Color.white;

            updateButton.sprite = optionButtonColor;
            updateButton.color = Color.white;
            updateButtonText.color = Color.white;

            optionsMenuBorder.color = classicOptionBorderColor;
            optionsMenuPanel.color = classicOptionPanelColor;

            sensitivitySlider.colors = classicSliderColorblock;
            sensitivitySliderHandle.color = classicSliderHandleColor;
            sensitivitySliderFill.color = classicSliderFillColor;
            sensitivitySliderBackground.color = classicSliderBackgroundColor;

            startdelaySlider.colors = classicSliderColorblock;
            startdelaySliderHandle.color = classicSliderHandleColor;
            startdelaySliderFill.color = classicSliderFillColor;
            startdelaySliderBackground.color = classicSliderBackgroundColor;

            foreach (Text text in playerNumbers)
            {
                text.color = Color.white;
            }
            playerNumberQuestion.color = Color.white;
            
            foreach (Text text in optionsText)
            {
                text.color = GetColorFromString("#323232", text.color);
                text.GetComponent<Outline>().effectColor = GetColorFromString("#FFD0FF", text.GetComponent<Outline>().effectColor);
            }

            playerNumbersPanel.color = new Color(0, 0, 0, 0.5f);
            playerNumberQuestionPanel.color = new Color(0, 0, 0, 0.5f);
            playerNumberSelectedPanel.color = classicSelectedPanelColor;

            continueArrow.sprite = continueArrowColor;
            continueArrow.color = Color.white;


            themesButton.sprite = optionButtonColor;
            themesButton.color = Color.white;

            themesQuitButton.sprite = quitButtonColor;
            themesQuitButton.color = Color.white;

            themesPanel.color = classicColorThemePanel;//GetColorFromString(theme.colorThemesPanel, themesPanel.color);

            themesTextImage.color = Color.white;

            themesTextVideo.color = Color.white;

            foreach(Text text in contentHolder.GetComponentsInChildren<Text>())
            {
                text.color = Color.white;
            }


            indicator.GetComponent<Image>().color = themesScroll.selectedColor;

            themesScroll.selectedColor = classicColorSelectedTheme;
            themesScroll.unselectedColor = Color.white;

            themesScroll.RefreshIndicators();


            packButton.sprite = optionButtonColor;
            addPackButton.sprite = optionButtonColor;
            newPackButton.sprite = optionButtonColor;
            packBackButton.sprite = optionButtonColor;

            packButton.color = Color.white;
            addPackButton.color = Color.white;
            packButtonText.color = Color.white;
            addPackButtonText.color = Color.white;
            newPackButton.color = Color.white;
            newPackButtonText.color = Color.white;
            packsTitleBackground.color = Color.black;
            packTitleText.color = Color.white;
            packBackButton.color = Color.white;
            packBackButtonText.color = Color.white;

            newPackFolderInputBackground.color = newPackFolderInputBackgroundColor;
            newPackFolderInputText.color = Color.white;
            newPackFolderInputPlaceholderText.color = newPackFolderInputPlaceholderTextColor;
            newPackFolderCreateInstructionsLink.color = newPackFolderCreateInstructionsLinkColor;
            newPackFolderAddInstructionsText.color = Color.white;

            PackControl.bannerBackgroundColor = Color.black;
            PackControl.bannerTextColor = classicOptionPanelColor;
            PackControl.updateButtonColor = Color.white;
            PackControl.updateButtonTextColor = Color.white;

            foreach (var packControl in contentHolder.GetComponentsInChildren<PackControl>())
            {
                packControl.bannerBackground.color = Color.black;
                packControl.nameText.color = Color.white;
            }


            Camera.main.backgroundColor = classicMainColor;
            BackgroundHandler.DefaultBackground();
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


    public void Awake()
    {
        classicMainColor = Camera.main.backgroundColor;
        classicOptionPanelColor = optionsMenuPanel.color;
        classicOptionBorderColor = optionsMenuBorder.color;
        classicSliderColorblock = sensitivitySlider.colors;
        classicSelectedPanelColor = playerNumberSelectedPanel.color;

        classicSliderHandleColor  = sensitivitySliderHandle.color;
        classicSliderFillColor = sensitivitySliderFill.color;
        classicSliderBackgroundColor = sensitivitySliderBackground.color;
        classicColorThemePanel = themesPanel.color;
        classicColorSelectedTheme = themesScroll.selectedColor;


        newPackFolderInputBackgroundColor = newPackFolderInputBackground.color;
        newPackFolderInputPlaceholderTextColor = newPackFolderInputPlaceholderText.color;
        newPackFolderCreateInstructionsLinkColor = newPackFolderCreateInstructionsLink.color;


        Debug.Log("Theme Loader Main Menu Awake");
    }

    public void Start()
    {
        Debug.Log("Theme Loader Main Menu Start");
        Debug.Log("GetThemes");
        GetThemes();
        Debug.Log("LoadTheme");
        LoadTheme();
        Debug.Log("LoadThemeSelection");
        LoadThemeSelection();
    }

    public void ReloadThemes()
    {
        GetThemes();
        LoadThemeSelection();
        for(int i=0;i<themes.Count;i++)
        {
            var theme = themes[i];
            if(theme.GetName() == currentTheme && theme.packId == currentThemePackId)
            {
                LoadThemeAtIndex(i + 1);
                break;
            }
        }
    }
    public void SerializeTheme()
    {
        Theme newTheme = new Theme();
        newTheme.mainColor = "cyan";
        newTheme.secondaryColor = "green";
        newTheme.mainTextColor = "black";

        newTheme.backgroundColorMainMenu = "cyan";
        newTheme.backgroundColorDecksChoice= "cyan";
        newTheme.backgroundColorGame = "cyan";

        newTheme.Check();
        JsonSerialization.WriteToJsonResource<Theme>("theme", newTheme);
    }

    void GetThemes()
    {
        themes = new List<Theme>();
        Debug.Log("Getting theme files");

        List<string> themeFiles = new List<string>();
        var packDirectories = Directory.GetDirectories(Path.Combine(PathManager.MainPath, "Packs"));
        foreach(var packDirectory in packDirectories)
        {
            Debug.Log($"Pack: {packDirectory}");
            var packThemeDirectory = Path.Combine(packDirectory, "Themes");
            if (!Directory.Exists(packThemeDirectory))
            {
                Debug.Log($"Pack: {packDirectory} does not exist");
                continue;
            }
            string[] packThemeFiles = Directory.GetFiles(packThemeDirectory, "*.json");
            Debug.Log($"Pack: {packDirectory} has {packThemeFiles.Length} theme files");
            foreach (var themeFile in packThemeFiles)
            {
                themeFiles.Add(themeFile);
            }
        }
        Debug.Log($"Got {themeFiles?.Count.ToString() ?? "null"} theme files");
        theme = null;
        Global.theme = null;

        int selected = 0;
        for (int i=0;i<themeFiles.Count;i++)
        {
            string themeFile = themeFiles[i];
            Theme newTheme = JsonSerialization.ReadFromJson<Theme>(themeFile);
            newTheme.packId = new DirectoryInfo(new FileInfo(themeFile).DirectoryName).Parent.Name;
            Debug.Log("Theme Pack id: " + newTheme.packId);
            newTheme.Check();
            themes.Add(newTheme);

            string themeName = Path.GetFileName(themeFile);
            themeName = themeName.Substring(0, themeName.Length - 5);
            newTheme.SetName(themeName);
            Debug.Log("PlayerPrefs GetString theme");
            Debug.Log(PlayerPrefs.GetString("theme"));
            Debug.Log(themeName);
            Debug.Log($"Selected theme : [{PlayerPrefs.GetString("theme")}/{PlayerPrefs.GetString("themePackId")}]");
            Debug.Log($"Comparing to theme : [{themeName}/{theme?.packId ?? "null"}]");
            if (PlayerPrefs.GetString("theme") == themeName && PlayerPrefs.GetString("themePackId") == newTheme?.packId)
            {
                newTheme.Check();
                Global.theme = newTheme;
                theme = newTheme;
                themesScroll.startingPage = i + 1;
                selected = i + 1;
                currentTheme = themeName;
                currentThemePackId = theme.packId;
            }
        }

        foreach(Transform otherIndicator in indicator.transform.parent.GetComponentsInChildren<Transform>())
        {
            if(otherIndicator.GetInstanceID() != indicator.transform.GetInstanceID() && 
                otherIndicator.GetInstanceID() != indicator.transform.parent.GetInstanceID())
            {
                Destroy(otherIndicator.gameObject);
            }
        }

        for(int i=0;i<themes.Count;i++)
        {
            GameObject newIndicator = Instantiate(indicator);
            if(selected == i+1)
            {
                newIndicator.GetComponent<Image>().color = themesScroll.selectedColor;
            }
            newIndicator.transform.SetParent(indicator.transform.parent, false);
        }
        if (selected == 0)
        {
            indicator.GetComponent<Image>().color = themesScroll.selectedColor;
        }
    }
    
    void LoadThemeSelection()
    {
        foreach(Transform otherContent in contentHolder.GetComponentsInChildren<Transform>())
        {
            if(otherContent.GetInstanceID() != imageTheme.transform.GetInstanceID() &&
                otherContent.transform.parent.GetInstanceID() == contentHolder.transform.GetInstanceID())
            {
                Destroy(otherContent.gameObject);
            }
        }
        foreach(Theme theme in themes)
        {
            string themeDirectory = Path.Combine(PathManager.MainPath,"Packs",theme.packId, "Themes");
            string content = Path.Combine(themeDirectory, theme.mainMenuBackground);

            Debug.Log("content:" + content);
            if (string.IsNullOrEmpty(content) || !File.Exists(content))
            {
                content = Path.Combine(themeDirectory, theme.decksChoiceBackground);
                if (string.IsNullOrEmpty(content) || !File.Exists(content))
                {
                    content = Path.Combine(themeDirectory, theme.decksChoiceBackground); //theme.gameBackground;
                }
            }

            //IsVideo
            string[] videoFormats = { ".asf", ".avi", ".dv", ".m4v", ".mov", ".mp4", ".mpg",
            ".mpeg", ".ogv", ".vp8", ".webm", ".wmv" };
            bool isVideo = false;
            foreach (string s in videoFormats)
            {
                isVideo = isVideo || content.ToUpper().EndsWith(s.ToUpper());
            }
            isVideo = isVideo && File.Exists(content);


            if(isVideo)
            {
                GameObject videoHolder = Instantiate(videoTheme);
                videoHolder.transform.SetParent(imageTheme.transform.parent, false);
                videoHolder.GetComponentInChildren<VideoPlayer>().url = content;
                videoHolder.GetComponentInChildren<Text>().text = theme.GetName();


                videoHolder.SetActive(true);

                PackControl packControl = videoHolder.GetComponentInChildren<PackControl>(true);
                if (packControl && PackLoader.packPerId.ContainsKey(theme.packId))
                {
                    packControl.pack = PackLoader.packPerId[theme.packId];
                    packControl.Setup();
                    packControl.gameObject.SetActive(true);
                }

                if (videoHolder.activeInHierarchy)
                {
                    videoHolder.GetComponentInChildren<PlayVideo>().LoadVideo();
                }

            }
            else
            {
                GameObject imageHolder = Instantiate(imageTheme);
                imageHolder.transform.SetParent(imageTheme.transform.parent, false);
                imageHolder.GetComponentInChildren<Text>().text = theme.GetName();
                PackControl packControl = imageHolder.GetComponentInChildren<PackControl>(true);
                if (packControl && PackLoader.packPerId.ContainsKey(theme.packId))
                {
                    packControl.pack = PackLoader.packPerId[theme.packId];
                    packControl.Setup();
                    packControl.gameObject.SetActive(true);
                }

                StartCoroutine(LoadImage(content, imageHolder.GetComponent<Image>()));
            }

        }
        if(themesScroll.isActiveAndEnabled)
        {
            themesScroll.Reset();
        }

    }

    IEnumerator LoadImage(string path, Image image)
    {

        if (File.Exists(path))
        {

            Debug.Log($"About to get texture for: {path}");
            yield return null;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
            yield return null;
            Debug.Log($"About to send web request for: {path}");
            yield return www.SendWebRequest();

            Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");

            yield return null;


            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 1.0f);

            if (sprite != null)
            {
                image.sprite = sprite;
            }
            var aspectRatioFitter = image.GetComponentInChildren<AspectRatioFitter>();
            if (image.GetComponentInChildren<AspectRatioFitter>())
            {
                aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;
            }

            www.Dispose();
        }
    }

    public void LoadThemeAtIndex(int index)
    {
        currentTheme = theme?.GetName();
        currentThemePackId = theme?.packId;
        BackgroundHandler.ResetValues();
        if (index==0)
        {
            //hmm...
            theme = null;
            Global.theme = null;
            PlayerPrefs.SetString("theme", null);
            PlayerPrefs.SetString("themePackId", null);
            LoadTheme();
            Debug.Log("back to normal !");
        }
        else
        {
            if(index-1<themes.Count)
            {
                theme = themes[index - 1];
                PlayerPrefs.SetString("themePackId", theme.packId);
                PlayerPrefs.SetString("theme", theme.GetName());
                Global.theme = themes[index - 1];
                LoadTheme();
            }
            Debug.Log($"Loaded theme {theme.GetName()}");
        }
    }
}



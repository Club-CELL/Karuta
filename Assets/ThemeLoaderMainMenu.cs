using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
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


    List<Theme> themes = new List<Theme>();

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

    public void LoadTheme()
    {
        if(theme != null)
        {

            foreach(CheckBox checkbox in checkboxes)
            {
                Image checkBoxImage = checkbox.GetComponent<Image>();
                if (checkbox.activated)
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

            foreach (Text text in contentHolder.GetComponentsInChildren<Text>())
            {
                text.color = GetColorFromString(theme.colorThemesTitleText, text.color);
            }

            themesScroll.selectedColor = GetColorFromString(theme.colorThemeSelectedIndicator, themesScroll.selectedColor);
            themesScroll.unselectedColor = GetColorFromString(theme.colorThemeUnselectedIndicator, themesScroll.unselectedColor);

            indicator.GetComponent<Image>().color = themesScroll.unselectedColor;

            /*
            var indicatorImages = indicator.transform.parent.GetComponentsInChildren<Image>();
            for (int i = 0; i < indicatorImages.Length; i++)
            {
                if (themesScroll.PageIndex == i)
                {
                    indicatorImages[i].color = themesScroll.selectedColor;
                }
                else
                {
                    indicatorImages[i].color = themesScroll.unselectedColor;
                }
            }*/

            themesScroll.RefreshIndicators();
            Camera.main.backgroundColor = GetColorFromString(theme.backgroundColorMainMenu, Camera.main.backgroundColor);


            string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Themes"), theme.mainMenuBackground);
            BackgroundHandler.UseAsBackground(path);
            if (!string.IsNullOrEmpty(theme.mainMenuBackground) && File.Exists(path))
            {
            
            }
        }
        else///////////////////////////
        {
            
            foreach (CheckBox checkbox in checkboxes)
            {
                Image checkBoxImage = checkbox.GetComponent<Image>();
                if (checkbox.activated)
                {
                    checkBoxImage.sprite = checkBoxOnColor;
                }
                else
                {
                    checkBoxImage.sprite = checkBoxColor;
                }
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
            /*
            var indicatorImages = indicator.transform.parent.GetComponentsInChildren<Image>();
            for(int i=0;i<indicatorImages.Length;i++)
            {
                if(themesScroll.PageIndex == i)
                {
                    indicatorImages[i].color = themesScroll.selectedColor;
                }
                else
                {
                    indicatorImages[i].color = themesScroll.unselectedColor;
                }
            }*/
            themesScroll.RefreshIndicators();
            Camera.main.backgroundColor = classicMainColor;
            BackgroundHandler.DefaultBackground();
            //SceneManager.LoadScene("ChoixJoueurs");
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


    public void Awake()
    {
        //SerializeTheme();

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
        //GetTheme();
    }

    public void Start()
    {
        GetThemes();
        LoadTheme();
        LoadThemeSelection();
    }

    public void ReloadThemes()
    {
        GetThemes();
        LoadThemeSelection();
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
    /*public void GetTheme()
    {
        //theme = JsonSerialization.ReadFromJsonResource<Theme>("themeSakura");
        theme = JsonSerialization.ReadFromJson<Theme>(Path.Combine(Application.persistentDataPath, "Themes/themeSakura.json"));

        theme.Check();

        Global.theme = theme;
    }*/
    void GetThemes()
    {
        themes = new List<Theme>();
        string[] themeFiles = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "Themes"), "*.json");
        theme = null;
        Global.theme = null;

        int selected = 0;
        for (int i=0;i<themeFiles.Length;i++)
        {
            string themeFile = themeFiles[i];
            Theme newTheme = JsonSerialization.ReadFromJson<Theme>(themeFile);
            newTheme.Check();
            themes.Add(newTheme);

            string themeName = Path.GetFileName(themeFile);
            themeName = themeName.Substring(0, themeName.Length - 5);
            newTheme.SetName(themeName);
            Debug.Log(PlayerPrefs.GetString("theme"));
            Debug.Log(themeName);
            if(PlayerPrefs.GetString("theme") == themeName)
            {
                newTheme.Check();
                Global.theme = newTheme;
                theme = newTheme;
                themesScroll.startingPage = i + 1;
                selected = i + 1;
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
            newIndicator.transform.parent = indicator.transform.parent;
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
            string content = Path.Combine(Path.Combine(Application.persistentDataPath, "Themes"), 
                theme.mainMenuBackground);
            Debug.Log("content:" + content);
            if (string.IsNullOrEmpty(content) || !File.Exists(content))
            {
                content = theme.decksChoiceBackground;
                if (string.IsNullOrEmpty(content) || !File.Exists(content))
                {
                    content = theme.gameBackground;
                }
            }

            //IsVideo
            string[] videoFormats = { ".asf", ".avi", ".dv", ".m4v", ".mov", ".mp4", ".mpg",
            ".mpeg", ".ogv", ".vp8", ".webm", ".wmv" };
            bool isVideo = false;
            foreach (string s in videoFormats)
            {
                isVideo = isVideo || content.EndsWith(s);
            }
            isVideo = isVideo && File.Exists(content);


            if(isVideo)
            {
                GameObject videoHolder = Instantiate(videoTheme);
                videoHolder.transform.parent = imageTheme.transform.parent;
                videoHolder.GetComponentInChildren<VideoPlayer>().url = content;
                videoHolder.GetComponentInChildren<Text>().text = theme.GetName();
                videoHolder.SetActive(true);

                if(videoHolder.activeInHierarchy)
                {
                    videoHolder.GetComponentInChildren<PlayVideo>().LoadVideo();
                }

            }
            else
            {
                GameObject imageHolder = Instantiate(imageTheme);
                imageHolder.transform.parent = imageTheme.transform.parent;
                imageHolder.GetComponentInChildren<Text>().text = theme.GetName();
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

            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + path);
            yield return www.SendWebRequest();


            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 1.0f);

            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public void LoadThemeAtIndex(int index)
    {
        BackgroundHandler.ResetValues();
        if (index==0)
        {
            //hmm...
            theme = null;
            Global.theme = null;
            PlayerPrefs.SetString("theme", null);
            LoadTheme();
            Debug.Log("back to normal !");
        }
        else
        {
            if(index-1<themes.Count)
            {
                theme = themes[index - 1];
                PlayerPrefs.SetString("theme", theme.GetName());
                Global.theme = themes[index - 1];
                LoadTheme();
            }
        }
    }
}



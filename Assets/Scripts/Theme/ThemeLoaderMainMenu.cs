using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class ThemeLoaderMainMenu : MonoBehaviour
{
    [Header("Default colors")]
    [SerializeField] private Color mainColor;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private Color mainTextColor;
    [SerializeField] private Color secondaryTextColor;

    [Header("Theme selection")]
    [SerializeField] private GameObject imageTheme;
    [SerializeField] private GameObject videoTheme;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject contentHolder;
    [SerializeField] private ScrollSnapRect themesScroll;

    [Header("Objects to change")]
    [SerializeField] private List<Text> mainTexts;
    [SerializeField] private List<Text> secondaryTexts;
    [SerializeField] private List<TMP_Text> tmp_texts;
    [SerializeField] private List<GameModeButton> gameModes;
    [SerializeField] private List<Image> buttons;
    [SerializeField] private List<CheckBox> checkboxes;
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private List<Image> panels;
    [SerializeField] private List<Image> panelBorders;

    private List<Theme> themes = new();
    private Theme theme;

    string currentThemePackId;
    string currentTheme;

    public void Start()
    {
        GetThemes();
        LoadTheme();
        LoadThemeSelection();
    }

    public void LoadTheme()
    {
        if (theme == null)
        {
            Theme defaultTheme = new("default", mainColor, mainTextColor, secondaryColor, secondaryTextColor);
            defaultTheme.Check();
            Global.theme = defaultTheme;
            theme = defaultTheme;

            Camera.main.backgroundColor = mainColor;
            BackgroundHandler.DefaultBackground();
        }
        else
        {
            Camera.main.backgroundColor = GetColorFromString(theme.mainMenuBackgroundColor, Camera.main.backgroundColor);

            string path = Path.Combine(Path.Combine(PathManager.MainPath, "Packs", theme.packId ?? "", "Themes"), theme.mainMenuBackground);
            BackgroundHandler.UseAsBackground(path);
        }

        foreach (Text text in mainTexts)
        {
            text.color = GetColorFromString(theme.mainTextColor, text.color);
            if (text.TryGetComponent<Outline>(out var outline))
            {
                outline.effectColor = GetColorFromString(theme.textOutlineColor, outline.effectColor);
            }
        }

        foreach (Text text in secondaryTexts)
        {
            text.color = GetColorFromString(theme.secondaryTextColor, text.color);
            if (text.TryGetComponent<Outline>(out var outline))
            {
                outline.effectColor = GetColorFromString(theme.textOutlineColor, outline.effectColor);
            }
        }

        foreach (TMP_Text text in tmp_texts)
        {
            text.color = GetColorFromString(theme.mainTextColor, text.color);
            if (text.TryGetComponent<Outline>(out var outline))
            {
                outline.effectColor = GetColorFromString(theme.textOutlineColor, outline.effectColor);
            }
        }

        Color buttonColor = GetColorFromString(theme.buttonsColor, Color.white);
        Color secondColor = GetColorFromString(theme.secondaryColor, Color.white);
        foreach (GameModeButton button in gameModes)
        {
            button.Set(buttonColor, secondColor, button.GameMode == Global.GameModes.Classic);
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.color = GetColorFromString(theme.mainTextColor, buttonText.color);
        }

        foreach (Image button in buttons)
        {
            button.color = buttonColor;
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.color = GetColorFromString(theme.mainTextColor, buttonText.color);
        }

        foreach (CheckBox checkbox in checkboxes)
        {
            Image checkBoxImage = checkbox.GetComponent<Image>();
            checkBoxImage.color = GetColorFromString(theme.checkBoxColor, Color.white);
        }

        foreach (Slider slider in sliders)
        {
            SetSliderColor(slider, "Background", theme.sliderBackgroundColor);
            SetSliderColor(slider, "Fill Area/Fill", theme.sliderFillColor);
            SetSliderColor(slider, "Handle Slide Area/Handle", theme.sliderHandleColor);

            Text textValue = slider.GetComponent<SliderValue>().textValue;
            textValue.color = GetColorFromString(theme.mainTextColor, textValue.color);
        }

        foreach (Image panel in panels)
        {
            panel.color = GetColorFromString(theme.panelsColor, Color.white);
        }

        foreach (Image border in panelBorders)
        {
            border.color = GetColorFromString(theme.panelBorderColor, Color.white);
        }

        themesScroll.selectedColor = GetColorFromString(theme.buttonsColor, themesScroll.selectedColor);
        themesScroll.unselectedColor = GetColorFromString(theme.buttonInactiveColor, themesScroll.unselectedColor);
        indicator.GetComponent<Image>().color = themesScroll.unselectedColor;
        themesScroll.RefreshIndicators();

        PackControl.bannerBackgroundColor = GetColorFromString(theme.gameBackground, PackControl.bannerBackgroundColor);
        PackControl.bannerTextColor = GetColorFromString(theme.mainTextColor, PackControl.bannerTextColor);
        PackControl.updateButtonColor = GetColorFromString(theme.buttonsColor, PackControl.updateButtonColor);
        PackControl.updateButtonTextColor = GetColorFromString(theme.mainTextColor, PackControl.updateButtonTextColor);
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

    private Color GetColorFromString(string s, Color defaultColor)
    {
        if (ColorUtility.TryParseHtmlString(s, out Color color))
        {
            return color;
        }
        return defaultColor;
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

    void GetThemes()
    {
        themes = new List<Theme>();
        Debug.Log("Getting theme files");

        List<string> themeFiles = new();
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

        theme = null;
        Global.theme = null;

        int selected = 0;
        for (int i=0;i<themeFiles.Count;i++)
        {
            string themeFile = themeFiles[i];
            Theme newTheme = JsonSerialization.ReadFromJson<Theme>(themeFile);
            newTheme.packId = new DirectoryInfo(new FileInfo(themeFile).DirectoryName).Parent.Name;
            
            newTheme.Check();
            themes.Add(newTheme);

            string themeName = Path.GetFileName(themeFile);
            themeName = themeName[..^5];
            newTheme.SetName(themeName);

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

            if (string.IsNullOrEmpty(content) || !File.Exists(content))
            {
                content = Path.Combine(themeDirectory, theme.decksChoiceBackground);
                if (string.IsNullOrEmpty(content) || !File.Exists(content))
                {
                    content = Path.Combine(themeDirectory, theme.decksChoiceBackground);
                }
            }

            //Checks if file is a video
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

            if (www.isDone)
            {
                bool success = www.result == UnityWebRequest.Result.Success;
                bool handlerDone = www.downloadHandler.isDone;
                string errorstring = (www.error ?? "no error");

                Debug.Log($"Request for {path} is done: {success && handlerDone} - Error: {errorstring}");
            }
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
            theme = null;
            Global.theme = null;
            PlayerPrefs.SetString("theme", null);
            PlayerPrefs.SetString("themePackId", null);
            LoadTheme();
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



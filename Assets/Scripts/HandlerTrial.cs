using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandlerTrial : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int answerPts;
    [SerializeField] private int streakBonus;
    [SerializeField] private int maxIndicators = 10;
    [SerializeField] private float answerDisplayTime;

    [Header("Dependancies")]
	[SerializeField] private Text remainingText;
	[SerializeField] private Text scoreText;
	[SerializeField] private Text streakText;
    [SerializeField] private GameObject validPanel;
    [SerializeField] private GameObject incorrectPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private PlayPause pauseButton;
    [SerializeField] private Sprite defaultSprite;

    [Header("Cards display")]
    [SerializeField] private GameObject cardImage;
    [SerializeField] private GameObject indicator;
    [SerializeField] private Transform indicatorContainer;
    [SerializeField] private Transform contentHolder;
    [SerializeField] private ScrollSnapRect cardsScroll;

    //Game variables
    private int answerPool;
    private int remaining;
    private int deckSize;
    private int cardIndex;
    private int cardScrollIndex;
    
    private int score;
    private int streak;
    private string Main_Folder;

    readonly List<Card> deck = new();
    readonly System.Random rng = new();

    IEnumerator coroutine;
    bool questionReady;
    bool songLoaded;
    bool waitingToPlay;
    bool clipstarted;
    bool paused;

    string loadingSongPath;
    public string LoadingSongPath { get => loadingSongPath; }

    //Components
    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Main_Folder = Global.mainPath;
        Main_Folder = PathManager.MainPath;
        answerPool = Global.trialChoices == int.MinValue ? Global.deck.Count : Global.trialChoices;
        remaining = Global.trialLength;

        AddCardIndicators();
    }

    private void Start ()
    {
        remainingText.text = "Remaining: " + remaining.ToString();

        ReadDecks();
		First();
    }
	
	private void Update ()
    {
        if (questionReady && waitingToPlay)
        {
            waitingToPlay = false;
            if (!paused) PlayClip();
            else PauseClip();
        }

        if (clipstarted)
        {
            musicSlider.value = source.time / source.clip.length;
        }
	}

    private void ReadDecks()
    {
        deck.Clear();

        foreach (var s in Global.deck)
        {
            deck.Add(s);
        }

        deckSize = deck.Count;
    }

    private void First()
    {
        questionReady = true;
        if (deckSize > 0) CreateCards();
        else Finish();
    }

    public void PlayPauseAction()
    {
        if (songLoaded)
        {
            if (source.isPlaying) PauseClip();
            else PlayClip();
        }
        else
        {
            if (!paused) SetPause();
            else SetPlay();
        }
    }

    public void Answer()
    {
        if (questionReady && clipstarted)
        {
            clipstarted = false;
            questionReady = false;

            if (cardsScroll.PageIndex == cardScrollIndex)
            {
                if (deckSize > 0)
                {
                    deck.RemoveAt(cardIndex);
                    deckSize--;
                }

                int scoreIncrease = answerPts + streakBonus * streak;
                score += scoreIncrease;
                streak++;

                scoreText.text = "Score: " + score;
                streakText.text = "Streak: " + streak;

                validPanel.SetActive(true);
                validPanel.GetComponentInChildren<Text>().text = "Correct!\nScore +" + scoreIncrease;
            }
            else
            {
                streak = 0;
                incorrectPanel.SetActive(true);
                streakText.text = "Streak: 0";
            }

            StartCoroutine(PrepareNextCards());
            StartCoroutine(DisplayAnswer());
        }
    }

    private IEnumerator PrepareNextCards()
    {
        foreach (Transform card in contentHolder)
        {
            Destroy(card.gameObject);
        }

        yield return new WaitForEndOfFrame();

        if (remaining > 0)
        {
            remaining--;
            remainingText.text = "Remaining: " + remaining.ToString();

            CreateCards();
            Resources.UnloadUnusedAssets();
        }
        else Finish();
    }

	private IEnumerator DisplayAnswer()
	{
        yield return new WaitForSeconds(answerDisplayTime);

        validPanel.SetActive(false);
        incorrectPanel.SetActive(false);
        questionReady = true;
    }

    private void CreateCards()
    {
        cardIndex = Random.Range(0, deckSize);
        List<int> cards = new() { cardIndex };

        int rand;
        int maxAnswers = Mathf.Min(answerPool, deckSize);
        for (int i = 1; i < maxAnswers; i++)
        {
            do rand = Random.Range(0, deckSize);
            while (cards.Contains(rand));

            cards.Add(rand);
        }
        Shuffle(cards);

        for (int i = 0; i < maxAnswers; i++)
        {
            if (cards[i] == cardIndex)
            {
                cardScrollIndex = i;
                PlayCard(cardIndex);
            }
            else AddCardImage(cards[i]);
        }

        cardsScroll.Init();
        cardsScroll.RefreshIndicators();
    }

	private void PlayCard(int cardIndex)
	{
        Card cardInfo = deck[cardIndex];
        string cardName = cardInfo.name;

        char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        foreach (var character in forbiddenChar)
        {
            cardName = cardName.Replace(character, ',');
        }
        string soundPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Sounds", cardName + ".mp3");
        string imagePath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Visuals", cardName + ".png");

        GameObject imageHolder = Instantiate(cardImage, contentHolder);
        StartCoroutine(LoadImage(imagePath, imageHolder.GetComponent<Image>()));

        pauseButton.ChangeState(true);

        if (!File.Exists(soundPath))
        {
            Debug.LogError($"Did not find sound at: {soundPath}");
            return;
        }
        PlaySong (soundPath);
	}

    private void AddCardImage(int cardIndex)
    {
        Card cardInfo = deck[cardIndex];
        string cardName = cardInfo.name;

        char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        foreach (var character in forbiddenChar)
        {
            cardName = cardName.Replace(character, ',');
        }
        string imagePath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Visuals", cardName + ".png");

        GameObject imageHolder = Instantiate(cardImage, contentHolder);
        StartCoroutine(LoadImage(imagePath, imageHolder.GetComponent<Image>()));
    }

    private void Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private void AddCardIndicators()
    {
        if (answerPool > maxIndicators) return;

        for (int i = 0; i < answerPool; i++)
        {
            GameObject newIndicator = Instantiate(indicator);
            if (i == 0) newIndicator.GetComponent<Image>().color = cardsScroll.selectedColor;
            newIndicator.transform.SetParent(indicatorContainer, false);
        }
    }

    private void Finish()
	{
        validPanel.SetActive(true);
        string finishText = "Finished!\nScore: " + score;
        if (score > PlayerPrefs.GetInt("score", 0))
        {
            finishText += "\nHighscore!";
            PlayerPrefs.SetInt("score", score);
        }
        validPanel.GetComponentInChildren<Text>().text = finishText;
        remainingText.text = "Finished ! ";
    }

	public AudioClip SongLoadResources(string path)
	{
		AudioClip clp = (AudioClip) Resources.Load(path, typeof(AudioClip));
		return clp;
	}

    IEnumerator LoadImage(string path, Image image)
    {
        if(!File.Exists(path))
        {
            image.sprite = defaultSprite;
            image.preserveAspect = true;
            yield break;
        }

        image.sprite = null;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        Debug.Log("<color=yellow>file://" + path+"</color>");
        yield return www.SendWebRequest();

        Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");

        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 1.0f);

        image.sprite = sprite != null ? sprite : defaultSprite;
        image.preserveAspect = true;
        www.Dispose();
    }

    public void PlaySong(string path)
	{
		songLoaded = false;
		loadingSongPath = path;
        
        if(coroutine != null) StopCoroutine(coroutine);
        
        coroutine = LoadSong(path);
        StartCoroutine(coroutine);
	}

	IEnumerator LoadSong(string path)
	{
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
        yield return www.SendWebRequest();
        
        ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
        source.clip = DownloadHandlerAudioClip.GetContent(www);
        
        songLoaded = true;
		if (source.clip.loadState == AudioDataLoadState.Loaded)
        {
            waitingToPlay = true;
		}

        www.Dispose();
    }

    void PauseClip()
    {
        source.Pause();
        SetPause();
    }

    void PlayClip()
    {
        if (!clipstarted) source.Play();
        else source.UnPause();
        clipstarted = true;
        SetPlay();
    }

    void SetPlay() => SetPauseIcon(false);
    void SetPause() => SetPauseIcon(true);

    void SetPauseIcon(bool paused)
    {
        this.paused = paused;
        pauseButton.ChangeState(!paused);
    }
}
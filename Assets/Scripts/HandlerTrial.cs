using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandlerTrial : MonoBehaviour {

    [Header("Dependancies")]
    public Text selected;
	public Text remainingText;
	public Text score;
	public Text streak;
    public Slider musicSlider;
    public GameObject PauseIcon;
    public Sprite defaultSprite;

    //Game variables
    private int remaining;
    private int deckSize;
    private int cardIndex;
    private string Main_Folder;
    readonly List<Card> deck = new();

    IEnumerator coroutine;
    bool songLoaded;
    bool clipstarted;
    bool paused;

    string loadingSongPath;
    public string LoadingSongPath { get => loadingSongPath; }

    //Components
    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start ()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Main_Folder = Global.mainPath;
        Main_Folder =  PathManager.MainPath;
        remaining = Global.trialLength;
        remainingText.text = "Remaining: " + remaining.ToString();

        ReadDecks();
		First();
    }
	
	void Update ()
    {
        if (clipstarted)
        {
            musicSlider.value = source.time / source.clip.length;
        }
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

	void ReadDecks()
	{
        deck.Clear();
        
        foreach(var s in Global.deck)
        {
            deck.Add(s);
        }
		deckSize = deck.Count;
	}

	void Trouvee()
	{
		if (deckSize > 0)
        {
            deck.RemoveAt(cardIndex);
			deckSize--;
		}

		Next ();
	}

	void First() //Next, but without the swap
	{
		if (deckSize > 0)
        {
			cardIndex = UnityEngine.Random.Range (0, deckSize);
			Playcard (cardIndex);
        }
        else Finish ();
	}

	void Next()
	{
		if (remaining > 0)
        {
            remaining--;
			remainingText.text = "Remaining: " + remaining.ToString ();
			cardIndex = UnityEngine.Random.Range (0, deckSize);
			Playcard (cardIndex);
			Resources.UnloadUnusedAssets ();
        }
        else Finish ();
    }

	void Playcard(int c)
	{
        Card cardInfo = deck[c];
        string cardName = cardInfo.name;
        Debug.Log($"Playing card: {cardName}");

        char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        foreach (var character in forbiddenChar)
        {
            cardName = cardName.Replace(character, ',');
        }

        Debug.Log($"imPath: {Main_Folder}{Path.DirectorySeparatorChar}Packs{cardInfo.packId}Visuals{cardName +  ".png"}");
        string imPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Visuals", cardName +  ".png");
        Debug.Log($"imPath: {imPath}");

        //StartCoroutine(LoadImage(imPath, carte2Image));

        string soundPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Sounds", cardName + ".mp3");
        Debug.Log(soundPath);

        clipstarted = false;
        PauseIcon.GetComponent<PlayPause>().playing = true;
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        Color pauseColor = PauseIcon.GetComponent<Image>().color;
        PauseIcon.GetComponent<Image>().color = new Color(pauseColor.r, pauseColor.g, pauseColor.b, 0);

        if (!File.Exists(soundPath))
        {
            //answer.text = cardName + "\n<color=red>Sound not found</color>";
            Debug.LogError($"Did not find sound at: {soundPath}");
            return;
        }
        PlaySong (soundPath);
	}

	void Finish()
	{
        remainingText.text = "Finished ! ";
    }

	public AudioClip SongLoadResources(string path)
	{
		AudioClip clp = (AudioClip)Resources.Load (path, typeof(AudioClip));
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

	IEnumerator LoadSong(string path)/*IEnumerator*/
	{
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
        yield return www.SendWebRequest();
        
        ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
        source.clip = DownloadHandlerAudioClip.GetContent(www);
        
        songLoaded = true;
		if (source.clip.loadState == AudioDataLoadState.Loaded)
        {
            if (!paused) PlayClip();
            else PauseClip();
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
        PauseIcon.GetComponent<PlayPause>().playing = !paused;
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandlerEX : MonoBehaviour {

	public List<Card> deck = new();

    [Header("Debug")]
	public int nb;
	public int cardIndex;
	public string Main_Folder;

    [Header("Dependancies")]
	public Transform card;
	public Transform card2;
	public Transform card3;
	public GameObject arrowFound;
	public GameObject arrowNotFound;
    public GameObject revealButton;
    public Text answer;
	public Text remaining;
    public Text startDelayText;
    public GameObject PauseIcon;
    public Sprite defaultSprite;

    [Header("Parameters")]
	public float back_speed_x;
	public float back_speed_y;
	public float tresh_x;
	public float tresh_y;
	public float turn_ratio;

	public float move_ratio;
	public float threshold;
	public float validate_x;
	public float y_diff_restart;
    public float x3_thres;
    
    //Game variables
    float x0;
	float y0;
	float x_start_touch;
	float y_start_touch;
	float x3;
	float y3;

	Vector2 fingerStart = new(0,0);
	Vector2 fingerEnd = new(0,0);
    
    IEnumerator coroutine;
    bool hasMoved;
    bool songLoaded;
    bool clipstarted;
    bool paused;
    bool isStarting;
    float startClock;

    string loadingSongPath;
    public string LoadingSongPath { get => loadingSongPath; }

    //Option menu variables
    bool autoplay;
    bool playpause;
    bool hidecard;
    float startdelay;
    float pausesensitivity;

    //Components
    Image carteImage;
    Image carte2Image;
    Image carte3Image;
    Image flecheTrouveeImage;
    Image flecheNonTrouveeImage;

    Text flecheTrouveeText;
    Text flecheNonTrouveeText;

    Color flecheTrouveeImageColor;
    Color flecheNonTrouveeImageColor;
    Color flecheTrouveeTextColor;
    Color flecheNonTrouveeTextColor;
    AudioSource source;

    private void Awake()
    {
        carteImage = card.GetComponent<Image>();
        carte2Image = card2.GetComponent<Image>();
        carte3Image = card3.GetComponent<Image>();

        flecheTrouveeImage = arrowFound.GetComponent<Image>();
        flecheNonTrouveeImage = arrowNotFound.GetComponent<Image>();

        flecheTrouveeText = arrowFound.GetComponentInChildren<Text>(true);
        flecheNonTrouveeText = arrowNotFound.GetComponentInChildren<Text>(true);

        Debug.Log($"Fleche trouvee image is null ? {flecheTrouveeImage == null}");
        source = GetComponent<AudioSource>();
    }

    private void Start ()
    {
        carteImage.enabled = false;
        carte2Image.enabled = true;

        autoplay = PlayerPrefs.GetInt("autoplay", 1) == 1;
        playpause = PlayerPrefs.GetInt("playpause", 1) == 1;
        hidecard = PlayerPrefs.GetInt("hidecard", 1) == 1;
        startdelay = PlayerPrefs.GetFloat("startdelay", 0);
        pausesensitivity = PlayerPrefs.GetFloat("pausesensitivity", 50);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        revealButton.SetActive(hidecard);
        arrowNotFound.SetActive(true);
        arrowFound.SetActive(true);
        startDelayText.gameObject.SetActive(startdelay > 0);

		x0 = card.position.x;
		y0 = card.position.y;

        Main_Folder = Global.mainPath;
        Main_Folder =  PathManager.MainPath;

        ReadDecks ();
		First ();
		remaining.text = "Remaining: " + nb.ToString ();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(isStarting)
        {
            startClock -= Time.deltaTime;
            startDelayText.text = ((int)Mathf.Ceil(startClock)).ToString();

            if (startClock <= 0)
            {
                isStarting = false;
                startClock = startdelay;
                startDelayText.gameObject.SetActive(false);
                paused = false;
                PlayClip();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(songLoaded)
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

		SwipeDetect ();
		card.rotation = Quaternion.Euler(new Vector3(0, 0, card.position.x - x0) * turn_ratio);
		
		Vector2 pos = card.position;
		if (pos.x >= x0) {
			Color col = flecheTrouveeImageColor;
			flecheTrouveeImage.color= new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));
			col = flecheTrouveeTextColor;
			flecheTrouveeText.color = new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));

            flecheNonTrouveeImage.color = Color.clear;
			flecheNonTrouveeText.color = Color.clear;

		}
		if (pos.x <= x0) {
			Color col = flecheNonTrouveeImageColor;
			flecheNonTrouveeImage.color = new Color(col.r,col.g,col.b,Math.Min(1,(x0 - pos.x)/validate_x));
			col = flecheNonTrouveeTextColor;
			flecheNonTrouveeText.color = new Color(col.r,col.g,col.b,Math.Min(1,(x0 - pos.x)/validate_x));

			flecheTrouveeImage.color= Color.clear;
			flecheTrouveeText.color = Color.clear;
		}

		if (carte3Image.enabled)
        {
		    x3 = card3.position.x;
			y3 = card3.position.y;

			if (x3 < x0 && x3 > x0 - x3_thres || x3 >= x0 && x3 < x0 + x3_thres)
            {	
                card3.SetPositionAndRotation(new Vector2 (x3 + (x3 - x0) * back_speed_x, y3), 
                    Quaternion.Euler(new Vector3(0,0,card3.position.x-x0)*turn_ratio));
            }
            else
            {
				carte3Image.enabled = false;
			}
		}
	}

    public Vector2 SwipeDetect()
    {
        if (Input.GetMouseButtonDown(0)) //Start touch
        {
            hasMoved = false;
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = card.position.x;
            y_start_touch = card.position.y;
        }
        else if (Input.GetMouseButtonUp(0)) //End touch
        {
            float distance = Vector2.Distance(fingerStart, fingerEnd);
            if (distance <= 10 * pausesensitivity)
            {
                hasMoved = false;
            }

            bool changedCard = LetCard();

            if (!hasMoved && !changedCard)
            {
                if (songLoaded)
                {

                    if (source.isPlaying && playpause) PauseClip();
                    else if (paused) PlayClip();
                    else
                    {
                        if (source.clip.loadState == AudioDataLoadState.Loaded && !source.isPlaying)
                        {
                            PlayClip();
                        }
                    }
                }
                else
                {
                    if (!paused && playpause) SetPause();
                    if (paused) SetPlay();
                }

            }
        }
        else if (Input.GetMouseButton(0))
        {
            hasMoved = true;
            fingerEnd = Input.mousePosition;
            float delta_x = fingerEnd.x - fingerStart.x;
            float delta_y = fingerEnd.y - fingerStart.y;
            HoldCard(delta_x, delta_y);
        }
        else
        {
            LetCard();
        }
        return new Vector2(0, 0);
    }

    public void SetButtonColors()
    {
        flecheTrouveeImageColor = flecheTrouveeImage.color;
        flecheNonTrouveeImageColor = flecheNonTrouveeImage.color;
        flecheTrouveeTextColor = flecheTrouveeText.color;
        flecheNonTrouveeTextColor = flecheNonTrouveeText.color;
    }

    void SwapCards()
    {
        StartCoroutine(SwapCardsCoroutine());
    }
    IEnumerator SwapCardsCoroutine()
	{
		if (carteImage.enabled) {

			Vector2 pos = card.position;

			carte3Image.enabled = true;
			carte3Image.preserveAspect = true;
			carte3Image.sprite = carteImage.sprite;
            card3.SetPositionAndRotation(new Vector2(pos.x, pos.y), 
                Quaternion.Euler(new Vector3(0,0,card.rotation.z)));
            
            carteImage.enabled = false;
            card.SetPositionAndRotation(new Vector2(x0, y0), 
                Quaternion.Euler(new Vector3(0,0,0)));

            carte2Image.enabled = true;
            card2.SetPositionAndRotation(new Vector2(x0, y0 - y_diff_restart), 
                Quaternion.Euler(new Vector3(0,0,0)));
        }

        (card2, card) = (card, card2);
        (carte2Image, carteImage) = (carteImage, carte2Image);

        carteImage.enabled = false;
        while (carteImage.sprite == null)
        {
            yield return null;
        }
        card.position = new Vector2(x0, y0 - y_diff_restart);
        carteImage.enabled = true;

        PauseIcon.transform.SetParent(card);
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
	void HoldCard(float d_x, float d_y)
	{
        Vector2 pos = new(x_start_touch + d_x, y_start_touch + d_y);
        card.position = pos;
	}

	bool LetCard()//returns true if another card is found
	{
		Vector2 pos = card.position;
		float dx=0;
		float dy=0;

		if (pos.x > x0) {
			dx = Math.Min (-tresh_x, (x0-pos.x)*back_speed_x);
		}
		if (pos.x < x0) {
			dx = Math.Max (tresh_x, (x0-pos.x)*back_speed_x);
		}
		if (pos.y > y0) {
			dy = Math.Min (-tresh_y, (y0-pos.y)*back_speed_y);
		}
		if (pos.y < y0) {
			dy = Math.Max (tresh_y, (y0-pos.y)*back_speed_y);
		}
		pos = new Vector2 (pos.x + dx, pos.y + dy);
		card.position=pos;

		if (card.position.x > x0 + validate_x) { //Card found
			Trouvee();
            return true;
		}
		if (card.position.x < x0 - validate_x) { //Card found
			Next();
            return true;
        }
        return false;
	}

	void ReadDecks()
	{
        deck.Clear();
        
        foreach(var s in Global.deck)
        {
            deck.Add(s);
        }
		nb = deck.Count;
	}

	void Trouvee()
	{
		if (nb > 0)
        {
            deck.RemoveAt(cardIndex);
			//Deck [card] = Deck [nb - 1];
			nb--;
		}

		Next ();
	}

	void First() //Next, but without the swap
	{
		if (nb > 0)
        {
			cardIndex = UnityEngine.Random.Range (0, nb);
			Playcard (cardIndex);
            SwapCards();
        }
        else Finish ();
	}

	void Next()
	{
		if (nb > 0) {
			remaining.text = "Remaining: " + nb.ToString ();
			cardIndex = UnityEngine.Random.Range (0, nb);
			Playcard (cardIndex);
			Resources.UnloadUnusedAssets ();
            SwapCards();
        }
        else Finish ();
    }

	void Playcard(int c)
	{
        Card cardInfo = deck[c];
        string cardName = cardInfo.name;
        Debug.Log($"Playing card: {cardName}");
        answer.text = cardName;

        char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        foreach (var character in forbiddenChar)
        {
            cardName = cardName.Replace(character, ',');
        }

        Debug.Log($"imPath: {Main_Folder}{Path.DirectorySeparatorChar}Packs{cardInfo.packId}Visuals{cardName +  ".png"}");
        string imPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Visuals", cardName +  ".png");
        Debug.Log($"imPath: {imPath}");

        if (hidecard) HideCard(true);
        StartCoroutine(LoadImage(imPath, carte2Image));

        string soundPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Sounds", cardName + ".mp3");
        Debug.Log(soundPath);

        clipstarted = false;
        isStarting = startdelay > 0;
        paused = !autoplay || startdelay > 0;

        PauseIcon.GetComponent<PlayPause>().playing = autoplay;
        PauseIcon.transform.SetParent(carte2Image.enabled ? card2 : card);
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (autoplay)
        {
            Color pauseColor = PauseIcon.GetComponent<Image>().color;
            PauseIcon.GetComponent<Image>().color = new Color(pauseColor.r, pauseColor.g, pauseColor.b, 0);
        }

        if (startdelay > 0)
        {
            startClock = startdelay;
            startDelayText.gameObject.SetActive(true);
        }

        if (!File.Exists(soundPath))
        {
            answer.text = cardName + "\n<color=red>Sound not found</color>";
            Debug.LogError($"Did not find sound at: {soundPath}");
            return;
        }
        PlaySong (soundPath);
	}

	void Finish()
	{
		answer.text = "Finished !";
		remaining.text = "Remaining: " + nb.ToString ();
		carteImage.enabled = false;
		carte2Image.enabled = false;
		carte3Image.enabled = false;
        arrowNotFound.SetActive(false);
        arrowFound.SetActive(false);
    }

	public AudioClip SongLoadResources(string path)
	{
		AudioClip clp = (AudioClip)Resources.Load (path, typeof(AudioClip));
		return clp;
	}

    public void HideCard(bool hide)
    {
        revealButton.SetActive(hide);
        answer.gameObject.SetActive(!hide);
        carteImage.gameObject.SetActive(!hide);
        carte2Image.gameObject.SetActive(!hide);
        carte3Image.gameObject.SetActive(!hide);
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
        PauseIcon.transform.SetParent(carteImage.enabled ? card : card2);
        PauseIcon.GetComponent<PlayPause>().playing = !paused;
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

/******************************************************************\ */
/*Remarques
choix des Decks: Random deck

Menu de départ qui ne ressort pas assez
Rendre flou/baisser la saturation de l'image de fond durant le jeu
*/
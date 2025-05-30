using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandlerEX : MonoBehaviour {

	public List<Card> deck = new();

	public int nb;
	public int cardIndex;
	public string Main_Folder;
	public Transform card;
	public Transform card2;
	public Transform card3;
	public GameObject arrowFound;
	public GameObject arrowNotFound;
	public GameObject answer;
	public Text remaining;
    public Text startDelayText;
    public GameObject PauseIcon;
	public float back_speed_x;
	public float back_speed_y;
	public float tresh_x;
	public float tresh_y;
	public float turn_ratio;

	public float move_ratio;
	public float threshold;
	public float validate_x;
	public float y_diff_restart;

	float x0;
	float y0;

	float x_start_touch;
	float y_start_touch;

	float x3;
	public float x3_thres;
	float y3;
	bool songLoaded;

	Vector2 fingerStart = new(0,0);
	Vector2 fingerEnd = new(0,0);
    
    IEnumerator coroutine;
    bool hasMoved=false;
    
    bool paused;
    bool isStarting;
    float startClock;

    //Option menu variables
    bool autoplay;
    bool playpause;
    bool hidden;
    float startdelay;
    float pausesensitivity;

    string loadingSongPath;
    public string LoadingSongPath { get => loadingSongPath; }

    public Sprite defaultSprite;

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

    GameObject revealButton;
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
        hidden = PlayerPrefs.GetInt("hidden", 1) == 1;
        startdelay = PlayerPrefs.GetFloat("startdelay", 0);
        pausesensitivity = PlayerPrefs.GetFloat("pausesensitivity", 50);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        revealButton.SetActive(hidden);
        arrowNotFound.SetActive(true);
        arrowFound.SetActive(true);
        startDelayText.gameObject.SetActive(startdelay > 0);

		x0 = card.position.x;
		y0 = card.position.y;

        Main_Folder = Global.mainPath;
        Main_Folder =  PathManager.MainPath;

        flecheTrouveeImageColor = flecheTrouveeImage.color;
        flecheNonTrouveeImageColor = flecheNonTrouveeImage.color;
        flecheTrouveeTextColor = flecheTrouveeText.color;
        flecheNonTrouveeTextColor = flecheNonTrouveeText.color;

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
                if (!paused) PreparePause();
                else PreparePlay();
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
                    if (!paused && playpause) PreparePause();
                    if (paused) PreparePlay();
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

        char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        foreach (var character in forbiddenChar)
        {
            cardName = cardName.Replace(character, ',');
        }

        //for (int i = 0; i < cardName.Length; i++) {
        //
        //	char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
        //    foreach(var character in forbiddenChar)
        //    {
        //        cardName = cardName.Replace(character, ',');
        //    }
        //	int ind = HasIndex<char> (cardName [i],forbiddenChar);
        //	if(ind != -1)
        //	{
        //		cardName = cardName.Substring (0, i) + "," + cardName.Substring (i+ 1, cardName.Length-i-1);
        //		ind = HasIndex<char> (cardName [i], forbiddenChar);
        //
        //	}
        //
        //}

        //string imPath = Path.Combine(Main_Folder , "Visuels/" + cardName +".png");
        Debug.Log($"imPath: {Main_Folder}{Path.DirectorySeparatorChar}Packs{cardInfo.packId}Visuals{cardName +  ".png"}");
        string imPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Visuals", cardName +  ".png");
        Debug.Log($"imPath: {imPath}");

        answer.GetComponent<Text> ().text = cardName;

        if (hidden) HideCard(true);

        StartCoroutine(LoadImage(imPath, carte2Image));

        //string songPath = Path.Combine(Main_Folder, "Son/" + cardName + ".mp3");
        string soundPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Sounds", cardName + ".mp3");

        Debug.Log(soundPath);

        if (carte2Image.enabled)
        {
            PauseIcon.transform.SetParent(card2);
        }
        else
        {
            PauseIcon.transform.SetParent(card);
        }

        isStarting = startdelay > 0;
        paused = !autoplay || startdelay > 0;
        PauseIcon.GetComponent<PlayPause>().playing = autoplay;
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (autoplay)
        {
            Color pauseColor = PauseIcon.GetComponent<Image>().color;
            PauseIcon.GetComponent<Image>().color = new Color(pauseColor.r, pauseColor.g, pauseColor.b, 0);
        }

        if (!File.Exists(soundPath))
        {
            answer.GetComponent<Text>().text = cardName + "\n<color=red>Sound not found</color>";
            Debug.LogError($"Did not find sound at: {soundPath}");
            return;
        }

        if (startdelay > 0)
        {
            startClock = startdelay;
            startDelayText.gameObject.SetActive(true);
        }

        PlaySong (soundPath);
	}

	void Finish()
	{
		answer.GetComponent<Text> ().text = "Finished !";
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

    public void HideCard(bool reveal)
    {
        carteImage.gameObject.SetActive(reveal);
        carte2Image.gameObject.SetActive(reveal);
        carte3Image.gameObject.SetActive(reveal);
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
        //carteImage.preserveAspect = true;
        www.Dispose();

    }

    public void PlaySong(string path)
	{
		songLoaded=false;
		loadingSongPath = path;
        
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
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
        PreparePause();
    }

    void PlayClip()
    {
        source.Play();
        PreparePlay();
    }

    void PreparePlay() => SetPause(false);
    void PreparePause() => SetPause(true);

    void SetPause(bool paused)
    {
        this.paused = paused;
        if (carteImage.enabled)
        {
            PauseIcon.transform.SetParent(card);
        }
        else
        {
            PauseIcon.transform.SetParent(card2);
        }
        PauseIcon.GetComponent<PlayPause>().playing = !paused;
        PauseIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

/******************************************************************\ */
/*
affichage du nombre de cartes restantes
choix des Decks: Random deck

Remarques:
2 cartes à la fin

On peut sélectionner 2 fois le même deck: permettre de choisir de bloquer ça

Menu de départ qui ne ressort pas assez
Rendre flou/baisser la saturation de l'image de fond durant le jeu
*/
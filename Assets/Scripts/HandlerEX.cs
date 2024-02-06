using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandlerEX : MonoBehaviour {

	public List<Card> deck = new List<Card>();

	public int nb;
	public int card;
	public string Main_Folder;
	public GameObject carte;
	public GameObject carte2;
	public GameObject carte3;
	public GameObject flecheTrouvee;
	public GameObject flecheNonTrouvee;
	public GameObject reponse;
	public GameObject restantes;
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
	Vector2 fingerStart = new Vector2(0,0);
	Vector2 fingerEnd = new Vector2 (0,0);
    IEnumerator coroutine;
    bool hasMoved=false;
	AudioSource source;
    bool paused;

    bool autoplay;
    bool playpause;
    bool startedPlaying;
    public bool StartedPlaying
    {
        get => startedPlaying;
    }
    string loadingSongPath;
    public string LoadingSongPath
    {
        get => loadingSongPath;
    }

    public Sprite defaultSprite;

    Image carteImage;
    Image carte2Image;
    Image carte3Image;
    Image flecheTrouveeImage;
    Image flecheNonTrouveeImage;

    Text flecheTrouveeText;
    Text flecheNonTrouveeText;


    void Start () {



        carteImage = carte.GetComponent<Image>();
        carte2Image = carte2.GetComponent<Image>();
        carte3Image = carte3.GetComponent<Image>(); ;
        flecheTrouveeImage = flecheTrouvee.GetComponent<Image>();
        Debug.Log($"Fleche trouvee image is null ? {flecheTrouveeImage == null}");
        flecheNonTrouveeImage = flecheNonTrouvee.GetComponent<Image>();
        flecheTrouveeText = flecheTrouvee.GetComponentInChildren<Text>(true);
        flecheNonTrouveeText = flecheNonTrouvee.GetComponentInChildren<Text>(true);

        carteImage.enabled = false;
        carte2Image.enabled = true;

        autoplay = PlayerPrefs.GetInt("autoplay", 1) == 1;
        playpause = PlayerPrefs.GetInt("playpause", 1) == 1;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        flecheNonTrouvee.SetActive(true);
        flecheTrouvee.SetActive(true);

		x0 = carte.transform.position.x;
		y0 = carte.transform.position.y;
		source = GetComponent<AudioSource> ();

        Main_Folder = Global.mainPath;

        Main_Folder =  Application.persistentDataPath;
        


		ReadDecks ();
		First ();
		restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();

    }
	
	// Update is called once per frame
	void Update () {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(songLoaded)
            {
                if (source.isPlaying)
                {
                    PauseClip();
                }
                else
                {
                    PlayClip();
                }
            }
            else
            {
                if (!paused)
                {
                    PreparePause();
                }
                else
                {
                    PreparePlay();
                }
            }
            
        }
		swipeDetect ();
		carte.transform.rotation=Quaternion.Euler(new Vector3(0,0,carte.transform.position.x-x0)*turn_ratio);

		
		Vector2 pos = carte.transform.position;
		if (pos.x >= x0) {
			Color col = flecheTrouveeImage.color;
			flecheTrouveeImage.color= new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));
			col = flecheTrouveeText.color;
			flecheTrouveeText.color = new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));

			col = flecheNonTrouveeImage.color;
			flecheNonTrouveeImage.color= new Color(col.r,col.g,col.b,0);
			col = flecheNonTrouveeText.color;
			flecheNonTrouveeText.color = new Color(col.r,col.g,col.b,0);

		}
		if (pos.x <= x0) {
			Color col = flecheNonTrouveeImage.color;
			flecheNonTrouveeImage.color= new Color(col.r,col.g,col.b,Math.Min(1,(x0-pos.x)/validate_x));
			col = flecheNonTrouveeText.color;
			flecheNonTrouveeText.color = new Color(col.r,col.g,col.b,Math.Min(1,(x0 - pos.x)/validate_x));

			col = flecheTrouveeImage.color;
			flecheTrouveeImage.color= new Color(col.r,col.g,col.b,0);
			col = flecheTrouveeText.color;
			flecheTrouveeText.color = new Color(col.r,col.g,col.b,0);
		}


		if (carte3Image.enabled) {
			x3 = carte3.transform.position.x;
			y3 = carte3.transform.position.y;
			if (x3 < x0 && x3 > x0 - x3_thres || x3 >= x0 && x3 < x0 + x3_thres) {


				carte3.transform.position = new Vector2 (x3 + (x3 - x0) * back_speed_x, y3);
				carte3.transform.rotation=Quaternion.Euler(new Vector3(0,0,carte3.transform.position.x-x0)*turn_ratio);


			} else {
				carte3Image.enabled = false;
			}

		}


		

	}


    public Vector2 swipeDetect()
    {
        if (Input.GetMouseButtonDown(0)) //Start touch
        {
            hasMoved = false;
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = carte.transform.position.x;
            y_start_touch = carte.transform.position.y;
        }
        else if (Input.GetMouseButtonUp(0)) //End touch
        {
            float delta_x = fingerStart.x - fingerEnd.x;
            float delta_y = fingerStart.y - fingerEnd.y;

            float distance = Vector2.Distance(fingerStart, fingerEnd);
            if (distance <= 10 * PlayerPrefs.GetFloat("pausesensitivity", 50))
            {
                hasMoved = false;
            }

            bool changedCard = LetCard();

            if (!hasMoved && !changedCard)
            {
                if (songLoaded) //
                {

                    if (source.isPlaying && playpause)
                    {
                        PauseClip();
                    }
                    else if (paused)
                    {
                        PlayClip();
                    }
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
                    if (!paused && playpause)
                    {
                        PreparePause();
                    }
                    if (paused)
                    {
                        PreparePlay();
                    }
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

			Vector2 pos = carte.transform.position;

			carte3Image.enabled = true;
			carte3Image.preserveAspect = true;
			carte3Image.sprite = carteImage.sprite;
			carte3.transform.position = new Vector2(pos.x,pos.y);
			carte3.transform.rotation =Quaternion.Euler(new Vector3(0,0,carte.transform.rotation.z));


			carteImage.enabled = false;
			carte.transform.position = new Vector2(x0,y0);
			carte.transform.rotation =Quaternion.Euler(new Vector3(0,0,0));

			carte2Image.enabled = true;
			carte2.transform.position = new Vector2(x0,y0-y_diff_restart);
			carte2.transform.rotation =Quaternion.Euler(new Vector3(0,0,0));

		}
		GameObject temp=carte;
		carte = carte2;
		carte2 = temp;

        Image tempImage = carteImage;
        carteImage = carte2Image;
        carte2Image = tempImage;

        carteImage.enabled = false;
        while (carteImage.sprite == null)
        {
            yield return null;
        }
        carte.transform.position = new Vector2(x0, y0 - y_diff_restart);
        carteImage.enabled = true;


        PauseIcon.transform.SetParent(carte.transform);

        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;

    }
	void HoldCard(float d_x, float d_y)
	{
		
		Vector2 pos = carte.transform.position;
		pos = new Vector2 (x_start_touch + d_x, y_start_touch + d_y);
		carte.transform.position=pos;

	}

	bool LetCard()//returns if when to another card
	{
		Vector2 pos = carte.transform.position;
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
		carte.transform.position=pos;

		if (carte.transform.position.x > x0 + validate_x) { //Card found
			Trouvee();
            return true;
		}
		if (carte.transform.position.x < x0 - validate_x) { //Card found
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
		if (nb > 0) {

            deck.RemoveAt(card);
			//Deck [card] = Deck [nb - 1];
			nb--;
		}

		Next ();
	}


	void First() //Next, but without the swap
	{
		if (nb > 0) {
			card = UnityEngine.Random.Range (0, nb);
			Playcard (card);
            SwapCards();
        } else {

			Finish ();
		}
	}



	void Next()
	{

		if (nb > 0) {
			restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();
			card = UnityEngine.Random.Range (0, nb);
			Playcard (card);
			Resources.UnloadUnusedAssets ();
            SwapCards();
        } else {

			Finish ();
		}
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

        reponse.GetComponent<Text> ().text = cardName;



        StartCoroutine(LoadImage(imPath, carte2Image));

        //string songPath = Path.Combine(Main_Folder, "Son/" + cardName + ".mp3");
        string soundPath = Path.Combine(Main_Folder, "Packs", cardInfo.packId, "Sounds", cardName + ".mp3");

        Debug.Log(soundPath);
        if (!autoplay)
        {
            paused = true;
            if (carte2Image.enabled)
            {
                PauseIcon.transform.SetParent(carte2.transform);
            }
            else
            {
                PauseIcon.transform.SetParent(carte.transform);
            }

            PauseIcon.GetComponent<PlayPause>().playing = false;
            PauseIcon.transform.localPosition = Vector3.zero;
            PauseIcon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            paused = false;
            if (carte2Image.enabled)
            {
                PauseIcon.transform.SetParent(carte2.transform);
            }
            else
            {
                PauseIcon.transform.SetParent(carte.transform);
            }

            PauseIcon.GetComponent<PlayPause>().playing = true;
            Color pauseColor = PauseIcon.GetComponent<Image>().color;
            PauseIcon.GetComponent<Image>().color = new Color(pauseColor.r, pauseColor.g, pauseColor.b, 0);
            PauseIcon.transform.localPosition = Vector3.zero;
            PauseIcon.transform.localRotation = Quaternion.identity;
        }
            

        if(!File.Exists(soundPath))
        {
            reponse.GetComponent<Text>().text = cardName + "\n<color=red>Sound not found</color>";
            Debug.LogError($"Did not find sound at: {soundPath}");
            return;
        }
        PlaySong (soundPath);////////////////
		

	}


	void Finish()
	{
		reponse.GetComponent<Text> ().text = "Partie terminee !";
		restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();
		carteImage.enabled = false;
		carte2Image.enabled = false;
		carte3Image.enabled = false;
        flecheNonTrouvee.SetActive(false);
        flecheTrouvee.SetActive(false);
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
		if (source.clip.loadState==AudioDataLoadState.Loaded && !paused) {
            PlayClip();
		}
        else if(source.clip.loadState == AudioDataLoadState.Loaded && paused) //!autoplay && !source.isPlaying)
        {
            PauseClip();
        }

        www.Dispose();

    }


    



    int HasIndex<T>(T value, T[] array)
	{
		for (int i = 0; i < array.Length; i++) {
			if (array [i].Equals(value)) {

				return i;
			}

		}
		return -1;

	}

    void PauseClip()
    {
        source.Pause();
        paused = true;
        if (carteImage.enabled)
        {
            PauseIcon.transform.SetParent(carte.transform);
        }
        else
        {
            PauseIcon.transform.SetParent(carte2.transform);
        }
        
        PauseIcon.GetComponent<PlayPause>().playing = false;
        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;
    }
    void PlayClip()
    {
        source.Play();
        paused = false;
        if (carteImage.enabled)
        {
            PauseIcon.transform.SetParent(carte.transform);
        }
        else
        {
            PauseIcon.transform.SetParent(carte2.transform);
        }
        PauseIcon.GetComponent<PlayPause>().playing = true;
        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;
    }
    void PreparePlay()
    {
        paused = false;
        if (carteImage.enabled)
        {
            PauseIcon.transform.SetParent(carte.transform);
        }
        else
        {
            PauseIcon.transform.SetParent(carte2.transform);
        }

        PauseIcon.GetComponent<PlayPause>().playing = true;
        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;
    }
    void PreparePause()
    {
        paused = true;
        if (carteImage.enabled)
        {
            PauseIcon.transform.SetParent(carte.transform);
        }
        else
        {
            PauseIcon.transform.SetParent(carte2.transform);
        }
        PauseIcon.GetComponent<PlayPause>().playing = false;
        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;
    }


}


/******************************************************************\ */
 

/*
affichage du nombre de cartes restantes

choix des Decks: Random deck

Fond d'écran*
ok
Affichage des flêches trouvée et non trouvée lorque l'on déplace une carte (alpha --> 1)
ok
écrire nom anime
ok
streaming à partir de la v0.7
ok


Remarques:
2 cartes à la fin

On peut sélectionner 2 fois le même deck: permettre de choisir de bloquer ça

Menu de départ qui ne ressort pas assez
Rendre flou/baisser la saturation de l'image de fond durant le jeu

*/
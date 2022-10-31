using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class HandlerEX : MonoBehaviour {



	public string[] Deck = new string[1000];

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
	bool a;
	bool songLoaded;
	string loadingSongPath;
	Vector2 fingerStart = new Vector2(0,0);
	Vector2 fingerEnd = new Vector2 (0,0);
    IEnumerator coroutine;
    bool CR_running;
    bool hasMoved=false;
	AudioSource source;
    bool paused;

    bool autoplay;
    bool playpause;
    bool startedPlaying;
    // Use this for initialization

    public Sprite defaultSprite;


	void Start () {
        autoplay = PlayerPrefs.GetInt("autoplay", 1) == 1;
        playpause = PlayerPrefs.GetInt("playpause", 1) == 1;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        flecheNonTrouvee.SetActive(true);
        flecheTrouvee.SetActive(true);
        carte.GetComponent<Image>().enabled=false;
		carte2.GetComponent<Image>().enabled=true;

		x0 = carte.GetComponent<RectTransform> ().position.x;
		y0 = carte.GetComponent<RectTransform> ().position.y;
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
		//carte.GetComponent<RectTransform> ().rotation.Set(0,0,(carte.GetComponent<Transform> ().position.x-x0)*turn_ratio,0);
		carte.GetComponent<RectTransform> ().rotation=Quaternion.Euler(new Vector3(0,0,carte.GetComponent<Transform> ().position.x-x0)*turn_ratio);

		
		Vector2 pos = carte.GetComponent<Transform> ().position;
		if (pos.x >= x0) {
			Color col = flecheTrouvee.GetComponent<Image> ().color;
			flecheTrouvee.GetComponent<Image> ().color= new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));
			col = flecheTrouvee.GetComponentInChildren<Text> ().color;
			flecheTrouvee.GetComponentInChildren<Text> ().color = new Color(col.r,col.g,col.b,Math.Min(1,(pos.x - x0)/validate_x));

			col = flecheNonTrouvee.GetComponent<Image> ().color;
			flecheNonTrouvee.GetComponent<Image> ().color= new Color(col.r,col.g,col.b,0);
			col = flecheNonTrouvee.GetComponentInChildren<Text> ().color;
			flecheNonTrouvee.GetComponentInChildren<Text> ().color = new Color(col.r,col.g,col.b,0);

		}
		if (pos.x <= x0) {
			Color col = flecheNonTrouvee.GetComponent<Image> ().color;
			flecheNonTrouvee.GetComponent<Image> ().color= new Color(col.r,col.g,col.b,Math.Min(1,(x0-pos.x)/validate_x));
			col = flecheNonTrouvee.GetComponentInChildren<Text> ().color;
			flecheNonTrouvee.GetComponentInChildren<Text> ().color = new Color(col.r,col.g,col.b,Math.Min(1,(x0 - pos.x)/validate_x));

			col = flecheTrouvee.GetComponent<Image> ().color;
			flecheTrouvee.GetComponent<Image> ().color= new Color(col.r,col.g,col.b,0);
			col = flecheTrouvee.GetComponentInChildren<Text> ().color;
			flecheTrouvee.GetComponentInChildren<Text> ().color = new Color(col.r,col.g,col.b,0);
		}


		if (carte3.GetComponent<Image> ().enabled) {
			x3 = carte3.GetComponent<RectTransform> ().position.x;
			y3 = carte3.GetComponent<RectTransform> ().position.y;
			if (x3 < x0 && x3 > x0 - x3_thres || x3 >= x0 && x3 < x0 + x3_thres) {


				carte3.GetComponent<RectTransform> ().position = new Vector2 (x3 + (x3 - x0) * back_speed_x, y3);
				carte3.GetComponent<RectTransform> ().rotation=Quaternion.Euler(new Vector3(0,0,carte3.GetComponent<Transform> ().position.x-x0)*turn_ratio);


			} else {
				carte3.GetComponent<Image> ().enabled = false;
			}

		}


		

	}
    /*
	public bool pushDetect()
	{
		return Input.touchCount>0 &&Input.GetTouch (0).phase == TouchPhase.Ended && swipeDetect ().x < threshold;
	}*/

    public Vector2 swipeDetect()
    {
        if (Input.GetMouseButtonDown(0)) //Start touch
        {
            hasMoved = false;
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = carte.GetComponent<Transform>().position.x;
            y_start_touch = carte.GetComponent<Transform>().position.y;
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


    /*
	public Vector2 swipeDetect()
	{

		if (Input.touches.Length == 0) {

			LetCard ();

		}
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began) 
			{
                hasMoved = false;
				fingerStart = touch.position;
				fingerEnd  = touch.position;
				x_start_touch=carte.GetComponent<Transform> ().position.x;
				y_start_touch=carte.GetComponent<Transform> ().position.y;

			}
			if (touch.phase == TouchPhase.Moved )	
			{
                hasMoved = true;
                fingerEnd = touch.position;
				float delta_x=fingerEnd.x-fingerStart.x;
				float delta_y=fingerEnd.y-fingerStart.y;
				HoldCard (delta_x, delta_y);
			}
			if(touch.phase == TouchPhase.Ended)	
			{
				float delta_x=fingerStart.x-fingerEnd.x;
				float delta_y=fingerStart.y-fingerEnd.y;

				LetCard ();
                if(!hasMoved)
                {
                    if(songLoaded)
                    {
                        if (source.isPlaying && playpause)
                        {
                            //source.Pause();
                            PauseClip();
                            
                        }
                        else
                        {
                            if (source.clip.loadState == AudioDataLoadState.Loaded && !source.isPlaying)
                            {
                                //source.Play();
                                PlayClip();
                            }
                        }
                    }
                    else
                    {
                        if (!paused && playpause)
                        {
                            //source.Pause();
                            
                            PreparePause();
                        }
                        if(paused)
                        {
                            
                            PreparePlay();
                        }
                    }
                    
                }

			}
		}
		return new Vector2(0,0);
	}
    */
    void SwapCards()
    {
        StartCoroutine(SwapCardsCoroutine());
    }
    IEnumerator SwapCardsCoroutine()
	{

		if (carte.GetComponent<Image> ().enabled) {

			Vector2 pos = carte.GetComponent<Transform> ().position;

			carte3.GetComponent<Image> ().enabled = true;
			carte3.GetComponent<Image> ().preserveAspect = true;
			carte3.GetComponent<Image> ().sprite = carte.GetComponent<Image> ().sprite;
			carte3.GetComponent<Transform> ().position = new Vector2(pos.x,pos.y);
			carte3.GetComponent<Transform> ().rotation =Quaternion.Euler(new Vector3(0,0,carte.GetComponent<Transform> ().rotation.z));


			carte.GetComponent<Image> ().enabled = false;
			carte.GetComponent<Transform> ().position = new Vector2(x0,y0);
			carte.GetComponent<Transform> ().rotation =Quaternion.Euler(new Vector3(0,0,0));

			carte2.GetComponent<Image> ().enabled = true;
			carte2.GetComponent<Transform> ().position = new Vector2(x0,y0-y_diff_restart);
			carte2.GetComponent<Transform> ().rotation =Quaternion.Euler(new Vector3(0,0,0));

		}
		GameObject temp=carte;
		carte = carte2;
		carte2 = temp;

        carte.GetComponent<Image>().enabled = false;
        while (carte.GetComponent<Image>().sprite == null)
        {
            yield return null;
        }
        carte.transform.position = new Vector2(x0, y0 - y_diff_restart);
        carte.GetComponent<Image>().enabled = true;


        PauseIcon.transform.SetParent(carte.transform);

        PauseIcon.transform.localPosition = Vector3.zero;
        PauseIcon.transform.localRotation = Quaternion.identity;

    }
	void HoldCard(float d_x, float d_y)
	{
		
		Vector2 pos = carte.GetComponent<RectTransform> ().position;
		pos = new Vector2 (x_start_touch + d_x, y_start_touch + d_y);
		carte.GetComponent<RectTransform> ().position=pos;
		//carte.GetComponent<RectTransform> ().position = new Vector2 ();

	}

	bool LetCard()//returns if when to another card
	{
		Vector2 pos = carte.GetComponent<RectTransform> ().position;
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
		carte.GetComponent<RectTransform> ().position=pos;

		if (carte.GetComponent<RectTransform> ().position.x > x0 + validate_x) { //Card found
			Trouvee();
            return true;
		}
		if (carte.GetComponent<RectTransform> ().position.x < x0 - validate_x) { //Card found
			Next();
            return true;
        }
        return false;

	}
		
	void ReadDecks()
	{
		Deck = Global.Deck;
		nb = Global.entries;
	}


	void Trouvee()
	{
		if (nb > 0) {
			Deck [card] = Deck [nb - 1];
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
		string cardName = Deck [c];

		for (int i = 0; i < cardName.Length; i++) {

			char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
			int ind = hasIndex<char> (cardName [i],forbiddenChar);
			if(ind != -1)
			{
				cardName = cardName.Substring (0, i) + "," + cardName.Substring (i+ 1, cardName.Length-i-1);
				ind = hasIndex<char> (cardName [i], forbiddenChar);

			}

		}
		string imPath = Path.Combine(Main_Folder , "Visuels/" + cardName +".png");

		reponse.GetComponent<Text> ().text = cardName;



        StartCoroutine(LoadImage(imPath, carte2.GetComponent<Image>()));
        /*
		Sprite spr = imLoad (imPath);



		if (spr != null) {
			carte.GetComponent<Image> ().sprite = spr;
			carte.GetComponent<Image> ().preserveAspect = true;
		} else {
			
			imPath = Path.Combine(Main_Folder , "Visuels/Animint.png");

            carte.GetComponent<Image>().sprite = imLoad(imPath);
			carte.GetComponent<Image> ().preserveAspect = true;
			//Debug.Log ("Pas de sprite :(");
		}*/
        
		string songPath = Path.Combine(Main_Folder, "Son/" + cardName + ".mp3");


		//songPath = "Son/Pokemon Bleu";
		Debug.Log(songPath);
        /*
		AudioClip clp = SongLoad (songPath);
		if (clp != null) {
			Debug.Log ("Sound loading ?");
			source.clip = clp;
			source.Play ();

		} else {
			Debug.Log ("No sound loaded !");

		}*/

        //paused = !autoplay;

        ////////////////////////////////////////////////////////PLAYPAUSEGLITCH
        if (!autoplay)
        {
            paused = true;
            if (carte2.GetComponent<Image>().enabled)
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
            if (carte2.GetComponent<Image>().enabled)
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
            


        PlaySong (songPath);////////////////
		

	}


	void Finish()
	{
		reponse.GetComponent<Text> ().text = "Partie terminee !";
		restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();
		carte.GetComponent<Image> ().enabled = false;
		carte2.GetComponent<Image> ().enabled = false;
		carte3.GetComponent<Image> ().enabled = false;
        flecheNonTrouvee.SetActive(false);
        flecheTrouvee.SetActive(false);
    }

	public AudioClip SongLoadResources(string path)
	{
		AudioClip clp = (AudioClip)Resources.Load (path, typeof(AudioClip));
		return clp;
	}





	public Sprite imLoadResources(string path)
	{
		Debug.Log ("path image: " + path);
		Sprite spr = (Sprite)Resources.Load (path, typeof(Sprite));
		return spr;
	
	
	}

	public Sprite imLoad(string path)
	{

		Debug.Log ("Looking into " + path);
		if (!File.Exists (path)) {
            path = Path.Combine(Main_Folder, "Visuels/Animint.png");
        }
		
		byte[] bytes = File.ReadAllBytes(path);



		Texture2D texture = new Texture2D(1, 1);
		texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(bytes);
        return null;
        //return Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f,0.0f), 1.0f);


    }
    IEnumerator LoadImage(string path, Image image)/*IEnumerator*/
    {
        image.sprite = null;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        Debug.Log("<color=yellow>file://" + path+"</color>");
        yield return www.SendWebRequest();

        Debug.Log($"Request for {path} is done ? {www.isDone} result: {www.result} error ? : {www.error ?? "null"} download handler done ? :{www.downloadHandler.isDone}");

        //((DownloadHandlerTexture)www.downloadHandler). = true;


        Texture2D texture = DownloadHandlerTexture.GetContent(www);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 1.0f);

        image.sprite = sprite != null ? sprite : defaultSprite;
        carte.GetComponent<Image>().preserveAspect = true;
        www.Dispose();

    }




    public void PlaySong(string path)
	{
		
		//AudioSource source = GetComponent<AudioSource> ();
		songLoaded=false;
		loadingSongPath = path;
        //www = new WWW("file://" + path);
        

        
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        

        coroutine = LoadSong(path);
        StartCoroutine(coroutine);
		//source.clip = LoadSong (path);

	}

	/*public AudioClip SongLoad(string path)
	{

		//path = Main_Folder + "son/Pokemon Bleu.wav";
		Debug.Log ("Looking for song at " + path);


        //www = new WWW("file://" + path);
        //www = new WWW("file:///" + path);
        AudioClip clip = www.GetAudioClip(false, false);

		return www.GetAudioClip(false, false);

		//return clp;
	}*/

	IEnumerator LoadSong(string path)/*IEnumerator*/
	{
        //Debug.Log (path);
        //www = new WWW("file://" + loadingSongPath);


        /*
        WWW www = new WWW("file:///" + path);
        yield return www.GetAudioClip(false, false);
        source.clip = www.GetAudioClip(false, false);
        */
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);

        yield return www.SendWebRequest();
        
        ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;


        source.clip = DownloadHandlerAudioClip.GetContent(www);
        
        //source.clip = www.GetAudioClip(false, false);
        songLoaded = true;
        //source.clip = www.GetAudioClip(false, false);
		if (source.clip.loadState==AudioDataLoadState.Loaded && !paused) {
			//source.Play ();
            //PauseIcon.GetComponent<PlayPause>().playing = true;
            PlayClip();
		}
        else if(source.clip.loadState == AudioDataLoadState.Loaded && paused) //!autoplay && !source.isPlaying)
        {
            //source.Pause();
            PauseClip();
            //PauseIcon.GetComponent<PlayPause>().playing = false;
        }

        www.Dispose();

    }


    



    int hasIndex<T>(T value, T[] array)
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
        if (carte.GetComponent<Image>().enabled)
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
        if (carte.GetComponent<Image>().enabled)
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
        if (carte.GetComponent<Image>().enabled)
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
        if (carte.GetComponent<Image>().enabled)
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
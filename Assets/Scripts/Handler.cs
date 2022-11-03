using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class Handler : MonoBehaviour {



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
	Vector2 fingerStart = new Vector2(0,0);
	Vector2 fingerEnd = new Vector2 (0,0);
	AudioSource source;
    bool hasMoved = false;
    bool paused;

    public bool autoplay;
    public bool playpause;
    bool startedPlaying;
    bool songLoaded;

	public bool StartedPlaying
    {
		get => startedPlaying;
    }

    

    void Start () {
        autoplay = PlayerPrefs.GetInt("autoplay", 1) == 1;
        playpause = PlayerPrefs.GetInt("playpause", 1) == 1;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        flecheNonTrouvee.SetActive(true);
        flecheTrouvee.SetActive(true);
        carte.GetComponent<Image>().enabled=true;
		carte2.GetComponent<Image>().enabled=false;

		x0 = carte.transform.position.x;
		y0 = carte.transform.position.y;
		source = GetComponent<AudioSource> ();
		Main_Folder = "";//Application.dataPath;

		ReadDecks ();
		First ();
		restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();
	}
	
	// Update is called once per frame
	void Update () {

        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (songLoaded)
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
			x3 = carte3.transform.position.x;
			y3 = carte3.transform.position.y;
			if (x3 < x0 && x3 > x0 - x3_thres || x3 >= x0 && x3 < x0 + x3_thres) {


				carte3.transform.position = new Vector2 (x3 + (x3 - x0) * back_speed_x, y3);
				carte3.transform.rotation=Quaternion.Euler(new Vector3(0,0,carte3.transform.position.x-x0)*turn_ratio);

			} else {
				carte3.GetComponent<Image> ().enabled = false;
			}

		}


		

	}

	public bool pushDetect()
	{
		return Input.touchCount>0 &&Input.GetTouch (0).phase == TouchPhase.Ended && swipeDetect ().x < threshold;
	}

    public Vector2 swipeDetect()
    {
        if(Input.GetMouseButtonDown(0)) //Start touch
        {
            hasMoved = false;
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = carte.transform.position.x;
            y_start_touch = carte.transform.position.y;
        }
        else if(Input.GetMouseButtonUp(0)) //End touch
        {
            float distance = Vector2.Distance(fingerStart, fingerEnd);
            if (distance <= 10 * PlayerPrefs.GetFloat("pausesensitivity", 50))
            {
                hasMoved = false;
            }
            LetCard();

            if (!hasMoved)
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
        else if(Input.GetMouseButton(0))
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
        return Vector2.zero;
    }
    void SwapCards()
	{
		if (carte.GetComponent<Image> ().enabled) {

			Vector2 pos = carte.transform.position;

			carte3.GetComponent<Image> ().enabled = true;
			carte3.GetComponent<Image> ().preserveAspect = true;
			carte3.GetComponent<Image> ().sprite = carte.GetComponent<Image> ().sprite;
			carte3.transform.position = new Vector2(pos.x,pos.y);
			carte3.transform.rotation =Quaternion.Euler(new Vector3(0,0,carte.transform.rotation.z));


			carte.GetComponent<Image> ().enabled = false;
			carte.transform.position = new Vector2(x0,y0);
			carte.transform.rotation =Quaternion.Euler(new Vector3(0,0,0));

			carte2.GetComponent<Image> ().enabled = true;
			carte2.transform.position = new Vector2(x0,y0-y_diff_restart);
			carte2.transform.rotation =Quaternion.Euler(new Vector3(0,0,0));

		}


		GameObject temp=carte;

		carte = carte2;
		carte2 = temp;


	}
	void HoldCard(float d_x, float d_y)
	{
		Vector2 pos = carte.transform.position;
		pos = new Vector2 (x_start_touch + d_x, y_start_touch + d_y);
		carte.transform.position=pos;
	}

	void LetCard()
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
		}
		if (carte.transform.position.x < x0 - validate_x) { //Card found
			Next();
		}


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
		} else {

			Finish ();
		}
	}



	void Next()
	{
		SwapCards ();

		if (nb > 0) {
			restantes.GetComponent<Text> ().text = "Cartes Restantes: " + nb.ToString ();
			card = UnityEngine.Random.Range (0, nb);
			Playcard (card);
			Resources.UnloadUnusedAssets ();
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
		string imPath = Main_Folder + "Visuels/" + cardName;

		reponse.GetComponent<Text> ().text = cardName;
		Sprite spr = imLoad (imPath);
		if (spr != null) {
			carte.GetComponent<Image> ().sprite = imLoad (imPath);
			carte.GetComponent<Image> ().preserveAspect = true;
		} else {
			
			imPath = Main_Folder + "Visuels/Animint";

			carte.GetComponent<Image> ().sprite = imLoad (imPath);
			carte.GetComponent<Image> ().preserveAspect = true;
		}

		string songPath = Main_Folder + "Son/" + cardName;

		Debug.Log(songPath);
		AudioClip clp = SongLoad (songPath);
		if (clp != null) {
            source.Stop();
            source.clip = clp;
		}

        if (!autoplay)
        {
            
            PauseClip();
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
        else
        {
            PlayClip();
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
            Color pauseColor = PauseIcon.GetComponent<Image>().color;
            PauseIcon.GetComponent<Image>().color = new Color(pauseColor.r, pauseColor.g, pauseColor.b, 0);
            PauseIcon.transform.localPosition = Vector3.zero;
            PauseIcon.transform.localRotation = Quaternion.identity;
        }
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

	public AudioClip SongLoad(string path)
	{
		AudioClip clp = (AudioClip)Resources.Load (path, typeof(AudioClip));
        songLoaded = true;
		return clp;
	}


	public Sprite imLoad(string path)
	{
		Debug.Log ("path image: " + path);
		Sprite spr = (Sprite)Resources.Load (path, typeof(Sprite));
		return spr;
	}

	public Sprite imLoad2(string path)
	{
		byte[] bytes = File.ReadAllBytes(path);
		Texture2D texture = new Texture2D(1, 1);
		texture.filterMode = FilterMode.Trilinear;
		texture.LoadImage(bytes);
		return Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f,0.0f), 1.0f);
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
        Debug.Log("Pausing");
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
        Debug.Log("Playing");
        source.Play();
        paused = false;
        songLoaded = true;
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
        Debug.Log("Pausing");
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
ok
Fond d'écran*
ok
Affichage des flêches trouvée et non trouvée lorque l'on déplace une carte (alpha --> 1)
ok
écrire nom anime
ok
streaming à partir de la v0.7
ok
Remarques: 2 cartes à la fin
ok
On peut sélectionner 2 fois le même deck: permettre de choisir de bloquer ça
ok

Menu de départ qui ne ressort pas assez

Rendre flou/baisser la saturation de l'image de fond durant le jeu

choix des Decks: Random deck

S'assurer que cartes restante et retour ne se chevauchent pas (max 1/2)
ok
bug carte trouvée/non trouvée à la fin
ok?

S'assurer du non chevauchement d'éléments sur d'autres écrans

try catch pour déplacement dans l'explorer
ok

    //couleur flèches trouvé/pas trouvéeFFC8D6
    //couleur texte trouvé/ non trouvéC251DA
*/

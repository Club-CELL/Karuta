using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HandlerEX0 : MonoBehaviour {


	//Environment.Environment.getExternalStorageDirectory();
	public string[] Deck = new string[1000];

	public string mainDirectory;

	public int nb;
	public int card;
	public string Main_Folder;
	public GameObject carte;
	public GameObject carte2;
	public GameObject carte3;
	public GameObject flecheTrouvee;
	public GameObject flecheNonTrouvee;
	public GameObject reponse;
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
	Vector2 fingerStart = new Vector2(0,0);
	Vector2 fingerEnd = new Vector2 (0,0);
	AudioSource source;
	string extension;
	// Use this for initialization


	void Start () {
		
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		carte.GetComponent<Image>().enabled=true;
		carte2.GetComponent<Image>().enabled=false;

		x0 = carte.GetComponent<RectTransform> ().position.x;
		y0 = carte.GetComponent<RectTransform> ().position.y;
		source = GetComponent<AudioSource> ();
		Main_Folder = "";//Application.dataPath;

		ReadDecks ();
		First ();
	}
	
	// Update is called once per frame
	void Update () {

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

	public bool pushDetect()
	{
		return Input.touchCount>0 &&Input.GetTouch (0).phase == TouchPhase.Ended && swipeDetect ().x < threshold;
	}

	public Vector2 swipeDetect()
	{
		

		if (Input.touches.Length == 0) {

			LetCard ();

		}
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began) 
			{
				fingerStart = touch.position;
				fingerEnd  = touch.position;
				x_start_touch=carte.GetComponent<Transform> ().position.x;
				y_start_touch=carte.GetComponent<Transform> ().position.y;

			}
			if (touch.phase == TouchPhase.Moved )	
			{
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

			}
		}
		return new Vector2(0,0);
	}


	void SwapCards()
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


	}
	void HoldCard(float d_x, float d_y)
	{
		
		Vector2 pos = carte.GetComponent<RectTransform> ().position;
		pos = new Vector2 (x_start_touch + d_x, y_start_touch + d_y);
		carte.GetComponent<RectTransform> ().position=pos;
		//carte.GetComponent<RectTransform> ().position = new Vector2 ();



		

	}

	void LetCard()
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
		}
		if (carte.GetComponent<RectTransform> ().position.x < x0 - validate_x) { //Card found
			Next();
		}


	}
		
	void ReadDecks()
	{
		/*
		Deck[0]="Ano Hana";
		Deck[1]="Barakamon";
		Deck[2]="Boku no Hero Academia";
		Deck[3]="Chuunikoi";
		Deck[4]="Dagashi Kashi";
		Deck[5]="God Only Knows";
		Deck[6]="Gurren Lagann";
		Deck[7]="Haifuri";
		Deck[8]="Hibike Euphonium";
		Deck[9]="Initial D";
		Deck[10]="Joshiraku";
		Deck[11]="Kancolle";
		Deck[12]="Kiznaiver";
		Deck[13]="K-ON";
		Deck[14]="Konosuba";
		Deck[15]="Love Hina";
		Deck[16]="Mawaru Penguindrum";
		Deck[17]="Mikakunin";
		Deck[18]="Nagi no Asukara";
		Deck[19]="New Game";
		Deck[20]="Nisekoi";
		Deck[21]="Non Non Biyori";
		Deck[22]="Oban Star Racers";
		Deck[23]="Re Zero";
		Deck[24]="Slayers";
		Deck[25]="Sword Art Online";
		Deck[26]="Tamako Love Story";
		Deck[27]="Tatami Galaxy";
		Deck[28]="Watamote";
		Deck[29]="Yuri on Ice";
		nb = 30;
		*/
		//TextAsset[] Decks=Resources.LoadAll<TextAsset>("Decks");

		Deck = Global.Deck;
		nb = Global.entries;
		//Deck = Decks [0].text.Split (new char[] { '\n' });

		/*char excessChar = Deck [0] [Deck [0].Length - 1];
		for (int i = 0; i < Deck.Length; i++) {
			Deck [i] = Deck [i].TrimEnd (excessChar);
		}*/


		//Deck[0]="Ano Hana";
		//Deck[1]="Barakamon";
		//nb = Deck.Length;
		//nb = Deck.Length;


		/*
		Debug.Log (Deck[0].Length);
		for (int i = 0; i < Deck [2].Length; i++) {
			Debug.Log (i.ToString () + ": char:" + Deck [2] [i]);// + Deck[2][i].Equals("Boku no Hero Academia"[i]).ToString());
		}*/
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
		string originalName = cardName;
		for (int i = 0; i < cardName.Length; i++) {

			char[] forbiddenChar = new char[] { ':', '/', '"', '*', '\\', '|', '?', '<', '>' };
			int ind = hasIndex<char> (cardName [i],forbiddenChar);
			if(ind != -1)
			{
				cardName = cardName.Substring (0, i) + "," + cardName.Substring (i+ 1, cardName.Length-i-1);
				ind = hasIndex<char> (cardName [i], forbiddenChar);

			}

		}
		string imPath = Main_Folder + "Visuels/" + cardName + ".png";

		reponse.GetComponent<Text> ().text = originalName;
		Sprite spr = imLoad (imPath);
		if (spr != null) {
			carte.GetComponent<Image> ().sprite = imLoad (imPath);
			carte.GetComponent<Image> ().preserveAspect = true;
		} else {
			
			imPath = Main_Folder + "Visuels/Animint.png";

			carte.GetComponent<Image> ().sprite = imLoad (imPath);
			carte.GetComponent<Image> ().preserveAspect = true;
			//Debug.Log ("Pas de sprite :(");
		}

		string songPath = Main_Folder + "Son/" + cardName + ".mp3";


		//songPath = "Son/Pokemon Bleu";
		Debug.Log(songPath);


		/*AudioClip clp = SongLoad (songPath);
		if (clp != null) {
			
			source.clip = clp;
			source.Play ();

		}*/


		PlaySong (songPath);
		

	}


	void Finish()
	{

	}

	public AudioClip SongLoad(string path)
	{
		AudioClip clp = (AudioClip)Resources.Load (path, typeof(AudioClip));
		return clp;
	}


	public Sprite imLoadResource(string path)
	{
		Debug.Log ("path image: " + path);
		Sprite spr = (Sprite)Resources.Load (path, typeof(Sprite));
		return spr;
	
	
	}



	public Sprite imLoad(string path)
	{

		Debug.Log ("looking for picture at" + path);

		if (!File.Exists(path)) {
			Debug.Log ("No picture at" + path);
			return null;
		}

		byte[] bytes = File.ReadAllBytes(path);
		Texture2D texture = new Texture2D(1, 1);
		texture.filterMode = FilterMode.Trilinear;
		texture.LoadImage(bytes);
		return Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f,0.0f), 1.0f);

	}







	



	public void PlaySong(string path)
	{
		
		//AudioSource source = GetComponent<AudioSource> ();
	
		LoadSong (path);
		Debug.Log (path);

		if (source.clip.isReadyToPlay) {


		}


	}

	IEnumerator LoadSong(string path)/*IEnumerator*/
	{
		Debug.Log (path);
		string path2 = Main_Folder + "/son/Pokemon Bleu.wav";
		WWW www = new WWW("file://" + path2);

		source.clip = www.GetAudioClip(false, false);

		while(!www.isDone)
			yield return www;




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
*/
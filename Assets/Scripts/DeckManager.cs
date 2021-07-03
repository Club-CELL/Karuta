using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
public class DeckManager : MonoBehaviour {



	TextAsset[] DecksFiles;
	string[][] Decks = new string[100][];
	public string[] decktester = new string[40];

	public static int joueur=1;
	public float sep_y;
	public float back_speed;
	public static GameObject titre;
	float x0;
	float y0;
	public float decalageDebut;
	float x00;
	float y00;
	public int marginUp=50;
	public int marginDown=50;
	public GameObject Arrow;
	Vector2 fingerStart;
	Vector2 fingerEnd;
	float x_start_touch;
	float y_start_touch;
	float localX0;
	//float localY0;


	// Use this for initialization
	void Start () {
		titre = GameObject.Find ("Titre");
		x0 = Arrow.GetComponent<Transform> ().position.x;
		y0 = Arrow.GetComponent<Transform> ().position.y;
		localX0 = Arrow.GetComponent<RectTransform> ().localPosition.x;
		//localY0 = Arrow.GetComponent<RectTransform> ().localPosition.y;

		x00=GetComponent<Transform> ().position.x;
		y00=GetComponent<Transform> ().position.x;

		ReadDecks ();
	}
	
	// Update is called once per frame
	void Update () {
		swipeDetect ();
	}


    /*
	public Vector2 swipeDetect()
	{


		if (Input.touches.Length == 0) {

			LetGo ();

		}
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began) 
			{
				fingerStart = touch.position;
				fingerEnd  = touch.position;
				x_start_touch=GetComponent<Transform> ().position.x;
				y_start_touch=GetComponent<Transform> ().position.y;

			}
			if (touch.phase == TouchPhase.Moved )	
			{
				fingerEnd = touch.position;
				float delta_x=fingerEnd.x-fingerStart.x;
				float delta_y=fingerEnd.y-fingerStart.y;
				Hold (delta_y);
			}
			if(touch.phase == TouchPhase.Ended)	
			{
				float delta_x=fingerStart.x-fingerEnd.x;
				float delta_y=fingerStart.y-fingerEnd.y;

				LetGo ();

			}
		}
		return new Vector2(0,0);
	}*/


    public Vector2 swipeDetect()
    {

        if(Input.GetMouseButtonDown(0))
        {
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = GetComponent<Transform>().position.x;
            y_start_touch = GetComponent<Transform>().position.y;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            float delta_x = fingerStart.x - fingerEnd.x;
            float delta_y = fingerStart.y - fingerEnd.y;

            LetGo();
        }
        else if(Input.GetMouseButton(0))
        {
            fingerEnd = Input.mousePosition;
            float delta_x = fingerEnd.x - fingerStart.x;
            float delta_y = fingerEnd.y - fingerStart.y;
            Hold(delta_y);
        }
        else
        {
            //Hold(Input.GetAxis("Mouse ScrollWheel"));
            LetGo();
        }
        return Vector2.zero;
    }


    void Hold(float d_y)
	{

		Vector2 pos = GetComponent<RectTransform> ().position;
		pos = new Vector2 (pos.x, y_start_touch + d_y);
		GetComponent<RectTransform> ().position=pos;
        //carte.GetComponent<RectTransform> ().position = new Vector2 ();
        
	}

	void LetGo()
	{
		float dy=0;
		Vector2 pos = GetComponent<RectTransform> ().position;
		int nb_arrows = DecksFiles.Length;

		GameObject c= GameObject.Find ("Canvas");

		float cy = c.GetComponent<Transform> ().position.y;

        CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
        //int hh=Mathf.FloorToInt(Screen.height * canvasScaler.scaleFactor);
        int hh = Screen.height;
        //int hh = Screen.height;

        int hhh = hh - marginUp;


		//float begin = x - x0;
		float size=(nb_arrows)* sep_y;

		//float max = Math.Max (hh / 2 - y0, -hh / 2 - y0  +size);
		//float min = Math.Min (-hh / 2 - y0  +size, hh/2-y0);
		float max = Math.Max (hhh  , size +marginDown );
		float min = Math.Min (size +marginDown, hhh );

		//max = max-localY0;
		//min = min-localY0;

		//max = cy;
		//min = cy-hhh;
		//x+x0-(nb_arrows - 1))<-hh/2 // x>
        

		if(pos.y<min)
		{
			dy = back_speed * (min-pos.y);
		}
		if(pos.y>max)
		{
			dy = -back_speed * (pos.y-max);
		}





		pos = new Vector2 (pos.x, pos.y +dy);
		GetComponent<RectTransform> ().position=pos;
	}








	public static void NextJoueur()
	{
		joueur++;
		if (joueur <= Global.nbJoueurs) {
			titre.GetComponent<Text> ().text = "Joueur " + joueur.ToString ();

		} else {

			SceneManager.LoadScene ("Game");
		}

	}

	void ReadDecks()
	{
        //Start Int
        DecksFiles = Resources.LoadAll<TextAsset>("Decks"); 

        
		for (int i = 0; i < DecksFiles.Length; i++) {
			Decks[i] = DecksFiles [i].text.Split ('\n');

		}
        //End Int


        

        //Deck = Decks [0].text.Split (new char[] { '\n' });
        Debug.Log ("Nombre de decks:" + DecksFiles.Length.ToString());
		//Debug.Log (Decks.Length);
		char excessChar = Decks [0][0] [Decks [0][0].Length - 1];
		for (int i = 0; i < DecksFiles.Length; i++) {
			for (int j = 0; j < Decks [i].Length; j++) {
				Decks [i][j] = Decks [i][j].TrimEnd (excessChar);
			}
		}

		for (int i = 0; i < DecksFiles.Length; i++) {
			int nb = Decks [i].Length;
			Debug.Log (i.ToString() + "-" + Decks [i].Length.ToString() + "-" + Decks [i] [0].ToString());
			//decktester = Decks [i];
			while (Decks[i] [nb - 1].TrimEnd(new char[] {' '}).Equals("")) {
				nb--;
			}

			string[] temp = Decks [i];
			Decks [i] = new string[nb];
			Array.Copy (temp,0, Decks [i],0, nb);
		}

		//Debug.Log (DecksFiles.Length);
		for (int i = 0; i < DecksFiles.Length; i++) {

			GameObject Deck = createButton (DecksFiles [i].name, x0, y0 - i * sep_y);
			Deck.GetComponent<ChoixDecks> ().Deck = Decks [i];
			Deck.GetComponent<ChoixDecks> ().nomDeck = DecksFiles [i].name;
		}

			
	}

	GameObject createButton(string text, float x, float y)
	{
		GameObject newBouton = Instantiate (Arrow) as GameObject;
		newBouton.GetComponent<RectTransform> ().SetParent (this.GetComponent<RectTransform> (),false);
		newBouton.GetComponent<RectTransform> ().position = new Vector3(x,y,0);

		newBouton.GetComponentInChildren<Text> ().text= text;


		newBouton.SetActive (true);
		return newBouton;
	}


}

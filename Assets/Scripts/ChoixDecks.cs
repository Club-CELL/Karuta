using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class ChoixDecks : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler {
	


	public float scaleTouch;

	public bool activated;

	float scale;
	public float scaleSpeed;

	float startScale;

	bool execution=false;

	public float finX;
	public float speedX;

	public string deckName;
	public bool added=false;
	//public string[] Deck=new string[1000];
	public Deck deck;


	float x0;
	Vector2 pos;
	public void OnPointerDown(PointerEventData eventdata)
	{
		activated = true;
	}
	public void OnPointerExit(PointerEventData eventdata)
	{

		if (!execution) {
			activated = false;
		}

	}
	public void OnPointerUp(PointerEventData eventdata)
	{
		if (activated) {
			execution = true;
		}
	}

	// Use this for initialization
	void Start () {
		x0 = transform.position.x;
		startScale=transform.localScale.x;
	}

	// Update is called once per frame
	void Update () {
		if (activated) {
			Activate ();
		} else {
			Desactivate ();
		}
		if (execution) {
			Execute ();
		}
	}



	void Activate()
	{
		scale=transform.localScale.x;
		if (scale < scaleTouch) {
			scale = Math.Min(scaleTouch,scale+scaleSpeed);
		}
		transform.localScale = new Vector3(scale,scale,scale);
	}
	void Desactivate()
	{
		scale=transform.localScale.x;
		if (scale > startScale) {
			scale = Math.Max(startScale,scale-scaleSpeed);
		}
		transform.localScale = new Vector3(scale,scale,scale);
	}
	void Execute()
	{

		if (!added) {
			Global.AddDeck(deck);

            //DeckManager.NextJoueur (); //Int
            DeckManagerEx.NextJoueur(); //Ex
            added = true;
		}

		pos = transform.position;
		if (pos.x - x0 < finX) {

			pos = new Vector2 (pos.x + speedX, pos.y);
			transform.position = pos;

		} else {
			if(PlayerPrefs.GetInt("mirror") != 0)
            {
                transform.position = new Vector2(x0, pos.y);
            }
			
			added = false;
			execution = false;
			activated = false;
		}


	}
}

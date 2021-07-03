using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class Retour : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler {



	public float scaleTouch;

	public bool activated;

	float scale;
	public float scaleSpeed;

	float startScale;

	bool execution=false;

	public float finX;
	public float speedX;
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
		x0 = GetComponent<Transform> ().position.x;
		startScale=GetComponent<Transform> ().localScale.x;
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
		scale=GetComponent<Transform> ().localScale.x;
		//Debug.Log (scale);
		if (scale < scaleTouch) {
			scale = Math.Min(scaleTouch,scale+scaleSpeed);
		}
		GetComponent<Transform> ().localScale = new Vector3(scale,scale,scale);
	}
	void Desactivate()
	{
		scale=GetComponent<Transform> ().localScale.x;
		if (scale > startScale) {
			scale = Math.Max(startScale,scale-scaleSpeed);
		}
		GetComponent<Transform> ().localScale = new Vector3(scale,scale,scale);
	}
	void Execute()
	{
		pos = GetComponent<Transform> ().position;
		if (pos.x - x0 > finX) {

			pos = new Vector2 (pos.x - speedX, pos.y);
			GetComponent<Transform> ().position = pos;

		} else {
			//DeckManager.joueur = 1;
			Global.Restart ();
			SceneManager.LoadScene ("ChoixJoueurs");
		}
	}
}

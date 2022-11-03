using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Bouton : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler {


	public float scaleTouch;

	public bool activated;

	float scale;
	public float scaleSpeed;

	float startScale;

	public bool execution=false;

	public float finX;
	public float speedX;
	float x0;
	Vector2 pos;
	public GameObject choix;
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
	public void Start () {
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
		//Debug.Log (scale);
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
	virtual public void Execute()
	{
		pos = transform.position;
		if (pos.x - x0 < finX) {

			pos = new Vector2 (pos.x + speedX, pos.y);
			transform.position = pos;
			
		} else {
            Global.mainPath = PlayerPrefs.GetString("mainpath", Global.mainPath);
			Global.nbJoueurs = choix.GetComponent<ChoixNbJoueurs> ().nbJoueurs;
			SceneManager.LoadScene ("ChoixDecks");
		}
	}
}

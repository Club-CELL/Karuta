using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class ChoixNbJoueurs : MonoBehaviour {


	public int nbJoueurs=2;
    
	Vector2 fingerStart;
	Vector2 fingerEnd;

	public GameObject choix;

	float x_start_touch;
	float y_start_touch;


	public float diff=200;
	public float range=50;

	public int maxJoueurs=4;

	public float thresh;

	public float back_speed;
	public int[] positions;
	float x0;

    public GameObject menu;
	// Use this for initialization
	void Start () {
		maxJoueurs = Global.maxJoueurs;
		x0 = transform.position.x;


		positions = new int[maxJoueurs];
		for (int i = 0; i < maxJoueurs; i++) {
			positions [i] = -200 + 200 * i;
		}
	}
	
	// Update is called once per frame
	void Update () {
        if(!menu.activeInHierarchy)
        {
            swipeDetect();
        }
		
	}

    public Vector2 swipeDetect()
    {

        if(Input.GetMouseButtonDown(0))
        {
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
            x_start_touch = GetComponent<RectTransform>().position.x;
            y_start_touch = GetComponent<RectTransform>().position.y;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            float delta_x = fingerStart.x - fingerEnd.x;
            float delta_y = fingerStart.y - fingerEnd.y;

            LetGo();
        }
        else if (Input.GetMouseButton(0))
        {
            fingerEnd = Input.mousePosition;
            float delta_x = fingerEnd.x - fingerStart.x;
            float delta_y = fingerEnd.y - fingerStart.y;
            Hold(delta_x);
        }
        else 
        {
            LetGo();
        }
        return new Vector2(0, 0);
    }

    void Hold(float d_x)
	{
		Vector2 pos = transform.position;
		pos = new Vector2 (x_start_touch + d_x, pos.y);
		transform.position=pos;
	}

	void LetGo()
	{
		Vector2 pos = transform.position;

		for(int i=0;i<maxJoueurs;i++)
		{
			if (Math.Abs (pos.x - (x0-positions [i])) < range) {
				nbJoueurs = i + 1;
			}

		}

		float dir = x0-positions [nbJoueurs - 1];

		float dx=0;
		if (pos.x <= dir) {
			dx = Math.Max (thresh, (dir-pos.x)*back_speed);
		}
		if (pos.x > dir) {
			dx = Math.Min (-thresh, (dir-pos.x)*back_speed);
		}

		pos = new Vector2 (pos.x + dx, pos.y);
		transform.position=pos;
	}







}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLimit : MonoBehaviour {



	public float min;

	public float max;

	int hh;
	// Use this for initialization
	void Start () {
		hh = Screen.height;
	}

	// Update is called once per frame
	void Update () {
		checkLimit ();
	}

	void checkLimit()
	{
		float dy = 0;
		Vector2 pos = GetComponent<Transform> ().position;
		if (pos.y>max) {
			dy = (max - pos.y);

		}
		if (pos.y < min) {
			dy = (min - pos.y);
		}

		GetComponent<Transform> ().position=new Vector2(pos.x, pos.y+dy);
	}
}

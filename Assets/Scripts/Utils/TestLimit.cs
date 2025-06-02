using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLimit : MonoBehaviour {

	public float min;
	public float max;

	void Update () {
		checkLimit ();
	}

	void checkLimit()
	{
		float dy = 0;
		Vector2 pos = transform.position;
		if (pos.y>max) {
			dy = (max - pos.y);
		}
		if (pos.y < min) {
			dy = (min - pos.y);
		}
		transform.position=new Vector2(pos.x, pos.y+dy);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayPause : MonoBehaviour {

    public bool playing;
    public Sprite play;
    public Sprite pause;
    public float disappearSpeed;
    public float alphaMax;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(playing)
        {
            Color c = GetComponent<Image>().color;
            GetComponent<Image>().sprite=play;
            GetComponent<Image>().color = new Color(c.r, c.g, c.b, Mathf.Max(0, c.a - disappearSpeed * Time.deltaTime));
        }
        else
        {
            GetComponent<Image>().sprite = pause;
            Color c = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(c.r, c.g, c.b, alphaMax);
        }
	}
}

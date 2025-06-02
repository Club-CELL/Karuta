using UnityEngine;
using UnityEngine.UI;
public class PlayPause : MonoBehaviour {

    public bool playing;
    public Sprite play;
    public Sprite pause;
    public float disappearSpeed;
    public float alphaMax;

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

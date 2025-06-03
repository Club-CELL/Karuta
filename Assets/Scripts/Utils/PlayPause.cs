using UnityEngine;
using UnityEngine.UI;

public class PlayPause : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float alphaMax;

    [Header("Sprites")]
    [SerializeField] private Sprite play;
    [SerializeField] private Sprite pause;

    private bool playing;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {

        if (playing)
        {
            Color c = image.color;
            image.color = new Color(c.r, c.g, c.b, Mathf.Max(0, c.a - disappearSpeed * Time.deltaTime));
        }
        else
        {
            Color c = image.color;
            image.color = new Color(c.r, c.g, c.b, alphaMax);
        }
    }


    public void ChangeState(bool state)
    {
        playing = state;
        image.sprite = playing ? play : pause;
    }
}

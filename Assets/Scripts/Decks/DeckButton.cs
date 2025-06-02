using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class DeckButton : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler
{
	[Header("Button Parameters")]
    [SerializeField] private float scaleTouch;
    [SerializeField] private float scaleSpeed;

    private Deck deck;
    private bool mirror;
    private bool selected;
    private float scale;
    private float startScale;
    private Text buttonText;

    void Awake ()
	{
        buttonText = GetComponentInChildren<Text>();
		mirror = PlayerPrefs.GetInt("mirror") != 0;
        startScale = transform.localScale.x;
    }

	void Update ()
	{
		if (selected) Select ();
		else Unselect ();
	}

    public void SetButtonInfo(string name, Deck deck)
    {
        this.deck = deck;
        buttonText.text = name;
    }

    public void OnPointerDown(PointerEventData eventdata)
    {
        selected = true;
    }
    public void OnPointerExit(PointerEventData eventdata)
    {
        selected = false;
    }
    public void OnPointerUp(PointerEventData eventdata)
    {
        if (selected) AddDeck();
    }

    void Select()
	{
		scale = transform.localScale.x;
		if (scale < scaleTouch)
		{
			scale = Math.Min(scaleTouch, scale + scaleSpeed);
		}
		transform.localScale = scale * Vector3.one;
    }

	void Unselect()
	{
		scale = transform.localScale.x;
		if (scale > startScale)
		{
			scale = Math.Max(startScale, scale - scaleSpeed);
		}
        transform.localScale = scale * Vector3.one;
	}

	void AddDeck()
	{
        selected = false;
        Global.AddDeck(deck);
        DeckPicker.NextPlayer();

        if (!mirror) gameObject.SetActive(false);
    }
}

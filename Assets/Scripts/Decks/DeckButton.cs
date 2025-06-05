using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using TMPro;

public class DeckButton : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler
{
	[Header("Button Parameters")]
    [SerializeField] private float scaleTouch;
    [SerializeField] private float scaleSpeed;

    private Deck deck;
    private bool mirror;

    private bool added;
    private bool selected;
    private float scale;
    private float startScale;

    private TextMeshProUGUI buttonText;
    private Image buttonImage;
    private Color baseColor;
    private Color activeColor;

    void Awake ()
	{
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
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

    public void SetColors(Color activeColor)
    {
        baseColor = buttonImage.color;
        this.activeColor = activeColor;
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

	public void AddDeck()
	{
        selected = false;

        if (!mirror)
        {
            added = !added;
            if (added)
            {
                Global.AddDeck(deck);
                buttonImage.color = activeColor;
            }
            else
            {
                Global.RemoveDeck(deck);
                buttonImage.color = baseColor;
            }
        }
        else
        {
            added = true;
            buttonImage.color = activeColor;
            Global.AddDeck(deck);
        }

        if (Global.gameMode == Global.GameModes.Classic)
        {
            if (added) DeckPicker.NextPlayer();
            else DeckPicker.PreviousPlayer();
        }
        else TrialDeckPicker.Instance.DeckChange();
    }
}

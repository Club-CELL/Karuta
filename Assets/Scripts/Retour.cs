using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class Retour : MonoBehaviour, IPointerUpHandler,IPointerExitHandler,IPointerDownHandler
{
    public float scaleTouch;
    public float scaleSpeed;

    float scale;
    float startScale;

	public float finX;
	public float speedX;

    private bool selected;
    private bool execute;
    float x0;
	Vector2 pos;

	void Start ()
	{
		x0 = transform.position.x;
		startScale=transform.localScale.x;
	}

	void Update ()
	{
		if (selected) Select();
		else Unselect();

        if (execute) Execute();
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
        if (selected) execute = true;
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

    void Execute()
	{
		pos = transform.position;
		if (pos.x - x0 > finX)
		{
			pos = new Vector2 (pos.x - speedX, pos.y);
			transform.position = pos;

		} else
		{
            selected = false;
            Global.Restart();
            SceneManager.LoadScene("MainMenu");
        }
	}
}

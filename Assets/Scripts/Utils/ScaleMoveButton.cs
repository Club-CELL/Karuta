using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class ScaleMoveButton : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler
{
	[Header("Animation parameters")]
	public float scaleTouch;
	public float scaleSpeed;
	[Space(10)]
    public float finX;
    public float speedX;

    [Header("Callbacks")]
    public UnityEvent onExecute;

    protected float startScale;
	protected float scale;

	protected float x0;
	protected Vector2 pos;

    protected bool selected;
    protected bool execution;
	protected bool activated;
	protected bool disabled;

	public void OnPointerDown(PointerEventData eventdata)
	{
		if (!disabled) selected = true;
	}

	public void OnPointerExit(PointerEventData eventdata)
	{
		if (!execution) selected = false;
	}

	public void OnPointerClick(PointerEventData eventdata)
	{
		if (selected && !activated) execution = true;
	}

	protected void Start () {
		x0 = transform.position.x;
		startScale = transform.localScale.x;
	}

	protected void Update () {
		if (selected) OnSelect();
		else Unselect();
		
		if (execution) LaunchExecution();
	}

    protected void OnSelect()
	{
		scale = transform.localScale.x;
		if (scale < scaleTouch) scale = Math.Min(scaleTouch,scale + scaleSpeed);
		
		transform.localScale = scale * Vector3.one;
	}

	protected void Unselect()
	{
		scale = transform.localScale.x;
		if (scale > startScale) scale = Math.Max(startScale, scale - scaleSpeed);
		else activated = false;
		
		transform.localScale = scale * Vector3.one;
    }

	public void LaunchExecution()
	{
		pos = transform.position;
		bool translate = (finX > 0 && pos.x - x0 < finX) || 
						 (finX < 0 && pos.x - x0 > finX);
        if (translate)
		{
			pos = new Vector2 (pos.x + speedX, pos.y);
			transform.position = pos;
			
		} else {
			Execute();
		}
	}

	virtual public void Execute()
	{
		onExecute.Invoke();
		execution = false;
		selected = false;
		activated = true;
    }
}

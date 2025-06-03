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

    protected bool hover;
    protected bool execution = false;
	protected bool activated;

	public void OnPointerDown(PointerEventData eventdata)
	{
		hover = true;
	}

	public void OnPointerExit(PointerEventData eventdata)
	{
		if (!execution) hover = false;
	}

	public void OnPointerClick(PointerEventData eventdata)
	{
		if (hover) execution = true;
	}

	protected void Start () {
		x0 = transform.position.x;
		startScale = transform.localScale.x;
	}

	protected void Update () {
		if (hover) OnHover ();
		else OnLeave ();
		
		if (execution && !activated) LaunchExecution();
	}

    protected void OnHover()
	{
		scale = transform.localScale.x;
		if (scale < scaleTouch) scale = Math.Min(scaleTouch,scale + scaleSpeed);
		
		transform.localScale = scale * Vector3.one;
	}

	protected void OnLeave()
	{
		scale = transform.localScale.x;
		if (scale > startScale) scale = Math.Max(startScale, scale - scaleSpeed);
		else activated = false;
		
		transform.localScale = scale * Vector3.one;
    }

	public void LaunchExecution()
	{
		pos = transform.position;
		if (pos.x - x0 < finX) {

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
		hover = false;
		activated = true;
    }
}

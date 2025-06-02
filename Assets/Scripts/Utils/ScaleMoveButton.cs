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

    protected bool activated;
    protected bool execution = false;

	public void OnPointerDown(PointerEventData eventdata)
	{
		activated = true;
	}
	public void OnPointerExit(PointerEventData eventdata)
	{
		if (!execution) {
			activated = false;
		}
	}
	public void OnPointerClick(PointerEventData eventdata)
	{
		if (activated) {
			execution = true;
		}
	}

	// Use this for initialization
	protected void Start () {
		x0 = transform.position.x;
		startScale=transform.localScale.x;
	}

	// Update is called once per frame
	protected void Update () {
		if (activated) {
			Activate ();
		} else {
			Desactivate ();
		}
		if (execution) {
			LaunchExecution();
		}
	}

    protected void Activate()
	{
		scale=transform.localScale.x;
		//Debug.Log (scale);
		if (scale < scaleTouch) {
			scale = Math.Min(scaleTouch,scale+scaleSpeed);
		}
		transform.localScale = new Vector3(scale,scale,scale);
	}

	protected void Desactivate()
	{
		scale=transform.localScale.x;
		if (scale > startScale) {
			scale = Math.Max(startScale,scale-scaleSpeed);
		}
		transform.localScale = new Vector3(scale,scale,scale);
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
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Bouton0 : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler {



	public string room = "Menu";

	public string direction="TestTest";


	public  int id;
	public float scaleOverTime=3f;
	public float scaleOverFrequency = 1f;
	bool scalingOver=false;
	float timeOver=0f;

	public float balanceOsc;//oscillations
	public float balanceAgg;//agrandissement
	public float overPlus=0.3f;
	float scaleFactor=1;
	bool isOver;
	public bool isSelected=false;
	public bool BTbutton=false;
	public bool categoryButton = false;
	public float startScale=0.5f;

	public Color startColor=new Color(1,1,1);
	public Color validatedColor=new Color(1,1,1);
	public bool changeColor=false;
	public void OnPointerDown(PointerEventData eventdata)
	{
		Debug.Log ("you clicked !");
		if (isSelected) {
			Execute ();
			isSelected = false;
		} 
		else {
			isSelected = true;
		}
		//GetComponent<Transform> ().localScale = new Vector3(2,2,2);
		//Debug.Log ("mouse enters");

	}
	public void OnPointerEnter(PointerEventData eventdata)
	{
		//scaleFactor += 0.3f;
		//GetComponent<Transform> ().localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
		isOver = true;
	}
	public void OnPointerExit(PointerEventData eventdatz)
	{
		//scaleFactor -= 0.3f;
		//GetComponent<Transform> ().localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
		isOver = false;
	}
	public void OnPointerDrag(PointerEventData eventdatz)
	{

	}


	public void scale()
	{

		scaleFactor = startScale;
		if (isSelected) {
			scaleFactor += balanceOsc * Mathf.Pow (Mathf.Cos (timeOver * scaleOverFrequency), 2);//+balanceAgg*timeOver/scaleOverTime;
		}
		if (isOver) {
			scaleFactor += overPlus;
		}
		GetComponent<Transform> ().localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
		scalingOver = true;

	}

	void OnMouseEnter()
	{
		scaleFactor += 0.3f;
		GetComponent<Transform> ().localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
	}
	void OnMouseExit()
	{
		scaleFactor -= 0.3f;
		GetComponent<Transform> ().localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
	}

	void OnMouseDown()
	{
		//GetComponent<Transform> ().localScale = new Vector3(2,2,2);

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



		if (scalingOver) {
			timeOver += Time.deltaTime;
		}
		scale ();

	}

	public virtual void Execute()
	{
		Debug.Log ("execute");
	}
}

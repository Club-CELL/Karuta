using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextOption : MonoBehaviour, IPointerUpHandler, IPointerExitHandler, IPointerDownHandler
{


    public GameObject Box;

    CheckBox checkbox;

    void Start()
    {
        checkbox = Box.GetComponent<CheckBox>();
    }

    public void OnPointerDown(PointerEventData eventdata)
    {
        checkbox.activated = true;
    }
    public void OnPointerExit(PointerEventData eventdata)
    {

        if (!checkbox.execution)
        {
            checkbox.activated = false;
        }

    }
    public void OnPointerUp(PointerEventData eventdata)
    {
        if (checkbox.activated)
        {
            checkbox.execution = true;
        }
    }
    /*
    // Use this for initialization
    
	
	// Update is called once per frame
	void Update () {
		
	}
    */
}

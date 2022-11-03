using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : Bouton {

    public Sprite boxOn;
    public Sprite boxOff;
    bool state;
    public string variable;
    public GameObject text;
    // Use this for initialization
	new void  Start () {
        base.Start();
        int a = PlayerPrefs.GetInt(variable, 1);

        if (a==1)
        {
            GetComponent<Image>().sprite = boxOn;
            state = true;
        }
        if (a==0)
        {
            GetComponent<Image>().sprite = boxOff;
            state = false;
        }
        text.GetComponent<Outline>().enabled = state;
    }

    public override void Execute()
    {
        state = !state;

        if(state)
        {
            GetComponent<Image>().sprite = boxOn;
            PlayerPrefs.SetInt(variable, 1);
        }
        if (!state)
        {
            GetComponent<Image>().sprite = boxOff;
            PlayerPrefs.SetInt(variable, 0);
        }
        text.GetComponent<Outline>().enabled = state;
        execution = false;
        activated = false;
    }
}

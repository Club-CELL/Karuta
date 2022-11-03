using System;
using UnityEngine;
using UnityEngine.UI;
public class ExceptionBox : MonoBehaviour {

    public float stayTime=10f;

    float timeBeing;
    float a = 1;
    private void OnEnable()
    {
        a = 1;
        timeBeing = 0;
    }

    void Update () {
		if(a>0)
        {
            timeBeing += Time.deltaTime;
            a = Math.Max(0, 1 - timeBeing / stayTime);
        }
           
        Color c = GetComponent<Image>().color;
        c.a = a;
        GetComponent<Image>().color = c;
        c = GetComponentInChildren<Text>().color;
        c.a = a;
        GetComponentInChildren<Text>().color=c;
        if(a<=0)
        {
            gameObject.SetActive(false);
        }
    }
}

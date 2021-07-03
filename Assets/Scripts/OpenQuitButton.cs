using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenQuitButton : Bouton {

    public bool open;
    public GameObject objectToOpenOrQuit;
    // Use this for initialization
    /*void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/

    public override void Execute()
    {
        objectToOpenOrQuit.SetActive(open);
        execution = false;
        activated = false;
    }
}

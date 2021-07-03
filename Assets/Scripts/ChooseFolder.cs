using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseFolder : OpenQuitButton {

	// Use this for initialization
	/*void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/

    public override void Execute()
    {
        base.Execute();
        PlayerPrefs.SetString("mainpath", Global.mainPath);
    }
}

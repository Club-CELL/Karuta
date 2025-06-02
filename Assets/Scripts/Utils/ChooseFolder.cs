using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseFolder : OpenQuitButton {

    public override void Execute()
    {
        base.Execute();
        PlayerPrefs.SetString("mainpath", Global.mainPath);
    }
}

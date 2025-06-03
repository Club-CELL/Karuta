using UnityEngine;

public class OpenQuitButton : ScaleMoveButton {

    public bool open;
    public GameObject objectToOpenOrQuit;
    public override void Execute()
    {
        base.Execute();
        objectToOpenOrQuit.SetActive(open);
        execution = false;
        hover = false;
    }
}

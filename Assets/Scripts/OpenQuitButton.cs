using UnityEngine;

public class OpenQuitButton : Bouton {

    public bool open;
    public GameObject objectToOpenOrQuit;
    public override void Execute()
    {
        objectToOpenOrQuit.SetActive(open);
        execution = false;
        activated = false;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Retour : ScaleMoveButton
{
    public override void Execute()
	{
		base.Execute();
        Global.Restart();
        SceneManager.LoadScene("MainMenu");
    }
}

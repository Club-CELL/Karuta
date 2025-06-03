using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNumberValidateButton : ScaleMoveButton {

	public ChoixNbJoueurs choix;

	override public void Execute()
	{
		Global.mainPath = PlayerPrefs.GetString("mainpath", Global.mainPath);
		Global.nbJoueurs = choix.PlayerCount();
		SceneManager.LoadScene("ChoixDecks");
	}
}
 
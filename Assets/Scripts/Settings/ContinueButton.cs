using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : ScaleMoveButton {

	[SerializeField] private ChoixNbJoueurs nbPlayer;
	[SerializeField] private TrialParamButton trialLength;
	[SerializeField] private TrialParamButton trialDifficulty;

	override public void Execute()
	{
		Global.mainPath = PlayerPrefs.GetString("mainpath", Global.mainPath);
		Global.nbJoueurs = nbPlayer.PlayerCount();
		Global.trialLength = trialLength.Param();
		Global.trialChoices = trialDifficulty.Param();

		if (Global.gameMode == Global.GameModes.Classic) SceneManager.LoadScene(1);
		else SceneManager.LoadScene(3);
	}
}
 
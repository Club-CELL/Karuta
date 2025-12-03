using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : ScaleMoveButton {

	[SerializeField] private ChoixNbJoueurs nbPlayer;
	[SerializeField] private TrialParamButton trialLength;
	[SerializeField] private TrialParamButton trialDifficulty;
	[SerializeField] private TrialParamButton trialLives;

    override public void Execute()
	{
		Global.mainPath = PlayerPrefs.GetString("mainpath", Global.mainPath);
		Global.nbJoueurs = nbPlayer.PlayerCount();
		Global.trialLength = trialLength.SetParamValue();
		Global.trialChoices = trialDifficulty.SetParamValue();
		Global.trialLives = trialLives.SetParamValue();

        if (Global.gameMode == Global.GameModes.Classic) SceneManager.LoadScene(1);
		else SceneManager.LoadScene(3);
	}
}
 
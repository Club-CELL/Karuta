using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PlayerNumberValidateButton : ScaleMoveButton {


	public GameObject choix;

	override public void Execute()
	{
		Global.mainPath = PlayerPrefs.GetString("mainpath", Global.mainPath);
		Global.nbJoueurs = choix.GetComponent<ChoixNbJoueurs>().nbJoueurs;
		SceneManager.LoadScene("ChoixDecks");
	}
}
 
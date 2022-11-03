using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {


    public static string mainPath;
	public static int nbJoueurs=2;
	public static int maxJoueurs=4;
	public static string[] Deck = new string[1000];
	public static int entries;

    public static Theme theme = null;

	public static void AddTabInDeck(string[] T)
	{
		for (int i = 0; i < T.Length; i++) {
			Deck [entries] = T [i];
			entries++;
		}
	}

	public static void Restart()
	{
        DeckManager.joueur = 1;//Int
		entries = 0;
	}

}

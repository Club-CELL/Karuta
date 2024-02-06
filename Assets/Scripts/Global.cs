using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
	public string packId;
	public string name;
	public Card(string packId, string name)
    {
		this.packId = packId;
		this.name = name;
    }
}
public class Global : MonoBehaviour {

	
    public static string mainPath;
	public static int nbJoueurs=2;
	public static int maxJoueurs=4;
	public static List<Card> deck = new List<Card>();
	//public static string[] Deck = new string[1000];
	
    public static Theme theme = null;

	public static void AddDeck(Deck newDeck)
	{
		for (int i = 0; i < newDeck.deck.Count;i++) {

			deck.Add(new Card(newDeck.packId, newDeck.deck[i]));
			//deck.Add(cards[i]);
			//Deck [entries] = T [i];
			//entries++;
		}
	}

	public static void Restart()
	{
        DeckManager.joueur = 1;//Int
		//entries = 0;
		deck.Clear();
	}

}

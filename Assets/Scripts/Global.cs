using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{	
    public static string mainPath;
	public static int nbJoueurs = 2;
	public static List<Card> deck = new();
    public static Theme theme = null;

	public static void AddDeck(Deck newDeck)
	{
		for (int i = 0; i < newDeck.cards.Count; i++)
		{
			deck.Add(new Card(newDeck.packId, newDeck.cards[i]));
		}
	}

	public static void RemoveDeck(Deck newDeck)
	{
		deck.RemoveRange(deck.Count - newDeck.cards.Count, newDeck.cards.Count);
	}

	public static void Restart()
	{
		deck.Clear();
	}
}

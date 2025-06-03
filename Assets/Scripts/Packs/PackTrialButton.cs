using System.Collections.Generic;
using UnityEngine;

public class PackTrialButton : MonoBehaviour
{
    private List<DeckButton> decks;

    private void OnEnable()
    {
        decks = new List<DeckButton>();
    }

    public void AddDeck(DeckButton deck)
    {
        decks.Add(deck);
    }

    public void AddDecks(Transform grid)
    {
        foreach(Transform deck in grid)
        {
            decks.Add(deck.GetComponent<DeckButton>());
        }
    }

    public void AddAllDecksOfPack()
    {
        foreach (DeckButton deck in decks)
        {
            deck.AddDeck();
        }
    }
}

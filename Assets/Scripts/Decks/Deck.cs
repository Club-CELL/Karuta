using System.Collections.Generic;

public class Deck
{
    public string packId;
    public List<string> cards = new();

    public Deck(string packId)
    {
        this.packId = packId;
        cards = new();
    }
}

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
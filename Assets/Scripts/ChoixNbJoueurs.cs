using UnityEngine;
using System;
using UnityEngine.UI;

public class ChoixNbJoueurs : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Text playerText;
    private int nbPlayer = 2;

    public int PlayerCount() => nbPlayer;

    void Start ()
	{
        playerText.text = nbPlayer.ToString();
	}

    public void AddPlayer()
    {
        if (nbPlayer >= maxPlayers) return;

        nbPlayer++;
        playerText.text = nbPlayer.ToString();
    }
    
    public void RemovePlayer()
    {
        if (nbPlayer <= 1) return;

        nbPlayer--;
        playerText.text = nbPlayer.ToString();
    }
}

using TMPro;
using UnityEngine;

public class LivesDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heart1;
    [SerializeField] private GameObject heart2;
    [SerializeField] private GameObject heart3;
    [SerializeField] private TextMeshProUGUI mult;
    [SerializeField] private GameObject infinity;

    public void UpdateLives(int lives)
    {
        if (lives == 99)
        {
            infinity.SetActive(true);
            mult.gameObject.SetActive(false);
        }
        else
        {
            infinity.SetActive(false);
            mult.gameObject.SetActive(true);
        }
        
        if (lives > 3)
        {
            mult.text = "x" + lives;
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        else
        {
            mult.text = "";
            heart1.SetActive(lives >= 1);
            heart2.SetActive(lives >= 2);
            heart3.SetActive(lives >= 3);
        }
    }
}

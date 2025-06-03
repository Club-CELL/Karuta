using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckPicker : MonoBehaviour
{
    public static DeckPicker Instance; 
    public static int player;

    [SerializeField] private DeckGridDisplay gridDisplay;
    [SerializeField] private DeckListDisplay listDisplay;
    
    private Text title;
    private bool grid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        title = GetComponent<Text>();
        grid = PlayerPrefs.GetInt("deckgrid", 1) == 1;
    }

    private void Start()
    {
        gridDisplay.gameObject.SetActive(grid);
        listDisplay.gameObject.SetActive(!grid);

        Global.mainPath = PathManager.MainPath;
        player = 1;

        if (grid) gridDisplay.ReadDecks();
        else listDisplay.ReadDecks(); 
    }

    public static void NextPlayer()
    {
        player++;

        if (player <= Global.nbJoueurs)
        {
            Instance.title.text = "Player " + player.ToString();
        }
        else SceneManager.LoadScene("Game");
    }

    public static void PreviousPlayer()
    {
        player--;
        Instance.title.text = "Player " + player.ToString();
    }
}

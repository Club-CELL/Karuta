using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrialDeckPicker : MonoBehaviour
{
    public static TrialDeckPicker Instance; 

    [SerializeField] private DeckGridDisplay gridDisplay;
    [SerializeField] private DeckListDisplay listDisplay;
    [SerializeField] private StartTrialButton startButton;
    
    private bool grid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        grid = PlayerPrefs.GetInt("deckgrid", 1) == 1;
    }

    private void Start()
    {
        gridDisplay.gameObject.SetActive(grid);
        listDisplay.gameObject.SetActive(!grid);

        Global.mainPath = PathManager.MainPath;

        if (grid) gridDisplay.ReadDecks();
        else listDisplay.ReadDecks(); 
    }

    public void DeckChange()
    {
        startButton.UpdateButton();
    }
}

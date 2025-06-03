using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartTrialButton : ScaleMoveButton
{
    [SerializeField] private Color disabledColor;
    private Color activeColor;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = disabledColor;
    }


    public void SetActiveColor(Color color)
    {
        activeColor = color;
    }

    public void UpdateButton()
    {
        image.color = Global.deck.Count >= Global.trialLength ? activeColor : disabledColor;
    }

    public override void Execute()
    {
        if (Global.deck.Count >= Global.trialLength)
        {
            SceneManager.LoadScene(4);
        }
    }
}

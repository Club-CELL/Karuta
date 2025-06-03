using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeButton : ScaleMoveButton
{
    [SerializeField] private Global.GameModes gameMode;
    [SerializeField] private GameModeButton otherButton;
    [SerializeField] private List<GameObject> gameModeParameters;

    private bool state; 
    private Color activeColor;
    private Color inactiveColor;
    private Image image;

    public Global.GameModes GameMode => gameMode;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Set(Color active, Color inactive, bool state)
    {
        activeColor = active;
        inactiveColor = inactive;
        this.state = state;

        image.color = state ? activeColor : inactiveColor;
    }

    public void ChangeState()
    {
        state = !state;
        image.color = state ? activeColor : inactiveColor;
        if (state) Global.gameMode = gameMode;

        foreach (GameObject param in gameModeParameters)
        {
            param.SetActive(state);
        }
    }

    public override void Execute()
    {
        base.Execute();
        ChangeState();
        otherButton.ChangeState();
    }
}

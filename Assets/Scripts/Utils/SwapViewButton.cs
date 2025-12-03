using UnityEngine;

public class SwapViewButton : MonoBehaviour
{
    [SerializeField] private GameObject gridPanel;
    [SerializeField] private GameObject listPanel;

    [SerializeField] private GameObject gridIcon;
    [SerializeField] private GameObject listIcon;

    [SerializeField] private bool startWithGrid = true;
    private bool isGridView;

    private void Start()
    {
        isGridView = startWithGrid;

        gridPanel.SetActive(isGridView);
        listPanel.SetActive(isGridView);
    }

    public void SwapView()
    {
        isGridView = !isGridView;
        
        gridPanel.SetActive(isGridView);
        listPanel.SetActive(!isGridView);

        gridIcon.SetActive(isGridView);
        listIcon.SetActive(!isGridView);
    }
}

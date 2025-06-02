using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Setting")]
    [SerializeField] private string variable;

    [Header("Sprites")]
    [SerializeField] private Sprite boxOn;
    [SerializeField] private Sprite boxOff;

    private bool state;
    private Image image;
    private Outline textOutline;

    private void Awake()
    {
        image = GetComponent<Image>();
        textOutline = transform.parent.GetComponent<Outline>();
    }

    private void OnEnable()
    {
        bool value = PlayerPrefs.GetInt(variable, 1) == 1;
        UpdateDisplay(value);
    }

    private void UpdateToggle(bool value)
    {
        PlayerPrefs.SetInt(variable, value ? 1 : 0);
        UpdateDisplay(value);
    }

    private void UpdateDisplay(bool value)
    {
        image.sprite = value ? boxOn : boxOff;
        textOutline.enabled = value;
        state = value;
    }

    public void OnPointerClick(PointerEventData eventdata)
    {
        UpdateToggle(!state);
    }
}

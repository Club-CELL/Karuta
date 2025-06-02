using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Folder : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{

    public GameObject explorer;
    public Color colorSelected;
    public Color colorNeutral;
    public bool activated;

    public void OnPointerUp(PointerEventData eventdata)
    {
        if (activated)
        {
            enterFolder();
        }
        activated = false;
    }
    public void OnPointerExit(PointerEventData eventdata)
    {
        activated = false;
    }
    public void OnPointerDown(PointerEventData eventdata)
    {
        activated = true;
    }


    private void enterFolder()
    {
        explorer.GetComponent<Explorer>().ChangeDirectory(GetComponentInChildren<Text>().text);
    }


    void Update () {
		if(activated)
        {
            GetComponent<Image>().color = colorSelected;
        }
        else
        {
            GetComponent<Image>().color = colorNeutral;
        }
	}
}

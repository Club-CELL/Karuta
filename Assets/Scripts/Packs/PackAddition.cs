using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PackAddition : MonoBehaviour
{

    public TMP_InputField folderIdInput;
    public TMP_Text infoText;
    public UpdateContent updateContent;
    //public GameObject packControlPrefab ;
    //public Transform packControlParent;
    public PacksMenu packMenu;


    private void OnEnable()
    {
        Output("");
    }
    public void CheckFolder()
    {
        updateContent.gameObject.SetActive(true);
        updateContent.GetPack(folderIdInput.text, HandlePack, Output);
    }

    public void HandlePack(SerializedDeckPack serializedPack)
    {
        updateContent.gameObject.SetActive(false);
        if(serializedPack != null)
        {
            packMenu.AddSerializedPack(serializedPack);
            gameObject.SetActive(false);
        }
        else
        {
            //Output("<color=red>Pack not found :(</color>");
        }
    }


    public void Output(string s)
    {
        infoText.text = s;
    }
}

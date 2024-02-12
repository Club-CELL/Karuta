using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PackControl : MonoBehaviour
{
    public DeckPack pack;
    public UpdateContent updateContent;

    public Text nameText;
    public RawImage banner;
    public Image bannerBackground;
    public AspectRatioFitter bannerFitter;
    public Text updateButtonText;
    public Image updateButtonImage;

    public static Color bannerBackgroundColor = Color.black;
    public static Color bannerTextColor = Color.white;
    public static Color updateButtonColor = Color.white;
    public static Color updateButtonTextColor = Color.white;


    bool subscribedToLoadedBanner = false;
    private void Awake()
    {
        if(!subscribedToLoadedBanner)
        {
            PackLoader.onLoadedBanner += OnLoadedBanner;
            subscribedToLoadedBanner = true;
        }
    }

    private void OnEnable()
    {
        nameText.color = bannerTextColor;
        bannerBackground.color = bannerBackgroundColor;
        if(updateButtonImage)
        {
            updateButtonImage.color = updateButtonColor;
        }
        if(updateButtonText)
        {
            updateButtonText.color = updateButtonTextColor;
        }
        Setup();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            Setup();
        }
    }


    void OnLoadedBanner(DeckPack pack)
    {
        Debug.Log($"On loaded banner {pack.driveFolderId} {pack.banner != null}");
        if (pack.driveFolderId == this.pack.driveFolderId)
        {
            Setup();
        }
    }
    public void Setup()
    {
        if(!pack)
        {
            return;
        }

        nameText.text = pack.title;
        Debug.Log($"Setting up banner for pack {pack.title} and banner is {(pack.banner == null ? "null" : "not null" ) }", this);

        if (pack.banner)
        {
            banner.texture = pack.banner;
            bannerFitter.aspectRatio = (float)pack.banner.width / (float)pack.banner.height;
            banner.enabled = true;
        }

    }
    public void Select()
    {
        updateContent.gameObject.SetActive(true);
        updateContent.GetPack(pack.driveFolderId, ReadDeckPackAndUpdate, Output);
    }

    void Output(string s)
    {
        Debug.Log(s);
    }

    public void ReadDeckPack(SerializedDeckPack serializedPack)
    {
        if(serializedPack != null)
        {
            pack.driveFolderId = serializedPack.driveFolderId;
            pack.title = serializedPack.title;
            Setup();
            if(!string.IsNullOrEmpty(serializedPack.banner))
            {
                PackLoader.instance.LoadBanner(serializedPack);
            }
        }
    }

    void ReadDeckPackAndUpdate(SerializedDeckPack serializedPack)
    {
        ReadDeckPack(serializedPack);
        updateContent.pack = pack;
        updateContent.gameObject.SetActive(true);
        updateContent.UpdatePackContent();
    }

    private void OnDestroy()
    {
        Debug.Log($"Pack control {pack.title} on destroy");
        PackLoader.onLoadedBanner -= OnLoadedBanner;
    }
}

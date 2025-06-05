using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;

public class DeckListDisplay : MonoBehaviour
{
    private List<string> deckPaths;
    private readonly List<Deck> decks = new();
    private ThemeLoaderDecksChoice themeLoader;

    [Header("Dependancies")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject packBanner;
    [SerializeField] private GameObject deckButton;

    private void OnEnable()
    {
        themeLoader = GameObject.FindGameObjectWithTag("ThemeLoader").GetComponent<ThemeLoaderDecksChoice>();
    }

    private GameObject CreateDeckButton(string text)
    {
        GameObject newBouton = Instantiate(deckButton, content);
        newBouton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        newBouton.SetActive(true);

        return newBouton;
    }

    public void ReadDecks()
    {
        deckPaths = new List<string>();
        var packDirectories = Directory.GetDirectories(Path.Combine(Global.mainPath, "Packs"));

        foreach(var packDirectory in packDirectories)
        {
            var deckDirectory = Path.Combine(packDirectory, "Decks");
            if (Directory.Exists(deckDirectory))
            {
                var paths = Directory.GetFiles(deckDirectory);
                if (paths.Length > 0)
                {
                    var packId = new DirectoryInfo(packDirectory).Name;
                    GameObject banner = CreatePackBanner(packId);

                    foreach (var path in paths)
                    {
                        deckPaths.Add(path);

                        Deck newDeck = new(packId);
                        string text = File.ReadAllText(path);
                        var entries = text.Split('\n');

                        foreach (var entry in entries)
                        {
                            string trimmedEntry = entry.Trim();
                            if (!string.IsNullOrEmpty(trimmedEntry))
                            {
                                newDeck.cards.Add(trimmedEntry);
                            }
                        }
                        decks.Add(newDeck);

                        string name = new FileInfo(path).Name[..^4];

                        GameObject button = CreateDeckButton(name);
                        DeckButton deckButton = button.GetComponent<DeckButton>();
                        deckButton.SetButtonInfo(name, newDeck);
                        
                        if (Global.gameMode == Global.GameModes.Trial)
                        {
                            banner.GetComponent<PackTrialButton>().AddDeck(deckButton);
                            Button bannerButton = banner.GetComponentInChildren<Button>();
                            themeLoader.SetButtonColor(bannerButton.gameObject.GetComponent<Image>());
                        }

                        themeLoader.SetButtonColor(button.GetComponent<Image>());
                    }
                }
            }
        }
    }

    private GameObject CreatePackBanner(string packId)
    {
        GameObject newBanner = Instantiate(packBanner, content);
        var packControl = newBanner.GetComponent<PackControl>();

        if(PackLoader.packPerId.ContainsKey(packId))
        {
            packControl.pack = PackLoader.packPerId[packId];
        }
        else
        {
            string filePath = Path.Combine(Global.mainPath, "Packs", packId, "pack.json");

            if (File.Exists(filePath))
            {
                SerializedDeckPack serializedPack = JsonSerialization.ReadFromJson<SerializedDeckPack>(filePath);
                PackLoader.instance.AddSerializedPack(serializedPack);

                if (PackLoader.packPerId.ContainsKey(serializedPack.driveFolderId))
                {
                    DeckPack pack = PackLoader.packPerId[serializedPack.driveFolderId];
                    packControl.pack = pack;
                }
                else
                {
                    Debug.LogError($"No deck pack after AddSerializedPack id: {serializedPack.driveFolderId}");
                }
                packControl.ReadDeckPack(serializedPack);
            }
        }
        packControl.Setup();
        return newBanner;
    }
}

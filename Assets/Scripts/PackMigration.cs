using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackMigration : MonoBehaviour
{
    public string resourcePack = "Anim'INT";
    // Start is called before the first frame update
    void Start()
    {
        Migrate();
    }

    void Migrate()
    {
        DeckPack pack = Resources.Load<DeckPack>(Path.Combine("Packs", resourcePack));
        string id = pack.driveFolderId;

        string packDirectory = Path.Combine(Application.persistentDataPath, "Packs", id);
        if (Directory.Exists(packDirectory))
        {
            return;
        }
        Directory.CreateDirectory(packDirectory);
        string oldVisualsDirectory = Path.Combine(Application.persistentDataPath, "Visuels");
        string oldSoundsDirectory = Path.Combine(Application.persistentDataPath, "Son");
        string oldDecksDirectory = Path.Combine(Application.persistentDataPath, "Decks");
        string oldThemesDirectory = Path.Combine(Application.persistentDataPath, "Themes");
        if (Directory.Exists(oldVisualsDirectory))
        {
            Directory.Move(oldVisualsDirectory, Path.Combine(packDirectory, "Visuals"));
        }
        if (Directory.Exists(oldSoundsDirectory))
        {
            Directory.Move(oldSoundsDirectory, Path.Combine(packDirectory, "Sounds"));
        }
        if (Directory.Exists(oldDecksDirectory))
        {
            Directory.Move(oldDecksDirectory, Path.Combine(packDirectory, "Decks"));
        }
        if (Directory.Exists(oldThemesDirectory))
        {
            Directory.Move(oldThemesDirectory, Path.Combine(packDirectory, "Themes"));
        }
    }
}

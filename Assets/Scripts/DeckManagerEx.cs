using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;




public class Deck
{
    public string packId;
    public List<string> deck = new List<string>();

    public Deck(string packId, List<string> deck)
    {
        this.packId = packId;
        this.deck = deck;
    }
}
public class DeckManagerEx : MonoBehaviour
{
    List<string> deckPaths;
    //string[] DecksPaths;
    //string[][] Decks = new string[100][];
    List<Deck> decks = new List<Deck>();
    public string[] decktester = new string[40];

    public static int joueur = 1;
    public float sep_y;
    public float back_speed;
    public GameObject title;
    float x0;
    float y0;
    public float decalageDebut;
    public int marginUp = 50;
    public int marginDown = 50;
    public GameObject arrow;
    public GameObject packBanner;
    DeckPack[] resourcePacks;
    Vector2 fingerStart;
    Vector2 fingerEnd;
    float y_start_touch;
    float localY0;
    float height;
    static DeckManagerEx instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        Global.mainPath = PathManager.MainPath;
        joueur = 1;
        x0 = arrow.transform.position.x;
        y0 = arrow.transform.position.y;
        localY0 = arrow.GetComponent<RectTransform>().localPosition.y;
        resourcePacks = Resources.LoadAll<DeckPack>("Packs");
        ReadDecks();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("nbJoueurs:" + Global.nbJoueurs);
        swipeDetect();
    }



    public Vector2 swipeDetect()
    {

        if (Input.touches.Length == 0)
        {
            if (!Input.GetMouseButton(0))
            {
                LetGo();
            }
            if(Input.GetMouseButtonDown(0))
            {
                fingerStart = Input.mousePosition;
                fingerEnd = Input.mousePosition;
                y_start_touch = transform.position.y;
            }
            else if (Input.GetMouseButton(0))
            {
                fingerEnd = Input.mousePosition;
                float delta_x = fingerEnd.x - fingerStart.x;
                float delta_y = fingerEnd.y - fingerStart.y;
                Hold(delta_y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                float delta_x = fingerStart.x - fingerEnd.x;
                float delta_y = fingerStart.y - fingerEnd.y;

                LetGo();

            }
            //LetGo();

        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerStart = touch.position;
                fingerEnd = touch.position;
                y_start_touch = transform.position.y;

            }
            if (touch.phase == TouchPhase.Moved)
            {
                fingerEnd = touch.position;
                float delta_x = fingerEnd.x - fingerStart.x;
                float delta_y = fingerEnd.y - fingerStart.y;
                Hold(delta_y);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                float delta_x = fingerStart.x - fingerEnd.x;
                float delta_y = fingerStart.y - fingerEnd.y;

                LetGo();

            }
        }
        return new Vector2(0, 0);
    }

    void Hold(float d_y)
    {

        Vector2 pos = GetComponent<RectTransform>().position;
        pos = new Vector2(pos.x, y_start_touch + d_y);
        GetComponent<RectTransform>().position = pos;

    }

    void LetGo()
    {
        float dy = 0;
        Vector2 pos = GetComponent<RectTransform>().position;
        int nb_arrows = deckPaths.Count;//DecksPaths.Length;

        GameObject c = GameObject.Find("Canvas");

        int hh = Screen.height;

        int hhh = hh - marginUp;

        float size = height;// (nb_arrows) * sep_y;

        float max = Math.Max(hhh, size + marginDown);
        float min = Math.Min(size + marginDown, hhh);

        max = max - localY0;
        min = min - localY0;


        if (pos.y < min)
        {
            dy = back_speed * (min - pos.y);
        }
        if (pos.y > max)
        {
            dy = -back_speed * (pos.y - max);
        }

        pos = new Vector2(pos.x, pos.y + dy);
        GetComponent<RectTransform>().position = pos;
    }








    public static void NextJoueur()
    {
        joueur++;
        if (joueur <= Global.nbJoueurs)
        {
            instance.title.GetComponent<Text>().text = "Player " + joueur.ToString();

        }
        else
        {
            SceneManager.LoadScene("Game");
        }

    }

    void ReadDecks()
    {
        //Start Int
        /*
        DecksFiles = Resources.LoadAll<TextAsset>("Decks");


        for (int i = 0; i < DecksFiles.Length; i++)
        {
            Decks[i] = DecksFiles[i].text.Split('\n');

        }
        */
        //End Int

        //Start Ex (+DecksFiles--> DecksPaths)

        //DecksPaths = Directory.GetFiles(Path.Combine(Global.mainPath, "Decks"), "*.txt");
        deckPaths = new List<string>();
        var packDirectories = Directory.GetDirectories(Path.Combine(Global.mainPath, "Packs"));

        float currentY = y0;
        foreach(var packDirectory in packDirectories)
        {
            var deckDirectory = Path.Combine(packDirectory, "Decks");
            if (Directory.Exists(deckDirectory))
            {
                var paths = Directory.GetFiles(deckDirectory);

                if(paths.Length>0)
                {
                    var packId = new DirectoryInfo(packDirectory).Name;
                    if(currentY<y0)
                    {
                        currentY -= 0.5f*sep_y;
                    }
                    CreatePackBanner(packId, new Vector2(x0, currentY));
                    currentY -= 1.25f * sep_y;
                    foreach (var path in paths)
                    {
                        deckPaths.Add(path);


                        string text = File.ReadAllText(path);
                        var entries = text.Split('\n');


                        Deck newDeck = new Deck(packId, new List<string>());// = new List<string>();
                        foreach (var entry in entries)
                        {
                            string trimmedEntry = entry.Trim();
                            if (!string.IsNullOrEmpty(trimmedEntry))
                            {
                                newDeck.deck.Add(trimmedEntry);
                                Debug.Log($"Trimed entry: [{trimmedEntry}]");
                            }
                        }
                        decks.Add(newDeck);


                        string name = new FileInfo(path).Name;
                        name = name.Substring(0, name.Length - 4);
                        GameObject deckButton = CreateDeckButton(name, x0, currentY);
                        ChoixDecks choixDecks = deckButton.GetComponent<ChoixDecks>();

                        choixDecks.deck = newDeck;
                        choixDecks.deckName = name;

                        currentY -= sep_y;



                    }
                }
            }
        }
        height = y0 - currentY;
        /*
        //Debug build
        string s = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" + Path.Combine(Global.mainPath, "Decks");
        if (DecksPaths.Length > 0)
        {
            s += "\n" + DecksPaths[0];
        }
        GetComponent<Text>().text = s;
        //end debug
         */



        //for (int i = 0; i < deckPaths.Count; i++)
        //{
        //    string  text = File.ReadAllText(deckPaths[i]);
        //    var entries = text.Split('\n');
        //    decks[i].deck = new List<string>();
        //    foreach (var entry in entries)
        //    {
        //        if(!string.IsNullOrEmpty(entry.Trim()))
        //        {
        //            decks[i].deck.Add(entry);
        //        }
        //    }
        //    //Decks[i] = entries.Split('\n');
        //}



        //End Ex


        //Debug.Log("Nombre de decks:" + deckPaths.Count.ToString());
        //char excessChar = Decks[0][0][Decks[0][0].Length - 1];
        //for (int i = 0; i < deckPaths.Count; i++)
        //{
        //    for (int j = 0; j < Decks[i].Length; j++)
        //    {
        //        Decks[i][j] = Decks[i][j].TrimEnd(excessChar);
        //    }
        //}

        //for (int i = 0; i < deckPaths.Count; i++)
        //{
        //    int nb = decks[i].deck.Count;// Decks[i].Length;
        //    Debug.Log(i.ToString() + "-" + decks[i].deck.Count.ToString() + "-" + decks[i].deck[0].ToString());
        //    //decktester = Decks [i];
        //    while (Decks[i][nb - 1].TrimEnd(new char[] { ' ' }).Equals(""))
        //    {
        //        nb--;
        //    }
        //
        //    string[] temp = Decks[i];
        //    Decks[i] = new string[nb];
        //    Array.Copy(temp, 0, Decks[i], 0, nb);
        //}

        //for (int i = 0; i < deckPaths.Count; i++)
        //{
        //    string name = new FileInfo(deckPaths[i]).Name;
        //    name = name.Substring(0, name.Length - 4);
        //    GameObject Deck = createButton(name, x0, y0 - i * sep_y);
        //    Deck.GetComponent<ChoixDecks>().deck = decks[i].deck;
        //    Deck.GetComponent<ChoixDecks>().deckName = name;
        //}
    }


    GameObject CreatePackBanner(string packId, Vector2 pos)
    {

        GameObject newBanner = Instantiate(packBanner,transform);
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

        newBanner.transform.position = new Vector3(pos.x, pos.y, 0);

        return newBanner;

    }
    GameObject CreateDeckButton(string text, float x, float y)
    {
        GameObject newBouton = Instantiate(arrow) as GameObject;
        newBouton.transform.SetParent(transform, false);
        newBouton.GetComponent<RectTransform>().position = new Vector3(x, y, 0);

        newBouton.GetComponentInChildren<Text>().text = text;

        newBouton.SetActive(true);
        return newBouton;
    }

}

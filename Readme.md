# Karuta

## Origin

Karuta is a term used to describe **Japanese playing cards**. One of the game that used to be played with such cards was called **Uta-garuta**. In this game, a reader reads the **beginning of a poem** and the players have to **race** to find the corresponding card, with the **end of the poem**.

Members of the japanese animation club of Télécom SudParis (**Anim'INT**) created a similar game where cards correspond to **animes** and instead of poems, the opening of an anime is played and players have to race to tap on the right card.

This app can be thought of as a "companion app" to the game.
Note that while it was originally thought for animes, this new game is also played with video game cards and could be used with other types of media as well.

## Rules

Each player starts by choosing a deck. Then, they set up their cards in front of them and the application will randomly play musics. The players race to tap the corresponding card. When a card is tapped:

If it is the **right** card:
- If it's on the **tapping player's** side, the player gets the card
- If it's on the **other player's side**, the tapping player takes the card and **gives a card** to the other player to put on their (the other player's) side.

If it's the **wrong** card:
- If it's on the **tapping player's** side, nothing happens, the other player can still try to find the right card
- If it's on the **other player's side**, the other player can still try to find the right card and in any case, **the other player will give one card** to the mistaken tapping player to put on their (the mistaken tapping player's) side.

## What does the app do ?

People who want to contribute to the game create some **decks**, that are lists of anime/games/other with an image and a music (opening, ending, ost...) each. These decks are uploaded on google drive and this app downloads them to allow players to play with them.
Before the game starts, players each **choose a deck** and put their deck cards before them. In the app, the corresponding deck will be selected and the app will **randomly choose musics** to play.

If one of the players taps on the right card, they get this card and the card is marked as ***Found*** by swiping it to the **right** (it won't be played again). If no player got the card right, the card is marked ***Not found*** by swiping to the **left**. It could be played again at any moment (even right after).

## Compiling the app

If you want to compile the app yourself, you will need to create a **google service account**. Here is a [step by step tutorial](https://docs.google.com/document/d/12QXZd3Mx1vfYl01hFiYyAt3YCLnMrHFted1_7m6b7w0) on how to do this. Download the **json key** corresponding to this account.

Then, save the json key, and copy it to the ***Resources/Credentials*** folder. Rename it to ***credentials.json***. Do **not** commit this file to version control.

When creating packs, you should **share** them (with read access) with this service account (although, you could simply give read access to everyone, that works too and would allow it to be used by the same app compiled with a different service account).


## Packs

### Creating a pack

A ***Pack*** is a set of **decks** (and can also contain **themes** for the app). To create a pack, you need to use **google drive** (so you need to have a google account) and create a folder for this pack.
Inside the folder, you should have:
- A **"Decks"** folder, containing all the deck files of your pack (a deck is a .txt file with a list of the cards, one card per line)
- A **"Visuals"** folder, containing all the images of the cards of your decks. The name of each image has to be *card*.png, where *card* is the name of the corresponding card as written in the deck file
- A **"Sounds"** folder, containing all the musics of the cards of your decks. The name of each sound file has to be *card*.mp3, where *card* is the name of the corresponding card as written in the deck file
- A **"Themes"** folder, containing the themes you may want to add to the app.

In addition to this, you have to put a **"Pack" google doc** (recommended for ease of modification) or a **"Pack.json"** file in the root of the folder. This simple json will contain the **name** of the pack and the **name of the banner image** associated to the pack (which should also be in the root folder).

The json should look like this:
```
{
    "title": "MyPackName",
    "banner": "banner.png"
}
```

Check out [this pack](https://drive.google.com/drive/folders/1tHgOOtA-GQlIEd9xkNp-6vQd7uzoqVVF) as an example.


### Adding a pack

You need to share the google drive folder of your pack with read access. Sharing it with everyone is the easiest. Note that you can also share it only with the service account used in the app if you compiled it with your own.
Inside the app, you can add your pack by going to Options -> Packs -> New Pack to add your pack. For example, for the pack linked above ([this pack](https://drive.google.com/drive/folders/1tHgOOtA-GQlIEd9xkNp-6vQd7uzoqVVF)), you would need to enter the folder id *1tHgOOtA-GQlIEd9xkNp-6vQd7uzoqVVF*.

If you want to compile the app with your pack added by default:
- In the Assets/Resources/Packs folder: **Right click -> Create Deck Pack**
- Set the pack default title, folder ID and default banner.

It should be already added to the list when anyone installs the app. Note that you still need to **update** it a first time to get the content.


### Credits

- The **arrow** image in the app comes from the anime [***Jojo's Bizarre adventure***](https://jojo-animation.com/)
- The main **font** used in the app is [***Anime Ace 2***](https://www.dafont.com/anime-ace-bb.font)
- [**Mi-eau**](https://mi-eau.carrd.co/) for the drawing of **Momocom** on the main menu (the character on the main menu background when using the classic theme)
- [**Shingo Hayasa**](https://www.deviantart.com/shingo-hayasa) for the drawing of **Momocom** on the game screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Game Data
    public string playerName;
    public bool tutorial;
    public SerializationDictionary<string, bool> tempDict;
    public PersonalBests personalBests;
    //Game Data

    public GameData() //intial values for the game data
    {
        this.playerName = "NONAME";
        this.tutorial = false;
        this.tempDict = new SerializationDictionary<string,bool>();
        this.personalBests = new PersonalBests();
    }
}

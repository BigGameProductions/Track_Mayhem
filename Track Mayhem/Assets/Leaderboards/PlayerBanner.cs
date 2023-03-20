using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBanner
{
    public int place;
    public int flagNumber;
    public string player;
    public float bestMark;

    public PlayerBanner(int number, int flagNumber, string player, float bestMark)
    {
        this.place = number;
        this.flagNumber = flagNumber;
        this.player = player;
        this.bestMark = bestMark;
    }

    
}

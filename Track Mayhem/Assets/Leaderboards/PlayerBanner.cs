using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBanner
{
    public int place;
    public int flagNumber;
    public string player;
    public float bestMark;

    public float mark1;
    public float mark2;
    public float mark3;

    public bool isPlayer;

    public PlayerBanner(int number, int flagNumber, string player, float bestMark=0, bool isPlayer = false, float mark1=-100, float mark2=-100, float mark3=-100)
    {
        this.place = number;
        this.flagNumber = flagNumber;
        this.player = player;
        this.bestMark = bestMark;

        this.mark1 = mark1;
        this.mark2 = mark2;
        this.mark3 = mark3;

        this.isPlayer = isPlayer;
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string eventName; //name of the event that is active
    [SerializeField] private bool useSeeded; //determines if personal bests should be used too seed
    [SerializeField] private float seededMark; //if useSeeded is false determines the base mark to go off of
    [SerializeField] private float seedSpreadDown; //how much the players will be spread from the seed down
    [SerializeField] private float seedSpreadUp; //how much the players will be spread from the seed up


    private PersonalBests personalBests; //store personals bests for seeding

    public void LoadData(GameData data)
    {
        this.personalBests = data.personalBests; //load the personal bests
    }

    public void SaveData(ref GameData data)
    {
        //nothing to save
    }

    private void Start()
    {
        PlayerBanner[] playerBanners = generateBanners(8);
        foreach (PlayerBanner banner in playerBanners)
        {
            Debug.Log(banner.place + " " + banner.flagNumber + " " + banner.player + "     " + banner.bestMark);
        }
        //temp
        gameObject.GetComponent<DataPersistanceManager>().SaveGame();
        //temp
    }

    private PlayerBanner[] generateBanners(int size) //generates the banners for size number of people based off of the seeding and returns a list og them
    {
        PlayerBanner[] banners = new PlayerBanner[size];
        for (int i = 0; i<size; i++)
        {
            int flagNum = UnityEngine.Random.Range(0, 71);
            string name = "Testing";
            float personalBest = 0;
            if (useSeeded)
            {
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp);
            } else
            {
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp, seededMark);
            }
            banners[i] = new PlayerBanner(i, flagNum, name, (float)Math.Round(personalBest, 2)); //round personal bests to only two places

        }
        return sortBanners(banners, true);
    }

    //theEvent is the event that is used
    //useSeeded is if seeding on personal bests should be used
    //seedSpread[Down/Up] is the spread of the seeding from the original down and up from that mark
    //basedSeed is optional for if useSeeded is false what to base seeding around
    private float seedTimesForEvent(string theEvent, bool useSeeded, float seedSpreadDown, float seedSpreadUp, float basedSeed=0) //based on the event it gives a random seed based on that number
    {
        if (useSeeded)
        {
            if (theEvent == "LongJump")
            {
                basedSeed = personalBests.longJump;

            }
        }
        return UnityEngine.Random.Range((float)basedSeed - seedSpreadDown, basedSeed + seedSpreadUp + 1f);
        
    }

    private PlayerBanner[] sortBanners(PlayerBanner[] banners, bool bigOnTop) //sorts the banners by big or small on the top in the array
    {
        List<PlayerBanner> sortedBanners = new List<PlayerBanner>();
        sortedBanners.Add(new PlayerBanner(0, 0, "SortingPlaceholder", float.MaxValue)); //placeholder to help sort the items in the array by value
        foreach (PlayerBanner pb in banners)
        {
            for (int i = 0; i < sortedBanners.Count; i++)
            {
                if (pb.bestMark < sortedBanners.ElementAt(i).bestMark) //sort my best mark
                {
                    sortedBanners.Insert(i, pb);
                    break;
                }


            }

        }
        sortedBanners.RemoveAt(sortedBanners.Count - 1); // remove the placeholder
        PlayerBanner[] newBanners = bigOnTop ? sortedBanners.ToArray().Reverse<PlayerBanner>().ToArray() : sortedBanners.ToArray(); //if bigOnTop then reverse the list to make the biggest in top
        for (int i = 0; i<newBanners.Length; i++)
        {
            newBanners[i].place = i + 1;
        }
        return newBanners;

    }


}

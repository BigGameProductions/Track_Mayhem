using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string eventName; //name of the event that is active
    [SerializeField] private bool useSeeded; //determines if personal bests should be used too seed
    [SerializeField] private float seededMark; //if useSeeded is false determines the base mark to go off of
    [SerializeField] private float seedSpreadDown; //how much the players will be spread from the seed down
    [SerializeField] private float seedSpreadUp; //how much the players will be spread from the seed up
    [SerializeField] private Image[] leaderboardBanners; //a list of all the leaderboard banners that need to have information on them


    private PersonalBests personalBests; //store personals bests for seeding
    private string playerName; //store the name of the player

    private float basedSeed; //stores the based seed for the current event

    public void LoadData(GameData data)
    {
        this.personalBests = data.personalBests; //load the personal bests
        this.playerName = data.playerName; //name of the player
    }

    public void SaveData(ref GameData data)
    {
        //nothing to save
    }

    private void Start()
    {
        PlayerBanner[] playerBanners = generateBanners(7, true);
        PlayerBanner[] recordPlayerBanners = new PlayerBanner[] {
            new PlayerBanner(0, 0, "Olympic", 100),
            new PlayerBanner(0, 0, "World", 98),
            new PlayerBanner(0, 0, playerName, basedSeed)
        };
        setLeaderboard(recordPlayerBanners, 2);
        
    }

    //int size for amount of banners
    //bool addPlayer if the player should be added to the list
    private PlayerBanner[] generateBanners(int size, bool addPlayer) //generates the banners for size number of people based off of the seeding and returns a list og them
    {
        PlayerBanner[] banners = new PlayerBanner[size + (addPlayer ? 1:0)];
        for (int i = 0; i<size; i++)
        {
            int flagNum = UnityEngine.Random.Range(0, 71); //temporary amount of flags
            string name = "Testing";
            float personalBest = 0;
            if (useSeeded)
            {
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp);
            } else
            {
                basedSeed = seededMark;
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp);
            }
            banners[i] = new PlayerBanner(i, flagNum, name, (float)Math.Round(personalBest, 2)); //round personal bests to only two places

        }
        banners[banners.Length - 1] = new PlayerBanner(0, 10, playerName, personalBests.longJump);
        return sortBanners(banners, true);
    }

    //theEvent is the event that is used
    //useSeeded is if seeding on personal bests should be used
    //seedSpread[Down/Up] is the spread of the seeding from the original down and up from that mark
    //basedSeed is optional for if useSeeded is false what to base seeding around
    private float seedTimesForEvent(string theEvent, bool useSeeded, float seedSpreadDown, float seedSpreadUp) //based on the event it gives a random seed based on that number
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

    //mode 1 = normal 8 person leaderboard with best marks
    //mode 2 = olympic and world records and personal bests
    private void setLeaderboard(PlayerBanner[] playerBanners, int mode) //sets the leaderboard according to the array of playerBanners that it is given
    {
        for (int i = 0; i < playerBanners.Length; i++) //make all banenrs appear
        {
            leaderboardBanners[i].gameObject.SetActive(true);
        }
        for (int i=0; i<playerBanners.Length; i++)
        {
            for (int j = 0; j<leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true).Length; j++)
            {
                if (j>1) leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[j].gameObject.SetActive(false); //make the banner empty
            }
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(true); 
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[2].gameObject.SetActive(true); 
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.SetActive(true); 
            TextMeshProUGUI[] textBoxes = leaderboardBanners[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            textBoxes[0].text = playerBanners[i].place.ToString(); //place text box
            //Add flags for leaderboard
            textBoxes[1].text = playerBanners[i].player; //playerName text box
            if (mode == 1)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[0].gameObject.SetActive(true); //best mark label
                textBoxes[2].text = playerBanners[i].bestMark.ToString(); //best mark text box
            }
            else if (mode == 2)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[5].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.SetActive(true); //record label mark
                textBoxes[3].text = playerBanners[i].bestMark.ToString(); //setting record marks

            }
           
        }
        for (int i = playerBanners.Length; i < 8; i++) //remove banners that are not needed
        {
            leaderboardBanners[i].gameObject.SetActive(false);
        }
    }


}

//TODO Animations for the leaderboard
//TODO flags for the leaderboard
//TODO Make first stage of leaderboard (mode already created...add transition for it)
//TODO Make names list for players
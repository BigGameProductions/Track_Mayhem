using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string eventName; //name of the event that is active
    [SerializeField] private bool useSeeded; //determines if personal bests should be used too seed
    [SerializeField] private float seededMark; //if useSeeded is false determines the base mark to go off of
    [SerializeField] private float seedSpreadDown; //how much the players will be spread from the seed down
    [SerializeField] private float seedSpreadUp; //how much the players will be spread from the seed up
    [SerializeField] private Image[] leaderboardBanners; //a list of all the leaderboard banners that need to have information on them
    [SerializeField] private Camera cinematicCamera; // the camaera that plays the animations before the event
    [SerializeField] private GameObject leaderBoardHeader; //the header for the main leaderboard;
    [SerializeField] private GameObject personalBanner; //the banner shown for the player after each jump

    private PlayerBanner[] currentEventBanners;


    private PersonalBests personalBests; //store personals bests for seeding
    private string playerName; //store the name of the player
    private int animationStage = 0; //stores the stage for the cinematic camera before an event

    private float basedSeed; //stores the based seed for the current event

    private PlayerBanner personalBannersMarks; //stores the given set of marks for the event

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
        //nothing for now

        if (SceneManager.GetActiveScene().name != "EndScreen") //tests to make sure it is an event screen
        {
            //temp
            cinematicCamera.GetComponent<Animator>().speed = 10;
            //temp
        } else
        {
            currentEventBanners = PublicData.playerBannerTransfer;
            setLeaderboard(currentEventBanners, PublicData.leaderBoardMode); //shows the leaderboard sorted
        }



    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "EndScreen")
        {
            if (cinematicCamera.GetComponent<Animator>().GetInteger("Stage") != animationStage)
            {
                animationStage = cinematicCamera.GetComponent<Animator>().GetInteger("Stage"); //update the current stage
                updateCinematicStage(animationStage); //update the leaderboard
            }
        }
        
    }


    public void showCurrentPlayerMarks(PlayerBanner marks, int stageNum) //shows the single banner of the player preformace
    {
        personalBannersMarks = marks;
        updateCinematicStage(stageNum);
    }

    public void hidePersonalBanner() //hides the personal banner for the player
    {
        updateCinematicStage(0);
    }
    


    public void showUpdatedLeaderboard() //shows the updated numbers for all oppenents
    {
        updateCinematicStage(4); //show the updated leaderboard
        StartCoroutine(timeOfLeaderboard(3)); //hides the leaderboard after x seconds
    }

    public PlayerBanner getPlayerBanner()
    {
        foreach (PlayerBanner pb in currentEventBanners)
        {
            if (pb.isPlayer)
            {
                return pb;
            }
        }
        return null;
    }

    IEnumerator timeOfLeaderboard(int time)
    {
        yield return new WaitForSeconds(time);
        updateCinematicStage(0);
    }

    public bool leaderBoardVisble()
    {
        return leaderBoardHeader.activeInHierarchy;
    }
         
    //stage same as mode for setLeaderboard
    private void updateCinematicStage(int stage)
    {
        if (stage==0) //hides the leaderboard when it should not be shown
        {
            leaderBoardHeader.SetActive(false);
            personalBanner.SetActive(false);
            return;
        } else if (!leaderBoardHeader.activeInHierarchy && stage != 3) //make sure leaderboard is visible for other stages too
        {
            leaderBoardHeader.SetActive(true);
        }
        PlayerBanner[] playerBanners = new PlayerBanner[0];
        if (stage == 1)
        {
            playerBanners = generateBanners(7, true);
            currentEventBanners = playerBanners;
            
        } else if (stage == 2)
        {
            playerBanners = new PlayerBanner[] {
            new PlayerBanner(0, 0, "Olympic", 100),
            new PlayerBanner(0, 0, "World", 98),
            new PlayerBanner(0, 0, playerName, basedSeed)
             };
        } else if (stage == 3) //current player jump
        {
            playerBanners = new PlayerBanner[0];
            personalBanner.SetActive(true);
        } else if (stage == 4)
        {
            currentEventBanners = simulateMark(currentEventBanners, eventName, 2, 5); //makes marks for oppenents
            playerBanners = currentEventBanners;
            PublicData.playerBannerTransfer = currentEventBanners; //sets the end screen leaderboard to match the current one
            PublicData.leaderBoardMode = 4; //sets leaderboard mode for the end screen
        }

        setLeaderboard(playerBanners, stage);
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
        banners[banners.Length - 1] = new PlayerBanner(0, 10, playerName, personalBests.longJump, isPlayer:true);
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
            basedSeed = getMarkForEvent(theEvent);
        }
        return UnityEngine.Random.Range((float)basedSeed - seedSpreadDown, basedSeed + seedSpreadUp + 1f) * 12;
        
    }

    public float getMarkForEvent(string theEvent) //returns the player mark in the save files for the given event
    {
        if (theEvent == "LongJump")
        {
            return personalBests.longJump;

        }
        return 0;
    }

    //-100 is empty
    //-1000 is foul
    //simulates marks for the oppenents based on the spread given in the parameters
    private PlayerBanner[] simulateMark(PlayerBanner[] playerBanners, string theEvent, float spreadUp, float spreadDown)
    {
        foreach (PlayerBanner pb in playerBanners)
        {
            if (!pb.isPlayer) //making sure the banner is not for the player
            {
                if (pb.mark1 == -100)
                {
                    pb.mark1 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    continue;
                }
                if (pb.mark2 == -100)
                {
                    pb.mark2 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    continue;
                }
                if (pb.mark3 == -100)
                {
                    pb.mark3 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    continue;
                }
            }
            
        }
        return sortBanners(playerBanners, true, true);

    }

    private PlayerBanner[] sortBanners(PlayerBanner[] banners, bool bigOnTop, bool ofThree=false) //sorts the banners by big or small on the top in the array
    {
        List<PlayerBanner> sortedBanners = new List<PlayerBanner>();
        sortedBanners.Add(new PlayerBanner(0, 0, "SortingPlaceholder", float.MaxValue)); //placeholder to help sort the items in the array by value
        foreach (PlayerBanner pb in banners)
        {
            for (int i = 0; i < sortedBanners.Count; i++)
            {
                if ((ofThree ? Math.Max(Math.Max(pb.mark1, pb.mark2), pb.mark3) : pb.bestMark) < sortedBanners.ElementAt(i).bestMark) //sort my best mark
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

    //mode 0 = hide leaderboard
    //mode 1 = normal 8 person leaderboard with best marks
    //mode 2 = olympic and world records and personal bests
    //mode 3 = current jumps
    //mode 4 = 3 best marks for all players
    private void setLeaderboard(PlayerBanner[] playerBanners, int mode) //sets the leaderboard according to the array of playerBanners that it is given
    {
        for (int i = 0; i < playerBanners.Length; i++) //make all banners appear
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
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[4].gameObject.SetActive(true); //best mark label
                textBoxes[2].text = markToString(playerBanners[i].bestMark); //best mark text box
            }
            else if (mode == 2)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[5].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.SetActive(true); //record label mark
                textBoxes[3].text = markToString(playerBanners[i].bestMark); //setting record marks

            } else if (mode == 4)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[7].gameObject.SetActive(true); //record label mark
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[8].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[9].gameObject.SetActive(true); //record label mark
                textBoxes[4].text = markToString(playerBanners[i].mark1);
                textBoxes[5].text = markToString(playerBanners[i].mark2);
                textBoxes[6].text = markToString(playerBanners[i].mark3);

            }

        }
        for (int i = playerBanners.Length; i < 8; i++) //remove banners that are not needed
        {
            leaderboardBanners[i].gameObject.SetActive(false);
        }
        if (mode == 3)
        {
            TextMeshProUGUI[] textBoxes = personalBanner.GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int j = 4; j < personalBanner.GetComponentsInChildren<RectTransform>(true).Length; j++)
            {
                if (j <=6 || j>9) personalBanner.GetComponentsInChildren<RectTransform>(true)[j].gameObject.SetActive(false); //make the banner empty
                //j==0 is the main banner
                // j <= 6; j > 9 is the bound for the items that are needed
            }
            textBoxes[4].text = markToString(personalBannersMarks.mark1);
            textBoxes[5].text = markToString(personalBannersMarks.mark2);
            textBoxes[6].text = markToString(personalBannersMarks.mark3);

        }
    }

    private string markToString(float mark)
    {
        if (mark == -100)
        {
            return "X";
        } else if (mark == -1000)
        {
            return "FOUL";
        } else
        {
            return ((int)mark / 12) + "'" + Math.Round(mark % 12, 2) + "''";
        }
        
    }


}

//TODO Animations for the leaderboard
//TODO flags for the leaderboard
//TODO Make names list for players

//TODO add fouls for simulated oppenents
//TODO make oppenents round to nearest 0.25
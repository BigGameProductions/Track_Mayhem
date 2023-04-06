using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LongJumpManager : MonoBehaviour
{
    [SerializeField] private Image runMeterBar; //the bar that is displayed on the run meter
    [SerializeField] private Image jumpMeterBar;//the bar that is displayed on the jump meter
    [SerializeField] private GameObject player; //the player controller

    [SerializeField] private LeaderboardManager leaderboardManager;

    [SerializeField] private Camera runningCamera;//camera for running
    [SerializeField] private Camera jumpingCamera;//camera for jumping

    public float runningSpeed = 0; //stores the current running speed of the player
    public float jumpMeterSpeed = 0; //stores the current value of the jump meter
    [SerializeField] private float speedPerClick = 30; //temporary storing the speed gained when clicked
    private float maxSpeed = 300; //temporary storing the max speed for the player
    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    private bool jumpMeterDirection = false;//direction of the jemp meter
    private float startingJumpBar;//stores the starting position of the jump meter bar
    private float jumpBarIncreasePerInteger;//stores the amount the bar moves per increase
    [SerializeField] private float jumpMeterToTop;//stores the distance from the begining to the end of the jump meter
    [SerializeField] private float jumpBarSpeed; //stores how fast the jump bar moves
    private float movingBarSpeed;

    [SerializeField] private float powerToAnimationSpeedRatio;
    [SerializeField] private float pullInLegPower;


    private float startingBarHeight; //stores the starting height of the meter bar
    private float barIncreasePerSpeed; //stores the amountd the ui bar increases per speed
    [SerializeField] private float increaseForBarToTheTop;//stores distance from the bottom of the bar to the top
    [SerializeField] private float barDecreaseSpeed;//stores how fast the bar decreases

    private int currentJumpNumber = 0; //stores the amount of jumps that player has taken
    PlayerBanner currentPlayerBanner; //stores the current player data in a banner class

    private Vector3 startingPlayerPosition = new Vector3(-2103.53f, 226.73f, -369.71f); //starting position of the runner

    private bool isFoul = false; //tells if the current jump is a foul

    public float totalRunningSpeed; //total speed that has been made for not being idle
    public float timeElapsedRunning; //time that is elapsed for running not idle

    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand
    [SerializeField] private Image prImage; //is the image that appears when you pr

    
    // Start is called before the first frame update
    void Start()
    {
        startingBarHeight = runMeterBar.transform.position.y;
        barIncreasePerSpeed = increaseForBarToTheTop / maxSpeed;

        startingJumpBar = jumpMeterBar.transform.position.x;
        jumpBarIncreasePerInteger = jumpMeterToTop / 200f;

        jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter

        player.transform.position = startingPlayerPosition; //puts player in starting position

        foulImage.enabled = false; //hide the foul icon
        prImage.enabled = false; //hides the pr image
    }

    // Update is called once per frame
    void Update()
    {
        if (runningCamera.enabled)
        {
            if (runningSpeed <= 0) //makes sure running speed does not go below 0
            {
                runningSpeed = 0;
            }
            else
            {
                runningSpeed -= Time.deltaTime * barDecreaseSpeed; //decreses running speed
            }
            if (Input.GetKeyDown(KeyCode.Space)) //updating speed on click
            {
                runningSpeed += speedPerClick;
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            if (runningSpeed > maxSpeed) //making a max speed
            {
                runningSpeed = maxSpeed;
            }
            runMeterBar.transform.position = new Vector3(runMeterBar.transform.position.x, startingBarHeight + (runningSpeed * barIncreasePerSpeed), runMeterBar.transform.position.z);
            if (player.transform.position.x > -1901) //testing for an automatic foul by running past the board
            {
                isFoul = true;
                updatePlayerBanner(-1000);
                afterJump();
            }
            if (Input.GetKeyDown(KeyCode.P)) //if the player presses the jump button
            {
                runningCamera.enabled = false;
                jumpingCamera.enabled = true;
                runMeterBar.transform.parent.gameObject.SetActive(false); //hide run meter
                player.GetComponentInChildren<Animator>().speed = 0; //make running animation stop
                jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(true); //sets the jump meter to showing
                movingBarSpeed = jumpBarSpeed; //setting the bar speed to normal  speed
            }
        }
        if (jumpMeterBar.gameObject.transform.parent.gameObject.activeInHierarchy) //about to jump
        {
            
            jumpMeterSpeed += Time.deltaTime * movingBarSpeed * (jumpMeterDirection ? -1:1);
            if (jumpMeterSpeed >= 200 || jumpMeterSpeed <=0)
            {
                if (jumpMeterSpeed >= 200) //prevents bar from going out of bounds
                {
                    jumpMeterSpeed = 200;
                } else
                {
                    jumpMeterSpeed = 0;
                }
                jumpMeterDirection = !jumpMeterDirection;
            }
            jumpMeterBar.transform.position = new Vector3(startingJumpBar + (jumpMeterSpeed * jumpBarIncreasePerInteger), jumpMeterBar.transform.position.y, jumpMeterBar.transform.position.z);
            if (Input.GetKeyDown(KeyCode.Space)) //makes jump
            {
                movingBarSpeed = 0; //stopping the bar from moving
                if (player.transform.position.x > -1902.73) //testing got jumping foul
                {
                    isFoul = true;
                    foulImage.enabled = true;
                }
                StartCoroutine(jumpMeterHold(0.5f)); //calls waiting method 
            }
        }
        if (player.GetComponent<Rigidbody>().useGravity) //if in jumping animation
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float playerHeight = player.transform.position.y;
                if (playerHeight<227.4 && playerHeight>225.5) //checks if the leg pull is within an optimal range to work
                {
                    player.GetComponent<Rigidbody>().velocity = new Vector3(pullInLegPower, 0, 0); //gives a little extra boost
                    StartCoroutine(legKickVelocity(0.5f)); //make sure the kick doesn;t last for ever
                }
                else
                {
                    player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //makes the player stop the jump
                }
                player.GetComponentInChildren<Animator>().Play("LegPull"); //pulls in legs




            }
            if (player.transform.position.y<225.5)
            {
                player.GetComponent<Rigidbody>().useGravity = false; //stops the jumping animation loop
                StartCoroutine(legKickVelocity(0.2f)); //stop the landing from going forever
                if (player.transform.position.x < -1890) //checks if the player is on the sandpit
                {
                    isFoul = true;
                    foulImage.enabled = true;
                }
                StartCoroutine(waitAfterJump());
            }
        }
        
    }

    IEnumerator legKickVelocity(float time)
    {
        yield return new WaitForSeconds(time);
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //makes the player stop the kick velocity

    }

    IEnumerator waitAfterJump()
    {
        yield return new WaitForSeconds(1);
        //-1899.73 At 0 feet 
        //-1864.7 At 19 feet
        //-1875.03 At 16 feet
        if (isFoul) // checks for fouls
        {
            updatePlayerBanner(-1000);
        } else {
            float spacesPerInch = (1875.03f - 1864.7f) / 36f; //16 feet minus 19 feet divided by the inches in 3 feet
            int totalInches = (int)Math.Round((1930.4792068f + player.transform.position.x) / spacesPerInch); //finds total inches that have been jumped (distance jumped divided by spaces per inch of the sand)
            if (currentJumpNumber == 0) //fix random bugs for jumping distance
            {
                totalInches -= 2;
            } else
            {
                totalInches += 5;
            }
            if (totalInches > leaderboardManager.getMarkForEvent("LongJump")) //checks for a new personal record
            {
                prImage.enabled = true; //shows the pr banner
            }
            updatePlayerBanner(totalInches);
        }
        afterJump();
       
    }

    private void updatePlayerBanner(float mark)
    {
        currentPlayerBanner = leaderboardManager.getPlayerBanner();

        if (currentJumpNumber == 0)
        {
            currentPlayerBanner.mark1 = mark;
        } else if (currentJumpNumber == 1)
        {
            currentPlayerBanner.mark2 = mark;
        } else if (currentJumpNumber == 2)
        {
            currentPlayerBanner.mark3 = mark;
        }
    }

    private void afterJump()
    {
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentJumpNumber++; //inceases to the next jump
        player.GetComponentInChildren<Animator>().Play("EndStance"); //rotates the player to the camera
        player.transform.position = new Vector3(-1966, 226.73f, -370.56f); //makes the player in the middle of the runway for show
        runningSpeed = 0; //resets running speed
        StartCoroutine(waitAfterPersonalBanner(2));
    }

    IEnumerator waitAfterPersonalBanner(int time)
    {
        yield return new WaitForSeconds(time);
        if (currentJumpNumber == 3)
        {
            SceneManager.LoadScene("EndScreen");
        }
        player.transform.position = startingPlayerPosition;
        player.GetComponentInChildren<Animator>().Play("Running");
        runningCamera.enabled = true; //shows running camera
        jumpingCamera.enabled = false; //hides jumping camera
        runMeterBar.transform.parent.gameObject.SetActive(true); //shows run meter bar
        leaderboardManager.hidePersonalBanner(); //hides personal banner
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0); //reset rotation
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0); //reset position
        foulImage.enabled = false; //hides the image
        prImage.enabled = false;// hides the image
        isFoul = false; //makes the jump not a foul
        leaderboardManager.showUpdatedLeaderboard();
    }

    IEnumerator jumpMeterHold(float time) //holds the meter forzen for time so you can she what it landed on
    {
        yield return new WaitForSeconds(time);
        jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(false); //sets the jump meter to hiding
        Debug.Log(totalRunningSpeed / timeElapsedRunning);
        float powerPercent = 1; //percent of max power used
        float averageSpeed = totalRunningSpeed / timeElapsedRunning; //gets average running speed
        if (averageSpeed <= 8500) //sets percentage based on distance from 0 to 8500. 8500 is considered the perfect run
        {
            powerPercent = averageSpeed / 8500;
        } else //sets from 0 to 4500 for the top part
        {
            powerPercent = 1 - ((averageSpeed - 8500) / 4500);
        }
        float power = 10; //temp
        power *= powerPercent;
        float jumpPercent = 1 - (Math.Abs(100 - jumpMeterSpeed) / 100);
        power *= jumpPercent;
        power += 10;
        player.GetComponentInChildren<Animator>().Play("LongJump");
        player.GetComponentInChildren<Animator>().speed = power * powerToAnimationSpeedRatio;
        player.GetComponent<Rigidbody>().velocity = new Vector3(power, power*0.6f, 0); //make charcter jump
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    private void FixedUpdate()
    {
        if (runningCamera.enabled)
        {
            player.transform.Translate(new Vector3(0, 0, runningSpeed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = runningSpeed * animationRunningSpeedRatio; //making the animation match the sunning speed
            if (runningSpeed == 0)
            {
                totalRunningSpeed = 0;
                timeElapsedRunning = 0;
            } else
            {
                totalRunningSpeed += runningSpeed;
                timeElapsedRunning += Time.deltaTime;
                
            }
        }
    }

    
        
}


//TODO make it so you have to land on the sand
//TODO pr and foul flags
//TODO celebrations after jump
//TODO fix leg pull bugs
//TODO extend sand pit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunningMeterBar : MonoBehaviour
{
    public Image runningBar;

    public float runningSpeed; //public varible to get the running speed
    [SerializeField] private float speedPerClick; //speed added per click of the meter
    public float startingBarHeight; //hold the starting y value of the bar
    [SerializeField] private float maxSpeed;//max speed the player can go
    public float barIncreasePerSpeed; // increase of bar per speed number
    [SerializeField] private float barDecreaseSpeed; //the speed in which the bar decreases
    private float totalRunningSpeed; //total distance travelled for running
    private float timeElapsedRunning; //time that has elepsed for running
    [SerializeField] private float increaseForBarToTheTop;//stores distance from the bottom of the bar to the top


    // Start is called before the first frame update
    void Start()
    {
        startingBarHeight = runningBar.transform.position.y;
        barIncreasePerSpeed = increaseForBarToTheTop / maxSpeed;
    }

    public float getAverageSpeed()
    {
        return totalRunningSpeed / timeElapsedRunning;
    }

    public void increaseHeight() //increases the height of the bar on the run meter
    {
        runningSpeed += speedPerClick;
    }

    public void stopRunMeter() //stops the meter from being visible
    {
        //nothing
    }

    public void updateRunMeter()
    {
        if (runningSpeed <= 0) //makes sure running speed does not go below 0
        {
            runningSpeed = 0;
        }
        else
        {
            runningSpeed -= Time.deltaTime * barDecreaseSpeed; //decreses running speed
        }
        if (runningSpeed > maxSpeed) //making a max speed
        {
            runningSpeed = maxSpeed;
        }
        runningBar.transform.position = new Vector3(runningBar.transform.position.x, startingBarHeight + (runningSpeed * barIncreasePerSpeed), runningBar.transform.position.z);
    }

    public void updateTimeElapsed()
    {
        if (runningSpeed == 0)
        {
            totalRunningSpeed = 0;
            timeElapsedRunning = 0;
        }
        else
        {
            totalRunningSpeed += runningSpeed;
            timeElapsedRunning += Time.deltaTime;

        }
    }

}

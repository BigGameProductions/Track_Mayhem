using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongJumpManager : MonoBehaviour
{
    [SerializeField] private Image runMeterBar; //the bar that is displayed on the run meter
    [SerializeField] private Image jumpMeterBar;//the bar that is displayed on the jump meter
    [SerializeField] private GameObject player; //the player controller

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

    [SerializeField] private float powerToAnimationSpeedRatio;


    private float startingBarHeight; //stores the starting height of the meter bar
    private float barIncreasePerSpeed; //stores the amountd the ui bar increases per speed
    [SerializeField] private float increaseForBarToTheTop;//stores distance from the bottom of the bar to the top
    [SerializeField] private float barDecreaseSpeed;//stores how fast the bar decreases

    // Start is called before the first frame update
    void Start()
    {
        startingBarHeight = runMeterBar.transform.position.y;
        barIncreasePerSpeed = increaseForBarToTheTop / maxSpeed;

        startingJumpBar = jumpMeterBar.transform.position.x;
        jumpBarIncreasePerInteger = jumpMeterToTop / 200f;

        jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter
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
            }
            if (runningSpeed > maxSpeed) //making a max speed
            {
                runningSpeed = maxSpeed;
            }
            runMeterBar.transform.position = new Vector3(runMeterBar.transform.position.x, startingBarHeight + (runningSpeed * barIncreasePerSpeed), runMeterBar.transform.position.z);
            if (Input.GetKeyDown(KeyCode.P)) //if the player presses the jump button
            {
                runningCamera.enabled = false;
                jumpingCamera.enabled = true;
                runMeterBar.transform.parent.gameObject.SetActive(false); //hide run meter
                player.GetComponentInChildren<Animator>().speed = 0; //make running animation stop
                jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(true); //sets the jump meter to showing
            }
        }
        if (jumpMeterBar.gameObject.transform.parent.gameObject.activeInHierarchy)
        {
            
            jumpMeterSpeed += Time.deltaTime * jumpBarSpeed * (jumpMeterDirection ? -1:1);
            if (jumpMeterSpeed >= 200 || jumpMeterSpeed <=0)
            {
                jumpMeterDirection = !jumpMeterDirection;
            }
            jumpMeterBar.transform.position = new Vector3(startingJumpBar + (jumpMeterSpeed * jumpBarIncreasePerInteger), jumpMeterBar.transform.position.y, jumpMeterBar.transform.position.z);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpBarSpeed = 0;
                StartCoroutine(jumpMeterHold(0.5f)); //calls waiting method 
            }
        }
        
    }

    IEnumerator jumpMeterHold(float time) //holds the meter forzen for time so you can she what it landed on
    {
        yield return new WaitForSeconds(time);
        jumpMeterBar.gameObject.transform.parent.gameObject.SetActive(false); //sets the jump meter to hiding
        float power = 10; //temp
        player.GetComponentInChildren<Animator>().Play("LongJump");
        player.GetComponentInChildren<Animator>().speed = power * powerToAnimationSpeedRatio;
        player.GetComponent<Rigidbody>().velocity = new Vector3(power, power*0.75f, 0); //make charcter jump
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    private void FixedUpdate()
    {
        if (runningCamera.enabled)
        {
            player.transform.Translate(new Vector3(0, 0, runningSpeed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = runningSpeed * animationRunningSpeedRatio; //making the animation match the sunning speed
        }
    }

    
        
}

//TODO make the spacebar get in legs
//TODO make faults

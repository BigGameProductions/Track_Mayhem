using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JumpingMeter : MonoBehaviour
{
    public Image jumpBar; //the bar image
    public float jumpMeterSpeed; //current value of the jump meter
    private bool jumpMeterDirection; //the direction of the meter
    private float movingBarSpeed; //the speed back and forth of the jumpm meter
    [SerializeField] float jumpBarSpeed; //the set speed of the jump meter
    private float startingJumpBar; //holds the starting position of the jump bar
    private float jumpBarIncreasePerInteger; //the increase of the bar per speed unit
    [SerializeField] private float jumpMeterToTop;//stores the distance from the begining to the end of the jump meter

    // Start is called before the first frame update
    void Start()
    {
        startingJumpBar = jumpBar.transform.position.x;
        jumpBarIncreasePerInteger = jumpMeterToTop / 200f;
    }

    public void setToRegularSpeed()
    {
        movingBarSpeed = jumpBarSpeed;
    }

    public void updateJumpMeter()
    {
        jumpMeterSpeed += Time.deltaTime * movingBarSpeed * (jumpMeterDirection ? -1 : 1);
        if (jumpMeterSpeed >= 200 || jumpMeterSpeed <= 0)
        {
            if (jumpMeterSpeed >= 200) //prevents bar from going out of bounds
            {
                jumpMeterSpeed = 200;
            }
            else
            {
                jumpMeterSpeed = 0;
            }
            jumpMeterDirection = !jumpMeterDirection;
        }
        jumpBar.transform.position = new Vector3(startingJumpBar + (jumpMeterSpeed * jumpBarIncreasePerInteger), jumpBar.transform.position.y, jumpBar.transform.position.z);
    }

    public void MakeJump()
    {
        movingBarSpeed = 0;
    }
}
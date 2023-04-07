using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainButtonLogic : MonoBehaviour
{
    public void PlayEvent()
    {
        SceneManager.LoadScene("LongJump");
    }

    public void runnerMenu()
    {
        SceneManager.LoadScene("RunnersMenu");
    }
}

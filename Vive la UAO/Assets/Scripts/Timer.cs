using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    [Header("Time UI")]
    public TextMeshProUGUI timerText;
    public GameObject timerObject;

    [Space(20)]
    [Header("Timer Attributes")]
    public float totalTime = 30f;
    public float auxTimer = 10;
    private bool  temp = false;

    [Space(20)]
    public bool isTimeLoaded = false;
    public GetPins yincanaManager;
    private static bool GameManagerExists;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManagerExists) //if GameManagerexcistst is not true --> this action will happen.
        {
            GameManagerExists = true;
            DontDestroyOnLoad(this.gameObject);
            Instance=this;
            //this.gameObject.name = ("Timer Canvas Instance");

        }
        else
        {
            Destroy(gameObject);
            checkPins();
        }

        //yincanaManager = GameObject.Find("Get Pins Script").GetComponent<GetPins>();
    }

    // Update is called once per frame
    void Update()
    {
        checkPins();

        //If previous timer exist, start from the last time
        if (PlayerPrefs.GetInt("Yincana Time") > 0 && !isTimeLoaded)
        {

            totalTime = (float)PlayerPrefs.GetInt("Yincana Time");
            isTimeLoaded = true;
        }


        //If there no Yincana, then asign auxTime to Totaltime
        if (GetPins.yincanaTimer!=0 && !temp)
        {
            totalTime = auxTimer;
            temp=true;
        }


        //If is on Yincana and time is greater than 0
        if (GetPins.isOnYincana && totalTime > 0.5f)
        {
            PlayerPrefs.SetInt("Yincana Time", (int)totalTime);
            resetTime(totalTime);

            totalTime -= Time.deltaTime;
            UpdateLevelTimer(totalTime);
        }
        else
        {
            isTimeLoaded = false;
            PlayerPrefs.SetInt("Yincana Time", 0);
            hideTimer(totalTime);
        }

    }

    void checkPins()
    {
        if (yincanaManager == null && GameObject.Find("Get Pins Script") != null)
        {
            yincanaManager = GameObject.Find("Get Pins Script").GetComponent<GetPins>();
        }
    }

    public void UpdateLevelTimer(float totalMinutes)
    {

        int minutes = Mathf.FloorToInt(totalMinutes / 60f);
        int seconds = Mathf.RoundToInt(totalMinutes % 60f);
        string formatedSeconds = seconds.ToString();

        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");


        if (yincanaManager == null)
        {
            if (minutes == 0 && seconds <= 3)
            {
                PreviousScene.instance.MainScene();
            }
        }
        else
        {
            if (minutes == 0 && seconds <= 1)
            {
                yincanaManager.endYincana();
                hideTimer(auxTimer);
            }
        }
    }

    public void resetTime(float time)
    {
        timerObject.SetActive(true);
        totalTime = time;
    }

    public void hideTimer(float time)
    {
        totalTime = time;
        timerObject.SetActive(false);
    }
}



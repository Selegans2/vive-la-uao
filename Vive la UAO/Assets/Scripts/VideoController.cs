using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;

public class VideoController : MonoBehaviour
{
    private FirebaseAuth auth;
    DatabaseReference mDatabaseRef;
    public GetStationData StationDataScript;

    [Header("States")]
    public bool canSkip;
    public bool isPause;
    public bool isFinish;
    public bool isLoading;
    public bool isResponsive;
    public bool isChallengue;
    [Space(5)]

    public int curFrame;
    public VideoPlayer videoPlayer;
    [Space(10)]

    [Header("UI Components")]
    public Image PauseButton;
    public GameObject progressBar;
    public Text videoTimer;
    public Image videoBar;
    public Slider sliderVideoBar;
    public Image sliderHandle;
    public GameObject Spinner;
    public RawImage rawImage;
    public GameObject canvas;

    [Space(10)]
    public string videoUrl;
    public RenderTexture movieTexture;

    [Space(5)]
    public Sprite[] pauseSprites;
    public Image screenButton;
    public Sprite[] screenButtons;
    [Space(10)]

    [Header("Video Time")]
    string[] timeInfo = new string[4];

    #region Init methods
    //Get the video component in the object
    private void Awake()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        videoBar.fillAmount = 0;
        sliderVideoBar.value = 0;
    }

    private void Start()
    {
        openPlayer();
    }

    private void Update()
    {
        if (videoPlayer.isPlaying)
        {

            SetCurrentTimeUI();
            SetTotalTimeUI();

            checkIfFinish();
            UpdateUI();
        }

        isLoading = checkIfLoading();
    }
    #endregion

    #region Custom Methods
    public void openPlayer()
    {

        isFinish = false;

        if (!canSkip)
        {
            videoPlayer.playOnAwake = true;
            PlayPauseVideo();
        }
        else
        {
            videoPlayer.playOnAwake = false;
        }

        StartCoroutine(PlayVideo());

        if (isChallengue)
        {
            sliderHandle.enabled = false;
            sliderVideoBar.GetComponent<EventTrigger>().enabled = false;
        }
        else
        {
            sliderHandle.enabled = true;
            sliderVideoBar.GetComponent<EventTrigger>().enabled = true;
        }

        //Force Landscape
        if (isResponsive)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    public void setChallengue(bool state)
    {
        isChallengue = state;
    }

    void updateBar()
    {
        videoBar.fillAmount = Mathf.Lerp(0, 1, (float)CalculatePlayedFraction());

        //sliderVideoBar.value = Mathf.Lerp(0, 1, (float)CalculatePlayedFraction());
        sliderVideoBar.value = (float)(videoPlayer.time / videoPlayer.length);
    }

    //Pause the video only if canSkip bolean is true
    public void PlayPauseVideo()
    {
        if (!canSkip)
        {
            return;
        }


        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            curFrame = (int)videoPlayer.frame;
            animatePauseScreen(screenButtons[0]);
            PauseButton.sprite = pauseSprites[0];
            isPause = true;
        }
        else
        {
            isPause = false;
            videoPlayer.Play();

            animatePauseScreen(screenButtons[1]);
            PauseButton.sprite = pauseSprites[1];
            isFinish = false;
        }

    }

    #region Video Time
    void SetCurrentTimeUI()
    {
        string minutes = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
        string seconds = ((int)videoPlayer.time % 60).ToString("00");

        timeInfo[0] = minutes;
        timeInfo[1] = seconds;
    }

    void SetTotalTimeUI()
    {
        string minutes = Mathf.Floor((int)videoPlayer.length / 60).ToString("00");
        string seconds = ((int)videoPlayer.length % 60).ToString("00");

        timeInfo[2] = minutes;
        timeInfo[3] = seconds;
    }

    double CalculatePlayedFraction()
    {
        double fraction = (double)videoPlayer.frame / (double)videoPlayer.frameCount;
        return fraction;
    }

    void checkIfFinish()
    {
        if ((float)CalculatePlayedFraction() >= 0.99f)
        {
            isFinish = true;

            if (isChallengue)
            {
                StationDataScript.SetVideoScore();
                closeVideo();
            }
            PauseButton.sprite = pauseSprites[2];
        }
    }

    public void setVideoTime()
    {
        videoPlayer.time = sliderVideoBar.value * videoPlayer.length;
        PlayPauseVideo();
    }
    #endregion

    bool checkIfLoading()
    {
        bool Loading = false;

        if (videoPlayer.frame < 0 & !isPause)
        {
            Loading = true;
            Spinner.SetActive(true);
        }
        else
        {
            if (videoPlayer.isPlaying)
            {
                if (curFrame == (int)videoPlayer.frame)
                {
                    Loading = true;
                    Spinner.SetActive(true);
                }
                else
                {
                    Loading = false;
                    Spinner.SetActive(false);
                }
            }
        }

        return Loading;
    }


    public void UpdateUI()
    {
        //Set time text component
        videoTimer.text = timeInfo[0] + ":" + timeInfo[1] + " / " + timeInfo[2] + ":" + timeInfo[3];

        if (!isLoading)
        {
            updateBar();
        }

    }

    public void animatePauseScreen(Sprite buttonType)
    {
        Animator anim = screenButton.GetComponent<Animator>();
        anim.SetTrigger("playEffect");
        screenButton.sprite = buttonType;
    }

    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            //break;
        }

        videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = videoUrl;

        rawImage.texture = movieTexture;
        //videoPlayer.Play();
        PlayPauseVideo();
        GetComponent<AudioSource>().Play();
    }


    public void closeVideo()
    {
        Debug.Log("Closing video");

        //Pause video
        videoPlayer.Pause();
        curFrame = (int)videoPlayer.frame;
        animatePauseScreen(screenButtons[0]);
        PauseButton.sprite = pauseSprites[0];
        isPause = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
        canvas.SetActive(false);
    }

    public void openVideo()
    {

        if (!canSkip)
        {
            videoPlayer.playOnAwake = true;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.playOnAwake = false;
        }

        //Reset the video
        videoBar.fillAmount = 0;
        sliderVideoBar.value = 0;

        StartCoroutine(PlayVideo());

        //Force Landscape
        if (isResponsive)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

    }
    #endregion

}
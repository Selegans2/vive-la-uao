using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;


using UnityEngine.Assertions;

public class GetStationData : MonoBehaviour
{
    MeshRenderer meshRender;
    DatabaseReference reference;

    private FirebaseAuth auth;
    public GameObject Spinner;
    public GameObject Video;
    public GameObject ChallengeButton;
    private VideoPlayer videoPlayer;
    public GameObject VideoCanvas;
    public GameObject Title;
    public GameObject Description;
    public static string sNameTapped;
    public static string sDescriptionTapped;
    public static string sVideoUrl;
    private bool challengeBool = false;
    string selectedMechanic;
    public static Texture texturePublic;

    IEnumerator Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://charmander-d429e.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(fixTask =>
        {
            Assert.IsNull(fixTask.Exception);
            auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                Assert.IsNull(authTask.Exception);
            });
        });

        //Get the mesh render component disabled
        meshRender = GetComponent<MeshRenderer>();
        meshRender.enabled = false;
        //Get the video component
        videoPlayer = Video.GetComponent<VideoPlayer>();

        //Activate spinner
        Spinner.SetActive(true);
        yield return StartCoroutine(CheckIfChallenge());
        //Begin to get the image from url
        yield return StartCoroutine(SetDataStation());
    }

    //Checks if there is a challenge available for this station 
    IEnumerator CheckIfChallenge()
    {
        yield return new GetPins.YieldTask(reference.Child("Challenge").Child("Yincana").GetValueAsync().ContinueWith(task3 =>
                {
                    if (task3.IsCompleted)
                    {
                        DataSnapshot SnapShot = task3.Result;
                        if (SnapShot.Child(TapPin.StationTapped.key).Exists == false)
                        {
                            challengeBool = true;
                        }
                    }
                }));

        yield return null;
        if (challengeBool)
        {
            ChallengeButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Does a request to get the image as a texture from the url
    IEnumerator SetDataStation()
    {
        //Does a request to get the image as a texture from the url
        //using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(TapPin.StationTapped.imageUrl))
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(TapPin.StationTapped.imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                //Set the name
                var title = Title.GetComponent<Text>();
                sNameTapped = TapPin.StationTapped.name;
                title.text = sNameTapped;
                //Set the description
                var description = Description.GetComponent<Text>();
                sDescriptionTapped = TapPin.StationTapped.description;
                description.text = sDescriptionTapped;
                //Set the texture
                var texture = DownloadHandlerTexture.GetContent(uwr);
                texturePublic = texture;
                meshRender.enabled = true;
                meshRender.material.SetTexture("_MainTex", texture);
                //Set the video url
                sVideoUrl = TapPin.StationTapped.videoUrl;
                videoPlayer.url = sVideoUrl;
                //Disable the spinner
                Spinner.SetActive(false);
            }
        }
    }

    //Activates the video when trigger
    public void ActivateVideo()
    {
        VideoCanvas.SetActive(true);
        Video.GetComponent<VideoController>().openPlayer();
    }

    //Call coroutine to access the challenge
    public void GoToChallenge()
    {
        StartCoroutine(GetSettedChallenge());
    }

    //Score video handler
    public async void SetVideoScore()
    {
        int videoScore= 50;
        int totalScore = PlayerPrefs.GetInt("Total Score") + videoScore;
        PlayerPrefs.SetInt("Total Score", totalScore);
        if (PlayerPrefs.GetString("Yincana").Length != 0)
        {
            await reference.Child("Groups").Child(PlayerPrefs.GetString("Yincana")).Child("groups").Child(PlayerPrefs.GetString("Group")).Child("stations score").Child(TapPin.StationTapped.key).SetValueAsync(videoScore);
            GetPins.getStationsYincana = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
        }
    }

    //Get setted challenge 
    private IEnumerator GetSettedChallenge()
    {
        if (PlayerPrefs.GetString("Yincana").Length != 0 || PlayerPrefs.GetString("Yincana").Length == 0)
        {
            yield return new GetPins.YieldTask(reference.Child("Challenge").Child("Yincana").Child(TapPin.StationTapped.key).Child("selected").GetValueAsync().ContinueWith(task3 =>
                {
                    if (task3.IsCompleted)
                    {
                        //Save the selected mechanic
                        selectedMechanic = task3.Result.Value.ToString();
                    }
                }));
            yield return null;
            switch (selectedMechanic)
            {
                case "trivia":
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Quiz Scene");
                    break;
                case "video":
                    VideoCanvas.SetActive(true);
                    Video.GetComponent<VideoController>().isChallengue = true;
                    Video.GetComponent<VideoController>().openPlayer();
                    break;
                case "memory":
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Memory Scene");
                    break;
                case "tips":
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Tips Scene");
                    break;
                case "ghost":
                    if (PlayerPrefs.GetString("Yincana").Length == 0)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Quiz Scene");
                    else
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Score Validation");
                    break;
                default:
                    break;
            }
        }
        else
            Debug.Log("Recorrido exist :c");

        Debug.Log("selectedMechanic: " + TapPin.StationTapped.key);
        Debug.Log("selectedMechanic: " + selectedMechanic);
    }
}
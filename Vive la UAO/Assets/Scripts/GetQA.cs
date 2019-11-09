using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GetQA : MonoBehaviour
{

    private FirebaseAuth auth;
    private DatabaseReference reference;
    private List<QAStructure> QAList = new List<QAStructure>();
    private List<int> questionsAskedList = new List<int>();
    private AudioSource efxSource;
    public AudioSource efxWrong;
    public AudioSource efxCorrect;
    private string correct;
    private string question;
    public Text timerText;
    public GameObject answersPrefab;
    public GameObject answersBoxParent;
    public GameObject CorrectCanvas;
    public GameObject QuestionPrefab;
    public GameObject ScoreCanvas;
    public GameObject WrongCanvas;
    public GameObject Score;
    public GameObject finalScorePrefab;
    public GameObject cardParent;

    public GameObject Spinner;
    bool questionAsked = false;
    int questionsAllowed = 3;
    bool getText = false;
    bool triviaDone = false;
    private float playEverySeconds = 1;
    private float timePassed = 0;
    float timerFirebase;
    float timer = 20;
    int stationScore;
    int totalScore;
    int score = 0;
    string pickedAnswer;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://charmander-d429e.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(fixTask =>
        {
            Assert.IsNull(fixTask.Exception);
            auth = FirebaseAuth.DefaultInstance;

            Debug.Log(auth.CurrentUser);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                Assert.IsNull(authTask.Exception);
            });
        });
        Spinner.SetActive(true);
        efxSource = GetComponent<AudioSource>();
        //yield return StartCoroutine(GetTriviaFirebaseTimer());
        yield return StartCoroutine(SetUpTriviaQATimer());
        yield return StartCoroutine(GetQAData());
    }

    // Update is called once per frame
    void Update()
    {
        if (Spinner.activeSelf==false)
        {
            if (timer < 0)
            {
                efxSource.Stop();
                ValidateQuestion();
            }
            AsignText();
        }

    }

    //Method to retrieve the timer of each question from Firebase
    private IEnumerator SetUpTriviaQATimer()
    {
        yield return new YieldTask(reference.Child("Challenge").Child("Yincana").Child(TapPin.StationTapped.key).GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
               Debug.Log("Error");
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               timerFirebase = float.Parse(snapshot.Child("timer").Value.ToString());
               timer = timerFirebase;
               questionsAllowed = int.Parse(snapshot.Child("numQA").Value.ToString());
           }
       }));
    }

    private IEnumerator GetQAData()
    {
        yield return new YieldTask(reference.Child("Challenge").Child("Yincana").Child(TapPin.StationTapped.key).Child("trivia").Child("QA").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot QA in snapshot.Children) //QA
                {
                    List<string> answersList = new List<string>();
                    foreach (DataSnapshot node in QA.Children) //nodes 
                    {
                        foreach (DataSnapshot answers in node.Children) //answers
                        {
                            answersList.Add(answers.Value.ToString());
                        }
                        if (node.Key.ToString() == "question") //question
                        {
                            question = node.Value.ToString();
                        }
                        if (node.Key.ToString() == "correct") //correct
                        {
                            correct = node.Value.ToString();
                        }
                    }
                    QAList.Add(new QAStructure(question, correct, answersList));
                }
                //Instruction to instantiate objects
                getText = true;
            }
        }));
        yield return null;
        Spinner.SetActive(false);
    }

    public void AsignText()
    {
        timer -= Time.deltaTime;
        int timerInt = (int)timer;
        timePassed += Time.deltaTime;

        //timer
        if (timePassed >= playEverySeconds && triviaDone == false)
        {
            timePassed = 0;
            efxSource.Play();
        }

        //
        timerText.text = "" + timerInt.ToString();

        if (getText == true && questionsAskedList.Count <= questionsAllowed)
        {
            int randomNumber = Random.Range(0, QAList.Count);
            if (questionsAskedList.Count > 0)
            {
                for (int i = 0; i <= questionsAskedList.Count - 1; i++)
                {
                    if (randomNumber == questionsAskedList[i])
                    {
                        questionAsked = true;
                        return;
                    }
                    else
                    {
                        questionAsked = false;
                    }

                }
            }
            if (!questionAsked)
            {
                foreach (string al in QAList[randomNumber].answersList)
                {
                    var tempp = Instantiate(answersPrefab);
                    tempp.GetComponent<Text>().text = al;
                    tempp.transform.localScale = new Vector3(2, 2, 2);
                    tempp.transform.SetParent(answersBoxParent.gameObject.transform);
                }
                question = QAList[randomNumber].question;
                QuestionPrefab.GetComponent<Text>().text = question;
                correct = QAList[randomNumber].correct;
                getText = false;
            }
            else
            {
                getText = true;
            }
            questionsAskedList.Add(randomNumber);
            if (questionsAskedList.Count > questionsAllowed)
            {
                triviaDone = true;
                stationScore = score;
                totalScore = PlayerPrefs.GetInt("Total Score") + stationScore;
                PlayerPrefs.SetInt("Total Score", totalScore);
                ScoreCanvas.SetActive(true);
                finalScorePrefab.GetComponent<Text>().text = score.ToString();
            }
        }
    }
    public void ValidateQuestion()
    {
        timer = timerFirebase;
        timePassed = 0;
        CorrectCanvas.SetActive(false);
        WrongCanvas.SetActive(false);
        QuestionPrefab.GetComponent<Text>().text = "";
        foreach (Transform child in cardParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in answersBoxParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        getText = true;
        if (pickedAnswer == correct)
        {
            score += 30;
            efxCorrect.Play();
            Score.GetComponent<Text>().text = score.ToString();
            CorrectCanvas.SetActive(true);
        }
        else
        {
            efxWrong.Play();
            WrongCanvas.SetActive(true);
            if (score <= 10)
                score = 0;
            else
                score -= 10;
            Score.GetComponent<Text>().text = score.ToString();
        }
    }

    public async void RegisterScore()
    {
        //reset
        questionsAskedList.Clear();
        QAList.Clear();
        score = 0;
        Score.GetComponent<Text>().text = stationScore.ToString();
        //ScoreCanvas.SetActive(false);
        if (PlayerPrefs.GetString("Yincana").Length != 0)
        {
            await reference.Child("Groups").Child(PlayerPrefs.GetString("Yincana")).Child("groups").Child(PlayerPrefs.GetString("Group")).Child("stations score").Child(TapPin.StationTapped.key).SetValueAsync(stationScore);
            GetPins.getStationsYincana = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
    }

    //Fills the answer picked
    public void ToggleHandler(string sendedAnswer)
    {
        pickedAnswer = sendedAnswer;
    }


    //Method to wait for a task to complete
    public class YieldTask : CustomYieldInstruction
    {
        public YieldTask(Task task)
        {
            Task = task;
        }

        public override bool keepWaiting => !Task.IsCompleted;

        public Task Task { get; }
    }
}

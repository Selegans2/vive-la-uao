using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine.Assertions;
using System.Threading.Tasks;

using UnityEngine.Networking;

public class MemoryGameManager : MonoBehaviour
{
    [Header("UI")]
    public List<card> Cards = new List<card>();
    public Text score;
    public Text timer;
    public GridLayoutGroup grid;
    public GameObject blockPanel;
    public GameObject avisoTiempo;
    [Space(15)]
    public bool isWin;
    private int totalAmount;
    public int currentScore = 0;
    private int countDown;
    [Space(15)]

    public int cardsSelected = 0;
    public int firstCard = -1;
    public int lastCard = -1;
    [Space(15)]

    public int firstCardIndex = -1;
    public int lastCardIndex = -1;

    private DatabaseReference reference;
    private FirebaseAuth auth;
    private DataSnapshot Snapshot;
    public GameObject Spinner;


    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://charmander-d429e.firebaseio.com/");
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // Check Firebase dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(fixTask =>
        {
            Assert.IsNull(fixTask.Exception);
            auth = FirebaseAuth.DefaultInstance;

            //Debug.Log(auth.CurrentUser);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                Assert.IsNull(authTask.Exception);
            });
        });
        Spinner.SetActive(true);
        StartCoroutine(GetMemoryUrlImagesList());
    }
    void Update()
    {
        Debug.Log(currentScore);
    }

    #region Custom Methods
    public void ShowCard(int index)
    {

        cardsSelected++;
        if (cardsSelected > 1)
        {
            lastCardIndex = Cards[index].cardIndex;
            lastCard = index;

            //Check if the cards are the same or not
            if (firstCardIndex == lastCardIndex)
            {
                Debug.Log("Are the same!");

                currentScore++;
                updateUI();

                HideCards(firstCard, lastCard);
            }
            else
            {
                Debug.Log("You fail!");
                StartCoroutine(SwipeCardsSmooth(firstCard, lastCard));

            }

            cardsSelected = 0;

            firstCard = -1;
            lastCard = -1;

            firstCardIndex = -1;
            lastCardIndex = -1;
        }
        else
        {
            firstCard = index;
            firstCardIndex = Cards[index].cardIndex;
        }

        if (currentScore >= (totalAmount / 2))
        {
            currentScore = currentScore;
            avisoTiempo.GetComponent<Text>().text = "¡ GANASTE !";
            isWin = true;
           // if (PlayerPrefs.GetString("Yincana").Length != 0 || PlayerPrefs.GetString("Recorrido").Length != 0)
            
                totalScore = (PlayerPrefs.GetInt("Total Score") + currentScore);
                Debug.Log("totalScore:"+totalScore);
                RegisterScore();
                PlayerPrefs.SetInt("Total Score", totalScore);
            
            ScoreCanvas.SetActive(true);
            scoreText.text = currentScore.ToString();
        }
        StartCoroutine(ShowCardSmooth(index));
    }
    int totalScore;
    public GameObject ScoreCanvas;
    public Text scoreText;

    public async void RegisterScore()
    {
        if (PlayerPrefs.GetString("Yincana").Length != 0)
        {
            await reference.Child("Groups").Child(PlayerPrefs.GetString("Yincana")).Child("groups").Child(PlayerPrefs.GetString("Group")).Child("stations score").Child(TapPin.StationTapped.key).SetValueAsync(currentScore);
            GetPins.getStationsYincana = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
    }

    private IEnumerator ShowCardSmooth(int index)
    {
        card tempCar = Cards[index];
        float duration = 0.35f;

        #region Smooth rotation
        Quaternion initRot = Quaternion.identity;
        Quaternion endRot = Quaternion.identity;

        if (!tempCar.isShow)
        {
            initRot = Quaternion.Euler(0, 0, 0);
            endRot = Quaternion.Euler(0, 181, 0);
        }
        else
        {
            initRot = Quaternion.Euler(0, 181, 0);
            endRot = Quaternion.Euler(0, 0, 0);
        }

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            tempCar.cardObject.GetComponent<RectTransform>().rotation = Quaternion.Slerp(initRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;

            if (elapsed >= (duration / 2))
            {
                if (!tempCar.isShow)
                {
                    tempCar.frontSide.SetActive(true);
                    tempCar.backSide.SetActive(false);
                    tempCar.cardObject.GetComponent<Button>().interactable = false;
                }
                else
                {
                    tempCar.frontSide.SetActive(false);
                    tempCar.backSide.SetActive(true);
                    tempCar.cardObject.GetComponent<Button>().interactable = true;
                }
            }

            yield return null;
        }
        transform.rotation = endRot;
        #endregion

        tempCar.isShow = !tempCar.isShow;
    }

    private IEnumerator SwipeCardsSmooth(int index1, int index2)
    {
        yield return new WaitForSeconds(1.2f);

        card tempCar = Cards[index1];
        card tempCar2 = Cards[index2];

        float duration = 0.65f;

        #region Smooth rotation
        Quaternion initRot = Quaternion.identity;
        Quaternion endRot = Quaternion.identity;

        initRot = Quaternion.Euler(0, 181, 0);
        endRot = Quaternion.Euler(0, 0, 0);

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            tempCar.cardObject.GetComponent<RectTransform>().rotation = Quaternion.Slerp(initRot, endRot, elapsed / duration);
            tempCar2.cardObject.GetComponent<RectTransform>().rotation = Quaternion.Slerp(initRot, endRot, elapsed / duration);

            elapsed += Time.deltaTime;

            if (elapsed >= (duration / 2))
            {
                tempCar.frontSide.SetActive(false);
                tempCar.backSide.SetActive(true);
                tempCar.cardObject.GetComponent<Button>().interactable = true;

                tempCar2.frontSide.SetActive(false);
                tempCar2.backSide.SetActive(true);
                tempCar2.cardObject.GetComponent<Button>().interactable = true;
            }

            yield return null;
        }

        tempCar.cardObject.GetComponent<RectTransform>().rotation = endRot;
        tempCar2.cardObject.GetComponent<RectTransform>().rotation = endRot;

        #endregion

        tempCar.isShow = !tempCar.isShow;
        tempCar2.isShow = !tempCar2.isShow;
    }


    public void showAllCards()
    {
        //StartCoroutine(showAllCardsSmooth());
        for (int i = 0; i < Cards.Count; i++)
        {
            StartCoroutine(showAllCardsSmooth(i));
        }
    }

    IEnumerator showAllCardsSmooth(int index)
    {
        yield return new WaitForSeconds(0.8f);

        StartCoroutine(ShowCardSmooth(index));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(ShowCardSmooth(index));

        yield return new WaitForSeconds(0.8f);
        blockPanel.SetActive(false);
        StartCoroutine(setTimer());
    }


    public void HideCards(int index1, int index2)
    {
        StartCoroutine(HideCardsSmooth(index1, index2));
    }

    private IEnumerator HideCardsSmooth(int index1, int index2)
    {

        yield return new WaitForSeconds(0.5f);

        float duration = 0.75f;
        card tempCar1 = Cards[index1];
        card tempCar2 = Cards[index2];

        #region Smooth Dissolve
        float elapsed = 0.0f;
        while (elapsed < duration)
        {

            tempCar1.frontSide.GetComponent<UIDissolve>().effectFactor = Mathf.Lerp(0, 1, elapsed / duration);
            tempCar1.frontSide.transform.GetChild(0).GetComponent<UIDissolve>().effectFactor = Mathf.Lerp(0, 1, elapsed / duration);

            tempCar2.frontSide.GetComponent<UIDissolve>().effectFactor = Mathf.Lerp(0, 1, elapsed / duration);
            tempCar2.frontSide.transform.GetChild(0).GetComponent<UIDissolve>().effectFactor = Mathf.Lerp(0, 1, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        tempCar1.frontSide.GetComponent<UIDissolve>().effectFactor = 1;
        tempCar1.frontSide.transform.GetChild(0).GetComponent<UIDissolve>().effectFactor = 1;
        tempCar1.cardObject.GetComponent<Button>().interactable = false;
        tempCar1.backSide.SetActive(false);

        tempCar2.frontSide.GetComponent<UIDissolve>().effectFactor = 1;
        tempCar2.frontSide.transform.GetChild(0).GetComponent<UIDissolve>().effectFactor = 1;
        tempCar2.cardObject.GetComponent<Button>().interactable = false;
        tempCar2.backSide.SetActive(false);
        #endregion
    }


    private IEnumerator setTimer()
    {
        #region Countdown

        float duration = countDown;
        float elapsed = 0.0f;
        while ((elapsed < countDown) && (!isWin))
        {
            int timeValue = (int)Mathf.Lerp(countDown, 0, (elapsed / duration));
            timer.text = timeValue.ToString();
            elapsed += Time.deltaTime;
            yield return null;
        }

        blockPanel.SetActive(true);
        //showAllCards();
        avisoTiempo.SetActive(true);
        timer.text = 0.ToString();

        //Show score and canvas
        totalScore = PlayerPrefs.GetInt("Total Score") + currentScore;
        PlayerPrefs.SetInt("Total Score", totalScore);
        ScoreCanvas.SetActive(true);
        scoreText.text = currentScore.ToString();
        #endregion
    }


    public void updateUI()
    {
        score.text = currentScore.ToString() + " / " + (totalAmount / 2);
    }

    public void getCardsComponents()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].cardSprite = Cards[i].cardObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            Cards[i].frontSide = Cards[i].cardObject.transform.GetChild(0).gameObject;
            Cards[i].backSide = Cards[i].cardObject.transform.GetChild(1).gameObject;
        }

        //Desactivate un-used cards
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (i >= totalAmount)
            {
                grid.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //Organize Grid
        switch (totalAmount)
        {
            case 6:
                grid.constraintCount = 3;
                break;

            case 8:
                grid.constraintCount = 4;
                break;

            case 10:
                grid.constraintCount = 5;
                break;

            case 12:
                grid.constraintCount = 4;
                break;
        }
    }

    public void randomizeObjects()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            GameObject tempCard = Cards[i].cardObject;
            int randomIndex = Random.Range(0, Cards.Count);
            Cards[i].cardObject.transform.SetSiblingIndex(randomIndex);
        }
    }
    #endregion

    private List<string> urlList = new List<string>();

    //Get list of URLs of the images found in the 'memory' node
    IEnumerator GetMemoryUrlImagesList()
    {
        yield return new YieldTask(reference.Child("Challenge").Child("Yincana").Child(TapPin.StationTapped.key).Child("memory").GetValueAsync().ContinueWith(task2 =>
           {
               if (task2.IsCompleted)
               {
                   Snapshot = task2.Result;
                   totalAmount = int.Parse(Snapshot.Child("matrix").Value.ToString()) * 2;
                   countDown = int.Parse(Snapshot.Child("timer").Value.ToString());
                   foreach (DataSnapshot im in Snapshot.Child("img").Children) // url images
                   {
                       string imageUrl = im.Value.ToString();
                       urlList.Add(imageUrl);
                   }
               }
           }));
        yield return null;
        getCardsComponents();

        yield return StartCoroutine(SetSprite());
    }
    int index = 0;
    int cont = 0;
    IEnumerator SetSprite()
    {
        foreach (string url in urlList)
        {
            //Does a request to get the image as a texture from the url
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    //Set the texture
                    var texture = DownloadHandlerTexture.GetContent(uwr);

                    //Get the current index of the card
                    var indexCard = Cards[cont].cardIndex;
                    if (indexCard == index)
                    {
                        //Create the sprite
                        var mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                        Image spriteCard;
                        Image spriteCard2;
                        //Set the sprite to the element in the index position and the next to it (to the couple)
                        if (index == 0)
                        {
                            spriteCard = Cards[index].cardSprite;
                            spriteCard2 = Cards[index + 1].cardSprite;
                        }
                        else
                        {
                            spriteCard = Cards[cont].cardSprite;
                            spriteCard2 = Cards[cont + 1].cardSprite;
                        }
                        spriteCard.sprite = mySprite;
                        spriteCard2.sprite = mySprite;
                    }
                    index++;
                    cont = cont + 2;
                }
            }
        }
        //Continue after the images has been setted correctly
        Spinner.SetActive(false);
        randomizeObjects();
        updateUI();
        showAllCards();
        timer.text = (countDown).ToString();
    }

    [System.Serializable]
    public class card
    {
        public GameObject cardObject;
        public int cardIndex;


        public Image cardSprite;

        public GameObject frontSide;

        public GameObject backSide;

        public bool isShow = false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Globalization;
using System.Linq;
using UnityEngine.Assertions;


using System.Threading.Tasks;

using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;

public class GetPins : MonoBehaviour
{
    #region variables declaration
    #region firebase
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private DataSnapshot Snapshot;
    #endregion
    #region dictionaries
    private IDictionary station = new Dictionary<string, Object>();
    private IDictionary area = new Dictionary<string, Object>();
    #endregion
    #region lists
    public static bool isOnYincana = false;

    [Space(10)]
    public static List<Station> stationList = new List<Station>();
    public static List<Area> areaList = new List<Area>();
    public static List<string> stationsFinishedList = new List<string>();
    public static List<string> participantsNameList = new List<string>();
    #endregion
    #region GameObjects

    public InterfaceController ShowYinfanaNotFound;
    public GameObject AreaPrefab;
    public GameObject StationPrefab;
    public GameObject Pinx1;
    public GameObject Pinx2;
    public GameObject Pinx3;

    [Space(5)]
    public GameObject YincanaWelcomeCanvas;
    public GameObject YincanaFinishedCanvas;
    public GameObject YincanaCanvas;
    public GameObject FreeWelcomeCanvas;

    [Space(5)]
    public GameObject yincanaPoints;
    public GameObject RefreshButton;
    public GameObject ChatMessage;



    [Space(5)]
    public Text testText;
    #endregion
    #region variables
    bool getAreas = false;
    bool getStations = false;
    public static bool getStationsYincana = false;
    string yincanaName;
    string storedYincanaKey;
    bool yfound;
    int sequence;
    private string group;
    bool nextStationFound = false;
    bool stationsFinishedExist;
    int showYincanaWelcomeCanvasOnce = 0;
    int showFreeWelcomeCanvasOnce = 0;
    string groupName;
    string nameParticipant;
    string email;
    string age;
    string phone;
    int participantsCount = 0;
    public bool DisableYincanaTimer = false;
    bool yincanaState;
    public static float yincanaTimer = 0;
    bool yincanaStart = false;

    float timer = 0;
    public GameObject ParticipantName;
    public GameObject LayoutNames;

    [Space(15)]
    [Header("Group Variables")]
    public GameObject GroupCanvas;
    public GameObject groupNameBox;
    public GameObject groupNameCanvas;
    public GameObject groupRegisterCanvas;
    public GameObject DataPoliticsCanvas;
    public Toggle politicsToggle;
    private List<Participants> participantsList = new List<Participants>();
    public GameObject groupNameField;
    public GameObject pFieldName;
    public GameObject pFieldEmail;
    public GameObject pFieldAge;
    public GameObject pFieldPhone;
    public GameObject ParticipantsCount;
    public GameObject YincanaConfirmationModal;
    public GameObject Toast;
    [Space(15)]

    #endregion
    #region audios
    private AudioSource campusSoundAudioSource;
    #endregion
    #endregion

    //private static IDictionary picturesDictionary = new Dictionary<string, string>();
    // Start is called before the first frame update
    IEnumerator Start()
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
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                Assert.IsNull(authTask.Exception);
            });
        });
        /* 
        if (picturesDictionary["hey"] == null)
        {
            Debug.Log("hey is null");
            picturesDictionary.Add("hey", "macintosh");
        }
        if (picturesDictionary["hey"] != null)
        {
            Debug.Log("hey is not null now");
        }
        */
        //Handle if Recorrido is active
        if (PlayerPrefs.GetString("Recorrido").Length != 0)
        {
            stationList.Clear();
            yield return StartCoroutine(RetrieveDataYincanaCoroutine());
        }
        //Handle if Yincana is active
        else if (PlayerPrefs.GetString("Yincana").Length != 0)
        {
            YincanaCanvas.SetActive(true);
            yincanaPoints.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Total Score").ToString();
            //Handle welcome interface to show only the first time
            if (PlayerPrefs.GetInt("Yincana Welcome") < 1)
            {
                YincanaWelcomeCanvas.SetActive(true);
            }
            else
            {
                YincanaWelcomeCanvas.SetActive(false);
            }
            stationList.Clear();
            //Execute method to bring only stations
            getStationsYincana = true;
            //Set variable to use yincana name
            yincanaName = PlayerPrefs.GetString("Yincana");
            yield return StartCoroutine(RetrieveDataYincanaCoroutine());

            //Show welcome canvas once
            showYincanaWelcomeCanvasOnce++;
            PlayerPrefs.SetInt("Yincana Welcome", showYincanaWelcomeCanvasOnce);

            yield return StartCoroutine(RefreshYincana());
        }
        //Handle if there's no yincana or route active
        else
        {

            //Handle welcome interface to show only the first time
            if (PlayerPrefs.GetInt("Free Ride Welcome") < 1)
            {
                FreeWelcomeCanvas.SetActive(true);
            }
            else
            {
                FreeWelcomeCanvas.SetActive(false);
            }
            yield return StartCoroutine(RetrieveDataCoroutine());
            //Show welcome canvas once
            showFreeWelcomeCanvasOnce++;
            PlayerPrefs.SetInt("Free Ride Welcome", showFreeWelcomeCanvasOnce);
        }
        campusSoundAudioSource = GetComponent<AudioSource>();
        campusSoundAudioSource.Play();
        //testText.text = System.DateTime.UtcNow.ToString("dd,MM,yyyy");
    }

    void Update()
    {
        Debug.Log("sequence: " + sequence);
        //If yincana is enabled
        if (DisableYincanaTimer == false && PlayerPrefs.GetString("Yincana").Length != 0)
        {
            isOnYincana = true;
        }
        else
        {
            isOnYincana = false;
        }
        //Debug.Log(stationList[0].x);
    }

    public void endYincana()
    {
        Timer.temp = false;
        StartCoroutine(SetFinalScore());
        YincanaFinishedCanvas.SetActive(true);
        yincanaState = false;
        isOnYincana = false;
        stationsFinishedList.Clear();
        DisableYincanaTimer = true;
    }

    //////--------------

    //Show a button to refresh after seconds if Yincana pins aren't instantiate
    private IEnumerator RefreshYincana()
    {
        yield return new WaitForSeconds(3);
        if (Pinx3.transform.childCount == 0)
            RefreshButton.SetActive(true);
    }

    //Set yincana name
    private IEnumerator SetYincanaCoroutine()
    {
        //Saves sequence in phone
        PlayerPrefs.SetInt("Sequence", sequence);
        //Saves group in phone
        PlayerPrefs.SetString("Group", group);
        yield return null;
        //Get stations of yincana that was found with the name
        yield return new YieldTask(reference.Child("Yincanas").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsCompleted)
            {
                Snapshot = task2.Result;
                //Set Timer of yincana yincanatimer
                yincanaTimer = float.Parse(Snapshot.Child(storedYincanaKey).Child("Timer").Value.ToString());
                Timer.Instance.auxTimer = yincanaTimer;
                foreach (DataSnapshot st in Snapshot.Child(storedYincanaKey).Child("stations").Children) // stations of yincana
                {
                    IDictionary yinStations = (IDictionary)(st.Value);
                    string key = st.Key.ToString();
                    string description = yinStations["description"].ToString();
                    string imageUrl = yinStations["imageUrl"].ToString();
                    string videoUrl = yinStations["videoUrl"].ToString();
                    string stationName = yinStations["name"].ToString();
                    float x = float.Parse(yinStations["x"].ToString(), CultureInfo.InvariantCulture) - 32;
                    //float x = float.Parse(station["x"].ToString(),CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    float y = float.Parse(yinStations["y"].ToString(), CultureInfo.InvariantCulture);
                    float z = float.Parse(yinStations["z"].ToString(), CultureInfo.InvariantCulture);
                    //string place = yinStations["place"].ToString();
                    stationList.Add(new Station(key, yinStations["name"].ToString(), x, y, z, description, imageUrl, videoUrl, "0"));
                    //stationList.Add(new Station(key, yinStations["name"].ToString(), x, y, z, description, imageUrl, videoUrl, place));
                }
            }
        }));

        //Uploads the stations of yincana in the groups node only if it doesn't exist 
        yield return new YieldTask(reference.Child("Groups").Child(yincanaName).GetValueAsync().ContinueWith(task3 =>
        {
            if (task3.IsCompleted)
            {
                DataSnapshot SnapShot = task3.Result;
                //Uploads the stations to the Group-yincana name-stations node
                foreach (Station st in stationList)
                {
                    string station = "station" + stationList.IndexOf(st) + "/";
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("description").SetValueAsync(st.description);
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("imageUrl").SetValueAsync(st.imageUrl);
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("videoUrl").SetValueAsync(st.videoUrl);
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("name").SetValueAsync(st.name);
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("x").SetValueAsync(st.x.ToString());
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("y").SetValueAsync(st.y.ToString());
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("z").SetValueAsync(st.z.ToString());
                    reference.Child("Groups").Child(yincanaName).Child("stations").Child(station).Child("place").SetValueAsync(st.place.ToString());
                }
            }
        }));

        //Change to main scene to load animation
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
        yield return null;
    }

    // Get data from firebase to fill list of areas and stations
    private IEnumerator RetrieveDataCoroutine()
    {
        //Get areas data
        yield return new YieldTask(reference.Child("Areas").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                areaList.Clear();
                Snapshot = task.Result;
                foreach (DataSnapshot info in Snapshot.Children)
                {
                    area = (IDictionary)(info.Value);
                    string key = info.Key.ToString();
                    string description = area["description"].ToString();
                    string imageUrl = area["imageUrl"].ToString();
                    string stationName = area["name"].ToString();
                    float x = float.Parse(area["x"].ToString(), CultureInfo.InvariantCulture);
                    float y = float.Parse(area["y"].ToString(), CultureInfo.InvariantCulture);
                    float z = float.Parse(area["z"].ToString(), CultureInfo.InvariantCulture);
                    areaList.Add(new Area(area["name"].ToString(), x, y, z, description, imageUrl));
                }
                //Instruction to instantiate areas
                getAreas = true;
            }
        }));

        //Get stations data
        yield return new YieldTask(reference.Child("Stations").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                stationList.Clear();
                Snapshot = task.Result;
                foreach (DataSnapshot info in Snapshot.Children)
                {
                    station = (IDictionary)(info.Value);
                    string key = info.Key.ToString();
                    string description = station["description"].ToString();
                    string imageUrl = station["imageUrl"].ToString();
                    string videoUrl = station["videoUrl"].ToString();
                    string stationName = station["name"].ToString();
                    float x = float.Parse(station["x"].ToString(), CultureInfo.InvariantCulture);
                    float y = float.Parse(station["y"].ToString(), CultureInfo.InvariantCulture);
                    float z = float.Parse(station["z"].ToString(), CultureInfo.InvariantCulture);
                    string place = station["place"].ToString();
                    stationList.Add(new Station(key, station["name"].ToString(), x, y, z, description, imageUrl, videoUrl, place));
                    //stationList.Add(new Station(key, station["name"].ToString(), x, y, z, description, imageUrl, videoUrl, place));
                }
                //Instruction to instantiate stations
                getStations = true;
            }
        }));

        yield return null;
        //Execute method
        InstantiatePins();
    }

    // Get data from Firebase to fill list of stations of yincana
    private IEnumerator RetrieveDataYincanaCoroutine()
    {
        //Get and fill the station list with the data of the yincana
        yield return new YieldTask(reference.Child("Groups").Child(yincanaName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot st in snapshot.Child("stations").Children)
                {
                    IDictionary yinStations = (IDictionary)(st.Value);
                    string key = st.Key.ToString();
                    string description = yinStations["description"].ToString();
                    string imageUrl = yinStations["imageUrl"].ToString();
                    string videoUrl = yinStations["videoUrl"].ToString();
                    string stationName = yinStations["name"].ToString();
                    float x = float.Parse(yinStations["x"].ToString(), CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    float y = float.Parse(yinStations["y"].ToString(), CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    float z = float.Parse(yinStations["z"].ToString(), CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    //float x = float.Parse(yinStations["x"].ToString(), CultureInfo.InvariantCulture);
                    //float y = float.Parse(yinStations["y"].ToString(), CultureInfo.InvariantCulture);
                    //float z = float.Parse(yinStations["z"].ToString(), CultureInfo.InvariantCulture);
                    string place = yinStations["place"].ToString();
                    stationList.Add(new Station(key, yinStations["name"].ToString(), x, y, z, description, imageUrl, videoUrl, place));
                    //stationList.Add(new Station(key, yinStations["name"].ToString(), x, y, z, description, imageUrl, videoUrl, place));
                }
            }
        }));

        yield return StartCoroutine(ReOrderList());
        //reasures that the participantsNameList is not have been filled
        if (participantsNameList.Count == 0)
            yield return StartCoroutine(SetParticipantsNames());

        if (PlayerPrefs.GetString("Yincana").Length != 0 && PlayerPrefs.GetString("Group").Length != 0)
        {
            Instantiate(ChatMessage);
        }
        InstantiatePins();
    }

    //Get group participants names list
    private IEnumerator SetParticipantsNames()
    {
        string currentGroup = PlayerPrefs.GetString("Group");
        //Get stations data
        yield return new YieldTask(reference.Child("Groups").Child(yincanaName).Child("groups").Child(currentGroup)
        .Child("participants").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Snapshot = task.Result;
                foreach (DataSnapshot pt in Snapshot.Children)
                {
                    IDictionary participants = (IDictionary)(pt.Value);
                    string name = participants["name"].ToString();
                    participantsNameList.Add(name);
                }
            }
        }));
    }

    //Instantiate pins
    public void InstantiatePins()
    {
        #region Mode: Yincana
        if (getStationsYincana == true)
        {
            foreach (Transform child in LayoutNames.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (string pt in participantsNameList)
            {
                var temp = Instantiate(ParticipantName);
                //temp.transform.parent = LayoutNames.transform;
                temp.transform.SetParent(LayoutNames.transform, false);
                var participantTextName = temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                participantTextName.text = pt;
            }
            foreach (Station st in stationList)
            {
                //If yincana is over
                if (stationsFinishedList.Count == stationList.Count)
                {
                    //Show canvas that ends yincana!
                    Debug.Log("your yincana has endend!");
                    StartCoroutine(SetFinalScore());
                    YincanaFinishedCanvas.SetActive(true);
                    yincanaState = false;
                    stationsFinishedList.Clear();
                    break;
                }
                //Set the yincana and continue it
                else
                {
                    var temp = Instantiate(StationPrefab, new Vector3(st.x, st.y, st.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    temp.name = st.name;
                    var stationTextName = temp.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                    stationTextName.text = st.name;
                    //temp.transform.SetParent(Pinx3.transform, false);
                    //Disable the name interface
                    temp.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

                    //first set all stations disabled to transparency 0.5 and no script
                    var stationAnimated = temp.transform.GetChild(0).GetComponent<Animator>();
                    TapPin TapPinScript = temp.transform.GetChild(0).GetChild(1).GetComponent<TapPin>();
                    TapPinScript.enabled = false;
                    var color = temp.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color;
                    color.a = 0.5f;
                    temp.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = color;
                    //If the stations has score, compare the list of the stations that has a score with the first element
                    if (stationsFinishedExist)
                    {
                        //find station inside the list, first element always will be the one needed to compare
                        string stationFound = null;
                        stationFound = stationsFinishedList.Find((x) => x == st.key);
                        if (stationFound != null)
                        {
                            continue;
                        }
                        else if (stationFound == null && nextStationFound == false)
                        {
                            stationAnimated.Play("selectedStation");
                            //StartCoroutine(AnimateSelectedStation(stationAnimated));
                            TapPinScript.enabled = true;
                            color.a = 1f;
                            temp.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            temp.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = color;
                            //Once it has found the station it will not search for it again
                            nextStationFound = true;
                        }
                    }
                    //else go for the first station
                    else
                    {
                        //Get the first station to start
                        if (stationList[0].key == st.key)
                        {
                            PlayerPrefs.SetInt("Total Score", 0);
                            stationAnimated.Play("selectedStation");
                            //StartCoroutine(AnimateSelectedStation(stationAnimated));
                            temp.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            TapPinScript.enabled = true;
                            color.a = 1f;
                            temp.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = color;
                        }
                    }
                    //temp.transform.SetParent(Pinx3.transform, false);

                    //Choose where the pins must be shown (Campus,  UAO Labs1 or UAO Labs2)
                    if (st.place == "0")
                    {
                        temp.transform.parent = Pinx3.transform.GetChild(0);
                    }
                    if (st.place == "1")
                    {
                        temp.transform.parent = Pinx3.transform.GetChild(1);
                        temp.transform.GetChild(0).GetChild(1).localScale = new Vector3(3, 3, 3);
                        //temp.transform.localScale = new Vector3(30, 30, 30);
                    }
                    if (st.place == "2")
                    {
                        temp.transform.parent = Pinx3.transform.GetChild(2);
                        temp.transform.GetChild(0).GetChild(1).localScale = new Vector3(3, 3, 3);
                        //temp.transform.localScale = new Vector3(30, 30, 30);
                    }
                }
            }
            //Disable free exploration mode
            getStations = false;
        }
        #endregion
        #region Mode: Free exploration
        else
        {
            if (getAreas == true)
            {
                foreach (Area ar in areaList)
                {
                    var temp = Instantiate(AreaPrefab, new Vector3(ar.x - 32, ar.y, ar.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    temp.name = ar.name;
                    var areaTextName = temp.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                    areaTextName.text = ar.name;
                    temp.transform.parent = Pinx1.transform;
                }
                getAreas = false;
            }

            if (getStations == true)
            {
                foreach (Station st in stationList)
                {
                    var temp = Instantiate(StationPrefab, new Vector3(st.x - 32, st.y, st.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    temp.name = st.name;
                    var stationTextName = temp.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                    stationTextName.text = st.name;
                    temp.transform.parent = Pinx2.transform.GetChild(0);

                    //Choose where the pins must be shown (Campus,  UAO Labs1 or UAO Labs2)
                    if (st.place == "0")
                    {
                        temp.transform.parent = Pinx2.transform.GetChild(0);
                    }
                    if (st.place == "1")
                    {
                        temp.transform.parent = Pinx2.transform.GetChild(1);
                        temp.transform.GetChild(0).GetChild(1).localScale = new Vector3(3, 3, 3);
                        //temp.transform.localScale = new Vector3(30, 30, 30);
                    }
                    if (st.place == "2")
                    {
                        temp.transform.parent = Pinx2.transform.GetChild(2);
                        temp.transform.GetChild(0).GetChild(1).localScale = new Vector3(3, 3, 3);
                        //temp.transform.localScale = new Vector3(30, 30, 30);
                    }
                }
                getStations = false;
            }
        }
        #endregion
    }
    private IEnumerator SetFinalScore()
    {

        string currentGroup = PlayerPrefs.GetString("Group");
        int finalScore = PlayerPrefs.GetInt("Total Score");
        yield return null;
        yield return new YieldTask(reference.GetValueAsync().ContinueWith(task =>
                        {
                            reference.Child("Groups").Child(yincanaName).Child("groups").Child(currentGroup).Child("final score").SetValueAsync(finalScore);
                        }));

    }

    #region Order list
    //Re orders list to match the sequence
    private IEnumerator ReOrderList()
    {
        sequence = PlayerPrefs.GetInt("Sequence");
        if (sequence > 0)
        {
            for (int i = 0; i < stationList.Count; i++)
            {
                //Debug.Log("stationlist elements Before");
                //Debug.Log(stationList[i].name);
            }
            for (int i = 0; i < sequence; i++)
            {
                //saves first station
                var item = stationList[0];
                //removes station in first position
                stationList.RemoveAt(0);
                stationList.Add(item);
            }
            for (int i = 0; i < stationList.Count; i++)
            {
                //Debug.Log("stationlist elements After");
                //Debug.Log(stationList[i].name);
            }
        }
        yield return StartCoroutine(GetStationsFinishedList());
    }

    //Get a list of stations with scores (finished)
    public IEnumerator GetStationsFinishedList()
    {
        string currentGroup = PlayerPrefs.GetString("Group");
        yield return new YieldTask(reference.Child("Groups").Child(yincanaName).Child("groups").Child(currentGroup).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Child("stations score").Exists)
                {
                    stationsFinishedList.Clear();
                    //if it doesn't exist a list stations with scores
                    if (snapshot.Exists == false)
                    {
                        stationsFinishedExist = false;
                    }
                    //else if it does exist a list of station with scores
                    else
                    {
                        stationsFinishedExist = true;
                        foreach (DataSnapshot s in snapshot.Child("stations score").Children)
                        {
                            stationsFinishedList.Add(s.Key.ToString());
                        }
                    }
                }

            }
        }));
    }
    #endregion

    public void CheckAccessToYincana()
    {
        //There was an existing yincana
        if (yincanaName != "")
        {
            StartCoroutine(TryAccessToYincana());
        }
    }


    //Acesss to the Yincana, check if there was an existing yincana to set it up
    private IEnumerator TryAccessToYincana()
    {
        yield return new YieldTask(reference.Child("Yincanas").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsCompleted)
            {
                Snapshot = task2.Result;
                foreach (DataSnapshot info in Snapshot.Children)
                {
                    IDictionary yincanas = (IDictionary)(info.Value);
                    if (yincanas["yincanaName"].ToString() == yincanaName)
                    {
                        storedYincanaKey = info.Key;
                        yfound = true;
                        break;
                    }
                }
            }
        }));

        //if yincanawas found
        if (yfound)
        {
            //Clean the station
            stationList.Clear();
            //Clean the route
            PlayerPrefs.SetString("Recorrido", null);
            //Activate the group register canvas
            GroupCanvas.SetActive(true);
            //yield return StartCoroutine(SetYincanaCoroutine());
        }
        //Show message that yincana doesn't exist
        else
        {
            //Call script to execute a function to show the yincana not found canvas
            ShowYinfanaNotFound.YincanaNotFound_open();
            yield return null;
        }
    }

    public void SignOut()
    {
        participantsNameList.Clear();
        areaList.Clear();
        stationList.Clear();
        participantsNameList.Clear();
        isOnYincana = false;
        Timer.temp = false;

        //if there was a yincana, disable it 
        StartCoroutine(DisableYincana());
    }

    #region Input Fields
    public void setYincanaName(string setYincanaName)
    {
        yincanaName = setYincanaName;
    }
    public void setGroupName(string getGroupName)
    {
        groupName = getGroupName;
    }
    public void setName(string getName)
    {
        nameParticipant = getName;
    }
    public void setEmail(string getEmail)
    {
        email = getEmail;
    }
    public void setAge(string getAge)
    {
        age = getAge;
    }
    public void setPhone(string getPhone)
    {
        phone = getPhone;
    }
    #endregion

    //Register the name of the group
    public void RegisterGroupName()
    {
        PlayerPrefs.SetString("Group Name", groupName);
        groupNameBox.GetComponent<TextMeshProUGUI>().text = groupName;
        groupNameCanvas.SetActive(false);
        groupRegisterCanvas.SetActive(true);
    }

    //Add a participant to the group list
    public void AddParticipant()
    {
        Debug.Log(pFieldPhone);
        if (politicsToggle.GetComponent<Toggle>().isOn && pFieldName != null && pFieldEmail != null
        && pFieldAge != null && pFieldPhone != null)
        {
            participantsList.Add(new Participants(nameParticipant, email, age, phone));

            pFieldName.GetComponent<TMP_InputField>().text = "";
            pFieldEmail.GetComponent<TMP_InputField>().text = "";
            pFieldAge.GetComponent<TMP_InputField>().text = "";
            pFieldPhone.GetComponent<TMP_InputField>().text = "";
            nameParticipant = null;
            email = null;
            age = null;
            phone = null;

            ParticipantsCount.GetComponent<TextMeshProUGUI>().text = participantsList.Count.ToString();
        }
        if (politicsToggle.GetComponent<Toggle>().isOn == false)
            StartCoroutine(AcceptPolitics());

        else if (pFieldName == null || pFieldEmail == null || pFieldAge == null || pFieldPhone == null)
            StartCoroutine(BlankSpacesInput());
    }
    private IEnumerator BlankSpacesInput()
    {
        Toast.SetActive(true);
        Toast.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No pueden quedar espacios en blanco";
        yield return new WaitForSeconds(2f);
        Toast.SetActive(false);
    }

    private IEnumerator AcceptPolitics()
    {
        Toast.SetActive(true);
        Toast.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Debes aceptar las políticas de privacidad";
        yield return new WaitForSeconds(2f);
        Toast.SetActive(false);
        OpenWebsite();
    }

    //Hide politics canvas 
    public void hidePoliticsCanvas()
    {
        DataPoliticsCanvas.SetActive(false);
    }
    //Show politics canvas 
    public void showPoliticsCanvas()
    {
        DataPoliticsCanvas.SetActive(true);
    }

    //Activated by the end of the group registration
    public void StartYincana()
    {
        PlayerPrefs.SetString("Yincana", yincanaName);
        StartCoroutine(SaveGroup());
    }
    //Reasures that they have finished adding participants
    public void ConfirmYincanaStart()
    {
        YincanaConfirmationModal.SetActive(true);
    }
    public void HideConfirmationModal()
    {
        YincanaConfirmationModal.SetActive(false);
    }

    //Save the group in Firebase and start the Yincana
    private IEnumerator SaveGroup()
    {
        if (politicsToggle.GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("Total Score", 0);
            if (nameParticipant != null && email != null && age != null && phone != null)
                participantsList.Add(new Participants(nameParticipant, email, age, phone));

            yincanaStart = true;

            yield return new YieldTask(reference.Child("Groups").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot SnapShot = task.Result;
                    long itemNumber = SnapShot.Child(yincanaName).Child("groups").ChildrenCount;
                    sequence = (int)itemNumber;
                    group = "group " + itemNumber + "/";
                    yincanaState = true;
                    reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("state").SetValueAsync(yincanaState);
                    reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("sequence").SetValueAsync(itemNumber);
                    reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("name").SetValueAsync(groupName);
                    reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("date").SetValueAsync(System.DateTime.UtcNow.ToString("dd,MM,yyyy"));
                    foreach (Participants p in participantsList)
                    {
                        string participant = "participant " + participantsList.IndexOf(p) + "/";
                        reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("participants").Child(participant).Child("name").SetValueAsync(p.name);
                        reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("participants").Child(participant).Child("email").SetValueAsync(p.email);
                        reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("participants").Child(participant).Child("age").SetValueAsync(p.age);
                        reference.Child("Groups").Child(yincanaName).Child("groups").Child(group).Child("participants").Child(participant).Child("phone").SetValueAsync(p.phone);
                    }
                }
            }));

            //Uploads in the students node
            yield return new YieldTask(reference.Child("Students").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot SnapShot = task.Result;
                    long itemNumber = SnapShot.ChildrenCount;
                    foreach (Participants p in participantsList)
                    {
                        int student = (int)itemNumber + participantsList.IndexOf(p);
                        string students = "student " + student + "/";
                        reference.Child("Students").Child(students).Child("name").SetValueAsync(p.name);
                        reference.Child("Students").Child(students).Child("email").SetValueAsync(p.email);
                        reference.Child("Students").Child(students).Child("age").SetValueAsync(p.age);
                        reference.Child("Students").Child(students).Child("phone").SetValueAsync(p.phone);
                    }
                }
            }));

            //Start to set the yincana
            yield return StartCoroutine(SetYincanaCoroutine());
        }
        else

            OpenWebsite();
        //DataPoliticsCanvas.SetActive(true);
    }

    public void OpenWebsite()
    {
        string url = "https://www.uao.edu.co/noticias/la-uao-actualiza-su-politica-de-tratamiento-y-proteccion-de-datos";
        Application.OpenURL(url);
    }


    //Set state to false and disable the yincana
    private IEnumerator DisableYincana()
    {
        yincanaState = false;
        string currentGroup = PlayerPrefs.GetString("Group");
        yield return new YieldTask(reference.Child("Yincanas").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsCompleted)
            {
                Snapshot = task2.Result;
                foreach (DataSnapshot info in Snapshot.Children)
                {
                    IDictionary yincanas = (IDictionary)(info.Value);
                    if (yincanas["yincanaName"].ToString() == yincanaName)
                    {
                        storedYincanaKey = info.Key;
                        yfound = true;
                        break;
                    }
                }
            }
        }));
        if (yfound)
        {
            yield return new YieldTask(reference.GetValueAsync().ContinueWith(task =>
                        {
                            reference.Child("Groups").Child(yincanaName).Child("groups").Child(currentGroup).Child("state").SetValueAsync(yincanaState);
                        }));
            yield return null;
            yfound = false;
            getStationsYincana = false;
        }
        yield return null;
        PlayerPrefs.SetString("Yincana", null);
        PlayerPrefs.SetString("Recorrido", null);
        PlayerPrefs.SetString("Group Name", null);
        PlayerPrefs.SetString("Group", null);
        PlayerPrefs.SetString("Yincana Welcome", null);
        PlayerPrefs.SetInt("Total Score", 0);
        PlayerPrefs.SetInt("localscore", 0);
        //Destroy Chat Message Object
        try
        {
            GameObject.Destroy(GameObject.Find("Chat Message").gameObject);
        }
        catch
        {
        }
        //Refresh the main scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
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

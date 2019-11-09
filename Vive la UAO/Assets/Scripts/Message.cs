using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;

using UnityEngine.Assertions;

public class Message : MonoBehaviour
{
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private DataSnapshot Snapshot;
    public GameObject MessagePrefab;
    private static bool GameManagerExists;

    private bool workexecuted = false;
    bool workDone = false;
    string message;
    // Start is called before the first frame update
    void Start()
    {

        if (!GameManagerExists) //if GameManagerexcistst is not true --> this action will happen.
        {
            GameManagerExists = true;
            DontDestroyOnLoad(this.gameObject);
            //this.gameObject.name = ("Timer Canvas Instance");
        }
        else
        {
            Destroy(gameObject);
        }

        string currentYincana = PlayerPrefs.GetString("Yincana");
        string currentGroup = PlayerPrefs.GetString("Group");
        Debug.Log(currentGroup);
        Debug.Log(currentYincana);
        FirebaseDatabase.DefaultInstance.GetReference("Groups").Child(currentYincana).Child("groups").Child(currentGroup).Child("message")
        .ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        message = args.Snapshot.Value.ToString();
        StartCoroutine(BlankSpacesInput());

        workDone = true;
        // Do something with the data in args.Snapshot
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator BlankSpacesInput()
    {
        var temp = Instantiate(MessagePrefab);
        //MessagePrefab.SetActive(true);
        temp.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(temp.gameObject);
        //MessagePrefab.SetActive(false);
    }

}
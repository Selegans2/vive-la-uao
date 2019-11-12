using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHandler : MonoBehaviour
{

    private GameObject getQAObject;
    private ToggleGroup toggleGroup;
    private Toggle newToggle;
    GetQA getQADataScript;
    //TestQuiz testQuizScript;
    // Start is called before the first frame update
    void Start()
    {
        getQAObject = GameObject.Find("Get QA Script");
        getQADataScript = getQAObject.GetComponent<GetQA>();
        //testQuizScript = getQAObject.GetComponent<TestQuiz>();

        toggleGroup = GameObject.FindWithTag("toggleGroup").GetComponent<ToggleGroup>();

        newToggle = this.GetComponent<Toggle>();
        newToggle.group = toggleGroup;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ToggleChanged(bool newValue)
    {
        Debug.Log(newValue);
    }

    public void ToggleChanged2()
    {
        string sendedAnswer = transform.parent.GetComponent<Text>().text;
        getQADataScript.ToggleHandler(sendedAnswer);
        //Debug.Log(sendedAnswer);
        //testQuizScript.ToggleHandler(sendedAnswer);
    }

}

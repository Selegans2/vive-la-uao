using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousScene : MonoBehaviour
{
    static PreviousScene _instance;

    public static PreviousScene instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PreviousScene>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Scene");
        /* 
        if (PlayerPrefs.GetString("Yincana").Length != 0)
       {
            GetPins.getStationsYincana=true;
       }
       */
    }
    public void VRScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("VRScene");
    }
    public void ARScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ARScene");
    }

    public void ARSceneStation()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ARScene Station");
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wellcomeApp : MonoBehaviour
{
    public Animator Animator;
    public GameObject welcomeApp;


    public void closeWelcomeApp()
    {
        Animator.Play("welcomeAppClose");
        //welcomeApp.SetActive(false);
    }
}

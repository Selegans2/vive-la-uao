using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class InterfaceControllerYincana : MonoBehaviour
{

    public Animator Animator;
    bool cond = true;
    public GameObject InfoMore;
    public GameObject infoPanel;
    public GameObject GroupName;

    void Start(){
        GroupName.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Group Name");
    }
 public void FloorMenu()
    {    
        if (cond)
        {
             Animator.Play("FloorMenu1");
            cond = false;
        }
        else
        {
           Animator.Play("FloorMenu2");
            cond = true;
        }
    }

    public void ARmap()
    {
        //Add scene to see the AR map
    }
    public void PuntosUbicate()
    {
        //Add scene to see Puntos ubicate
    }


    public void YincanaInfoGroup_open()
    {
        infoPanel.SetActive(true);
        Animator.Play("YincanaInfoAnimation");
    }

    public void YincanaInfoGroup_close()
    {
        infoPanel.SetActive(false);
        Animator.Play("ScaleArrowButton");
    }

    public void OpenInfoMore()
    {
        InfoMore.SetActive(true);
    }


    public GameObject welcome_1;
    public GameObject welcome_2;
    public GameObject WelcomeDialog;
    private int cont = 1;

    public void Next()
    {
        switch (cont)
        {
            case 1:
                welcome_1.SetActive(false);
                welcome_2.SetActive(true);
                break;
            case 2:
                WelcomeDialog.SetActive(false);
                cont = 1;
                break;
        }
        cont++;
    }


    private bool showCond = false;
    public void showArContent()
    {
        if (!showCond)
        {
            Animator.Play("showArContent");
            showCond = true;
        }
        else
        {
            Animator.Play("closeArContent");
            showCond = false;
        }

    }

}

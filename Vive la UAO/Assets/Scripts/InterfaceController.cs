using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

public class InterfaceController : MonoBehaviour
{
    public Animator Animator;
    public GameObject Interface01;
    public GameObject Modal;
    public GameObject InterfaceMore;

    public GameObject modal_Options;
    public GameObject modal_Login_y;
    public GameObject modal_Login_r;
    public GameObject YincanaNotFound;

    bool cond = true;

    //private GameObject infoPanel;

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

    /*
    public void YincanaInfoGroup_open()
    {
        infoPanel = GameObject.Find("Canvas Yincana/interface-01-y/Info-panel-y");
        infoPanel.SetActive(true);
        Animator.Play("YincanaInfoAnimation");
    }

    public void YincanaInfoGroup_close()
    {
        infoPanel = GameObject.Find("Canvas Yincana/interface-01-y/Info-panel-y");
        infoPanel.SetActive(false);
        Animator.Play("ScaleArrowButton");
    }
    */

    public void OpenModalOptions()
    {
        Modal.SetActive(true);
        Animator.Play("ModalOption");
    }

 

    public void BackOptions()
    {
        modal_Options.SetActive(true);
        modal_Login_r.SetActive(false);
        modal_Login_y.SetActive(false);
    }

    public void CloseModalOptions()
    {
        Modal.SetActive(false);
        modal_Options.SetActive(true);
        modal_Login_r.SetActive(false);
        modal_Login_y.SetActive(false);
        Animator.Play("ScaleArrowButton");

    }

    public void OpenInterfaceMore()
    {
        Interface01.SetActive(false);
        InterfaceMore.SetActive(true);
    }
    public static string teamo;
    public void YincanaNotFound_open()
    {
        YincanaNotFound.SetActive(true);
        Animator.Play("NotFoundAnimation");
        PlayerPrefs.SetString("Yincana",null);
    }
    public void YincanaNotFound_close()
    {
        YincanaNotFound.SetActive(false);
    }


    public GameObject modalStation;
    public GameObject modalArea;


    public void modalStations()
    {
        modalStation.SetActive(true);
        Animator.Play("modalStations");
    }

    public void modalAreas()
    {
        modalArea.SetActive(true);
        Animator.Play("modalAreas");
    }


    public void closeModals()
    {
        modalStation.SetActive(false);
        modalArea.SetActive(false);
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

    public void GoToStation()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("VRScene");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLoginOptions : MonoBehaviour
{

    public GameObject selectMode;
    public GameObject login_y;
    public GameObject login_r;

    

    public void selectionMode(bool mode)
    {
        if (mode)
        {
            selectMode.SetActive(false);
            login_y.SetActive(true);
        }
        else
        {
            selectMode.SetActive(false);
            login_r.SetActive(true);
        }

    }

}

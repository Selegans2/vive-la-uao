using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBackGroupCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public GameObject GroupName;
    public GameObject GroupRegister;

    public void GoBack()
    {
        GroupName.SetActive(true);
        GroupRegister.SetActive(false);
    }
}

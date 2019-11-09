using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceControllerMore : MonoBehaviour
{


    public GameObject info_contacto;
    public GameObject info_ayuda;
    public Animator Animator;
    public GameObject interface_more;
    public GameObject interface_01;

    public void SwitchMore()
    {
         Animator.Play("line");
         info_contacto.SetActive(false);
         info_ayuda.SetActive(true);
    }

    public void SwitchMore_2()
    {
        Animator.Play("line-2");
        info_contacto.SetActive(true);
        info_ayuda.SetActive(false);
    }

    public void closeInterfaceMore()
    {
        interface_more.SetActive(false);
        interface_01.SetActive(true);
    }


}

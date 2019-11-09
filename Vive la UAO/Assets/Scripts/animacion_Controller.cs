using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class animacion_Controller : MonoBehaviour
{

    public Animator anim;
    public Animator animLine;
    public GameObject GroupCanvas;
    bool cond = true;
    bool cond2 = true;
    public Text groupNameText;

    public Text scoreText;

    void Start()
    {
        anim = GetComponent<Animator>();
        //var textGroupName = groupNameText.GetComponent<Text>();
        //textGroupName.text = PlayerPrefs.GetString("Group Name");

        //var textScoreText = scoreText.GetComponent<Text>();
        //textScoreText.text = PlayerPrefs.GetInt("localscore").ToString();
    }

    public void animacionMetodo()
    {
        if (cond)
        {
            anim.Play("animacion-01");
            cond = false;
        }
        else
        {
            anim.Play("animacion-02");
            cond = true;
        }
    }

    public void YincanaInformation()
    {
        if (cond2)
        {
            GroupCanvas.SetActive(true);
            anim.Play("yincana-info");
            cond2 = false;
            cond = true;    
        }
                else
                {
                    anim.Play("nothing");
                    GroupCanvas.SetActive(false);
                    cond2 = true;
                }
    }

    public void animacionMetodo2()
    {
        if (cond)
        {
            anim.Play("station-info");
            cond = false;
        }
        else
        {
            anim.Play("station-info-back");
            cond = true;
        }
    }

    public void animationLine01()
    {
        animLine.Play("animationLine-01");
    }

    public void animationLine02()
    {
        animLine.Play("animationLine-02");
    }
}



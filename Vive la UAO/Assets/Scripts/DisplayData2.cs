using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Video;

public class DisplayData2 : MonoBehaviour
{
    MeshRenderer m_Renderer;
    public GameObject Video;
    private VideoPlayer videoPlayer;
    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Renderer.enabled = false;
        videoPlayer = Video.GetComponent<VideoPlayer>();
        

        //store station key tapped to be used latter in the challanges
        var title = GameObject.FindWithTag("title").GetComponent<Text>();
        title.text = TapPin.StationTapped.name;

        var description = GameObject.FindWithTag("description").GetComponent<Text>();
        description.text = TapPin.StationTapped.description;

        var image = GameObject.FindWithTag("image").GetComponent<Text>();

        m_Renderer.enabled = false;
        m_Renderer.material.SetTexture("_MainTex", GetStationData.texturePublic);

        videoPlayer.url = TapPin.StationTapped.videoUrl;
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void ActivateImage()
    {
        if (m_Renderer.enabled == false)
        {
            m_Renderer.enabled = true;
        }
        else
        {
            m_Renderer.enabled = false;
        }
    }
}
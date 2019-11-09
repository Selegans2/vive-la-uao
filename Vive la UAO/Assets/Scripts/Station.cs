using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Station
{
    public string key { get; set; }
    public string name { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public string description { get; set; }
    public string imageUrl { get; set; }
    public string videoUrl { get; set; }
    public string place { get; set; }

    public Station()
    {
    }

    public Station(string key, string name, float x, float y, float z, string description, string imageUrl, string videoUrl, string place)
    {
        this.key = key;
        this.name = name;
        this.x = x;
        this.y = y;
        this.z = z;
        this.description = description;
        this.imageUrl = imageUrl;
        this.videoUrl = videoUrl;
        this.place = place;
    }
}


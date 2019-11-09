using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    public string name { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public string imageUrl { get; set; }
    public string description { get; set; }

    public Area()
    {
    }
    
    public Area(string name, float x, float y, float z, string description, string imageUrl)
    {
        this.name = name;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

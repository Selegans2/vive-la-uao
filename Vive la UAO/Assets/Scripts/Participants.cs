using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Participants
{
    
    public string name { get; set; }
    public string email { get; set; }
    public string age { get; set; }
    public string phone { get; set; }    
    
    public Participants()
    {
    }

    public Participants(string name, string  email, string age, string phone)
    {
        this.name = name;
        this.email = email;
        this.age = age;
        this.phone = phone;
    }
}

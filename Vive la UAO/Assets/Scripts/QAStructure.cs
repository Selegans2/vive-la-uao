using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAStructure
{
    
    public string question { get; set; }
    public string correct { get; set; }
    public List<string> answersList; 
    
    


    public QAStructure()
    {
        
    }

    public QAStructure(string question, string correct, List<string> answersList)
    {
        this.question = question;
        this.correct = correct;
        this.answersList = answersList;        
    }
}

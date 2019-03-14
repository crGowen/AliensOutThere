using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technosignature
{
    public int sender;
    public float timeToReceive;
    //timeToReceive in YEARS

    public Technosignature(int sen, float tim)
    {
        sender = sen;
        timeToReceive = tim;
        //Debug.Log("It will arrive at +" + tim.ToString());
    }
}

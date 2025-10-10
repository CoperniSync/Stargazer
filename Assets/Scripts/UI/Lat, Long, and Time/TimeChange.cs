using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeChange : MonoBehaviour
{

    public GameObject HourBox;
    public GameObject MinuteBox;
    public GameObject SuffixBox;

    //holds the dropdown components of the input boxes
    private Dropdown Hours;
    private Dropdown Minute;
    private Dropdown Suffix;

    //get user input sigleton
    private InputContainer inputs = InputContainer.Container;

    private void Awake()
    {
        Hours = HourBox.GetComponent<Dropdown>();
        Minute = MinuteBox.GetComponent<Dropdown>();
        Suffix = SuffixBox.GetComponent<Dropdown>();
    }
    public void UpdateTime()
    {
        DateTime newTime = inputs.Time.Date;
        if(Suffix.value == 1)
        {
            newTime = newTime.AddHours(12.0);
        }
        if (Hours.value != 11)
        {
            newTime = newTime.AddHours(Hours.value + 1.0);
        }
        newTime = newTime.AddMinutes(Minute.value);
        inputs.Time = newTime;
        
    }

}
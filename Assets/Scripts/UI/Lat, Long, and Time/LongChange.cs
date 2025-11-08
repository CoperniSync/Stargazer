using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


// Author: Morgan Hendon FA 2025
public class LongChange : MonoBehaviour
{
    //get user input sigleton
    private InputContainer inputs = InputContainer.Container;
    public void updateLongitudeDeg()
    {
        // check if string is empty
        if (this.gameObject.transform.GetComponent<InputField>().text == "")
        {
            return;
        }

        int newLong = Int32.Parse(this.gameObject.transform.GetComponent<InputField>().text);
        if (newLong > 180)
        {
            newLong = 180;
        }
        else if (newLong < -180)
        {
            newLong = -180;
        }
        this.gameObject.transform.GetComponent<InputField>().text = newLong.ToString();
        inputs.LongitudeDeg = newLong;
        Debug.Log(inputs.LongitudeDeg);
    }

    public void updateLongitudeMin()
    {
        // check if string is empty
        if (this.gameObject.transform.GetComponent<InputField>().text == "")
        {
            return;
        }

        int newLong = Int32.Parse(this.gameObject.transform.GetComponent<InputField>().text);
        if (newLong > 60)
        {
            newLong = 60;
        }
        else if (newLong < 0)
        {
            newLong = 0;
        }
        this.gameObject.transform.GetComponent<InputField>().text = newLong.ToString();
        inputs.LongitudeMin = newLong;
        Debug.Log(inputs.LongitudeMin);
    }
}

using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Author: Morgan Hendon FA 2025

public class LatChange : MonoBehaviour
{

    //get user input sigleton
    private InputContainer inputs = InputContainer.Container;
    
    public void updateLatitudeDeg()
    {
        // check if string is empty
        if (this.gameObject.transform.GetComponent<InputField>().text == "")
        {
            return;
        }

        int newLat = Int32.Parse(this.gameObject.transform.GetComponent<InputField>().text);
        if(newLat>90)
        {
            newLat = 90;
        }
        else if(newLat<-90)
        {
            newLat = -90;
        }
        this.gameObject.transform.GetComponent<InputField>().text = newLat.ToString();
        inputs.LatitudeDeg = newLat;
        Debug.Log(inputs.LatitudeDeg);
    }

    public void updateLatitudeMin()
    {
        // check if string is empty
        if (this.gameObject.transform.GetComponent<InputField>().text == "")
        {
            return;
        }

        int newLat = Int32.Parse(this.gameObject.transform.GetComponent<InputField>().text);
        if (newLat > 60)
        {
            newLat = 60;
        }
        else if (newLat < 0)
        {
            newLat = 0;
        }
        this.gameObject.transform.GetComponent<InputField>().text = newLat.ToString();
        inputs.LatitudeMin = newLat;
        Debug.Log(inputs.LatitudeMin);
    }
}

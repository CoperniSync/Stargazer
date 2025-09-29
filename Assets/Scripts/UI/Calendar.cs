using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;


public class Calendar : MonoBehaviour
{

    // get the singleton for the Container and set internal values
    InputContainer inputs;
    private GameObject[,] buttons = new GameObject[7, 6];
    static private string[] monthList = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        inputs = InputContainer.Container;
    }
    void Start()
    {
       

        //load array with days
        for (int column = 0; column < 7; column++)
        {
            for (int row = 0; row < 6; row++)
            {
                buttons[column, row] = gameObject.transform.GetChild(1).transform.GetChild(column).transform.GetChild(row + 1).gameObject;
                buttons[column, row].AddComponent<Text>();


            }
        }
  

        UpdateButtons();

    }

    //calculate and display what number each button should have
    public void UpdateButtons()
    {
        //show current year
        gameObject.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().text = monthList[inputs.Time.Month - 1] + " " + inputs.Time.Year.ToString();

        int yearpoint = (inputs.Time.Year % 100 + ((inputs.Time.Year % 100) / 4)) % 7;
        int monthpoint = 0;
        int centpoint = 0;
        int leappoint = 0;
        int offset = 0;
        switch (inputs.Time.Month)
        {
            case 1:
            case 10:
                monthpoint = 0;
                break;
            case 5:
                monthpoint = 1;
                break;
            case 8:
                monthpoint = 2;
                break;
            case 2:
            case 3:
            case 11:
                monthpoint = 3;
                break;
            case 6:
                monthpoint = 4;
                break;
            case 9:
            case 12:
                monthpoint = 5;
                break;
            case 4:
            case 7:
                monthpoint = 6;
                break;
        }
        switch (inputs.Time.Year % 400 / 100)
        {
            case 0:
                centpoint = 6;
                break;
            case 1:
                centpoint = 4;
                break;
            case 2:
                centpoint = 2;
                break;
            case 3:
                centpoint = 0;
                break;

        }
        if (DateTime.IsLeapYear(inputs.Time.Year))
        {
            leappoint = 1;
        }
        else
        {
            leappoint = 0;
        }
        offset = (yearpoint + monthpoint + centpoint + 1 - leappoint) % 7;
        int counter = 0;
        for (int row = 0; row < 6; row++)
        {
            for (int column = 0; column < 7; column++)
            {
                if (counter < offset)
                {
                    int prevmonth = inputs.Time.Month - 1;
                    if (prevmonth < 1)
                        prevmonth = 12;
                    buttons[column, row].transform.GetChild(0).gameObject.GetComponent<Text>().text = (DateTime.DaysInMonth(inputs.Time.Year, prevmonth) - (offset - counter) + 1).ToString();
                }
                else if (counter == offset)
                {
                    buttons[column, row].transform.GetChild(0).gameObject.GetComponent<Text>().text = "1";
                    counter = 1;
                    offset = -1;
                }
                else if (counter <= DateTime.DaysInMonth(inputs.Time.Year, inputs.Time.Month))
                {
                    buttons[column, row].transform.GetChild(0).gameObject.GetComponent<Text>().text = counter.ToString();
                }
                else
                {
                    buttons[column, row].transform.GetChild(0).gameObject.GetComponent<Text>().text = (counter-DateTime.DaysInMonth(inputs.Time.Year, inputs.Time.Month)).ToString();
                }
                    counter++;
            }
        }
    }
    public void YearUp()
    {
        inputs.Time = inputs.Time.AddYears(1);
        UpdateButtons();
    }
    public void YearDown()
    {
        inputs.Time = inputs.Time.AddYears(-1);
        UpdateButtons();
    }

    public void MonthUp()
    {
        inputs.Time = inputs.Time.AddMonths(1);
        UpdateButtons();
    }
    public void MonthDown()
    {
        inputs.Time = inputs.Time.AddMonths(-1);
        UpdateButtons();
    }
}

using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;

// Script for the Calendar Widget
// Author: Morgan Hendon
public class Calendar : MonoBehaviour
{

    
    private InputContainer inputs = InputContainer.Container; //singleton for input container
    private DateTime time = DateTime.Now;                     // holds time for calender display
    private CalendarButtons[,] buttons = new CalendarButtons[7, 6];  
    static private string[] monthList = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    private struct CalendarButtons      // struct for holding information for each of the day buttons on the widget
    {
       public GameObject button;
       public int year, month, day;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

        //load array with buttons
        for (int column = 0; column < 7; column++)
        {
            for (int row = 0; row < 6; row++)
            {
                buttons[column, row].button = gameObject.transform.GetChild(1).transform.GetChild(column).transform.GetChild(row + 1).gameObject;
            }
        }
  
        //load buttons with dates
        UpdateButtons();

    }

    //method for calculating and displaying what number each button should have
    private void UpdateButtons()
    {
        //show current month and current year
        gameObject.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().text = monthList[time.Month - 1] + " " + time.Year.ToString();

        int yearpoint = (time.Year % 100 + ((time.Year % 100) / 4)) % 7;  // determine the year factor for the weekday offset
        int monthpoint = 0;
        int centpoint = 0;
        int leappoint = 0;
        int offset = 0;

        // determine the month factor for the weekday offset
        switch (time.Month)
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

        // determine the century factor for the weekday offset
        switch (time.Year % 400 / 100)
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
        
        //determine if it is a leap year for weekday offset
        if (DateTime.IsLeapYear(time.Year))
        {
            leappoint = 1;
        }
        else
        {
            leappoint = 0;
        }

        //calcuate weekday offset
        offset = (yearpoint + monthpoint + centpoint + 1 - leappoint) % 7;
        int counter = 0;

        for (int row = 0; row < 6; row++)
        {
            for (int column = 0; column < 7; column++)
            {
                if (counter < offset)   // if the date is the last month
                {
                    int prevmonth = time.Month - 1;
                    if (prevmonth < 1)
                    { 
                        prevmonth = 12;
                        buttons[column, row].year = time.Year-1;
                    }
                    else
                    {
                        buttons[column, row].year = time.Year;
                    }

                        buttons[column, row].day = DateTime.DaysInMonth(time.Year, prevmonth) - (offset - counter) + 1;
                    buttons[column, row].month = prevmonth;
                    
                    buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = buttons[column, row].day.ToString();
                    
                }
                else if (counter == offset) // if the date is the first day of the month
                {
                    buttons[column, row].day = 1;
                    buttons[column, row].month = time.Month;
                    buttons[column, row].year = time.Year;
                    buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "1";
                    counter = 1;
                    offset = -1;
                }
                else if (counter <= DateTime.DaysInMonth(time.Year, time.Month)) // if the date is in the month
                {
                    buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = counter.ToString();
                    buttons[column, row].day = counter;
                    buttons[column, row].month = time.Month;
                    buttons[column, row].year = time.Year;
                }
                else // if the date is in the next month
                {
                    buttons[column, row].day = counter - DateTime.DaysInMonth(time.Year, time.Month);
                    if (time.Month + 1 > 12) 
                    {
                        buttons[column, row].month = 1;
                        buttons[column, row].year = time.Year + 1;
                        
                    }
                    else  
                    {                        
                        buttons[column, row].month = time.Month + 1;
                        buttons[column, row].year = time.Year;
                    }
                    buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = buttons[column, row].day.ToString();
                }
                    counter++;
            }
        }
    }

    //method for incrementing the year on the calendar display
    public void YearUp()
    {
        time = time.AddYears(1);
        UpdateButtons();
    }

    //method for decrementing the year on the calendar display
    public void YearDown()
    {
        time = time.AddYears(-1);
        UpdateButtons();
    }

    //method for incrementing the month on the calendar display
    public void MonthUp()
    {
        time = time.AddMonths(1);
        UpdateButtons();
    }

    //method for decrementing the month on the calendar display
    public void MonthDown()
    {
        time = time.AddMonths(-1);
        UpdateButtons();
    }

    //method for handling button input
    public void UpdateDate(GameObject buttonPressed)
    {
        for (int row = 0; row < 6; row++)
        {
            for (int column = 0; column < 7; column++)
            {
                if (buttons[column, row].button == buttonPressed)
                {
                    inputs.time = new(buttons[column, row].year, buttons[column, row].month, buttons[column, row].day, inputs.Time.Hour, inputs.Time.Minute, inputs.Time.Second, inputs.Time.Millisecond, inputs.Time.Kind);
                    Debug.Log(inputs.Time.ToString() +"\n" +DateTime.Now.ToString());
                    return;
                }
            }
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Assets.Scripts.UI
{

    // Script for the Calendar Widget
    // Author: Morgan Hendon
    public class Calendar : MonoBehaviour
    {
        /// <summary>
        /// class for controlling the calender widget
        /// </summary>


        private InputContainer inputs = InputContainer.Container; //singleton for input container
        private DateTime time = DateTime.Now;                     // holds time for calender display
        private int year = DateTime.Now.Year;
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
            /// <summary>
            /// method for updating the display for the calender widget
            /// </summary>
            /// 
            //show current month and current year
            gameObject.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().text = monthList[time.Month - 1] + " " + year.ToString();

            // variables for calculating day of week offset
            int yearpoint = (year % 100 + year % 100 / 4) % 7;  // determine the year factor for the weekday offset
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
            switch (year % 400 / 100)
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
            if (IsLeapYear(year))
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


            // populate the day buttons' text
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (counter < offset)   // if the date is the last month
                    {
                        int prevmonth = time.Month - 1;
                        if (prevmonth < 1)  // wrap around to December
                        {
                            prevmonth = 12;
                            buttons[column, row].year = year - 1;
                        }
                        else
                        {
                            buttons[column, row].year = year;
                        }

                        buttons[column, row].day = DaysInMonth(year, prevmonth) - (offset - counter) + 1;
                        buttons[column, row].month = prevmonth;

                        buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = buttons[column, row].day.ToString();

                    }
                    else if (counter == offset) // if the date is the first day of the month
                    {
                        buttons[column, row].day = 1;
                        buttons[column, row].month = time.Month;
                        buttons[column, row].year = year;
                        buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "1";
                        counter = 1;
                        offset = -1;
                    }
                    else if (counter <= DaysInMonth(year, time.Month)) // if the date is in the month
                    {
                        buttons[column, row].button.transform.GetChild(0).gameObject.GetComponent<Text>().text = counter.ToString();
                        buttons[column, row].day = counter;
                        buttons[column, row].month = time.Month;
                        buttons[column, row].year = year;
                    }
                    else // if the date is in the next month
                    {
                        buttons[column, row].day = counter - DaysInMonth(year, time.Month);
                        if (time.Month + 1 > 12) // wrap around to january if current month is December
                        {
                            buttons[column, row].month = 1;
                            buttons[column, row].year = year + 1;

                        }
                        else
                        {
                            buttons[column, row].month = time.Month + 1;
                            buttons[column, row].year = year;
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
            /// <summary>
            /// method for incrementing the year on the calendar display
            /// </summary>
            year = year + 1;
            inputs.Year = year;
            UpdateButtons();
        }

        //method for decrementing the year on the calendar display
        public void YearDown()
        {
            /// <summary>
            /// method for decrementing the year on the calendar display
            /// </summary>
            year = year - 1;
            inputs.Year = year;
            UpdateButtons();
        }

        //method for incrementing the month on the calendar display
        public void MonthUp()
        {
            /// <summary>
            /// method for incrementing the month on the calendar display
            /// </summary>
            time = time.AddMonths(1);
            UpdateButtons();
        }

        //method for decrementing the month on the calendar display
        public void MonthDown()
        {
            /// <summary>
            /// method for decrementing the month on the calendar display
            /// </summary>
            time = time.AddMonths(-1);
            UpdateButtons();
        }

        //method for handling button input
        public void UpdateDate(GameObject buttonPressed)
        {
            ///<summary>
            /// method used for when one of the day buttons are pressed. Updates the date in InputContainer
            ///</summary>
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (buttons[column, row].button == buttonPressed)
                    {
                        inputs.Time = new(buttons[column, row].year, buttons[column, row].month, buttons[column, row].day, inputs.Time.Hour, inputs.Time.Minute, inputs.Time.Second, inputs.Time.Millisecond, inputs.Time.Kind);
                        inputs.Year = buttons[column, row].year;
                        Debug.Log(inputs.Time.ToString() + "\n" + DateTime.Now.ToString());
                        return;
                    }
                }
            }
        }

        private bool IsLeapYear(int testyear)
        {
            ///<summary>
            /// method to determine if a year is a leap year. The DateTime version breaks on years greater than 9999
            ///</summary>
            return testyear % 4 == 0 && (testyear % 100 != 0 || testyear % 400 == 0);
        }
        private int DaysInMonth(int testyear, int testmonth)
        {
            ///<summary>
            /// method that returns the days in a given month on a given year. The DateTime version breaks on years greater than 9999
            ///</summary>
            switch (testmonth)
            {
                case 1:
                    return 31;
                case 2:
                    if (IsLeapYear(testyear))
                    {
                        return 29;
                    }
                    else
                    {
                        return 28;
                    }
                case 3:
                    return 31;
                case 4:
                    return 30;
                case 5:
                    return 31;
                case 6:
                    return 30;
                case 7:
                    return 31;
                case 8:
                    return 31;
                case 9:
                    return 30;
                case 10:
                    return 31;
                case 11:
                    return 30;
                case 12:
                    return 31;
            }

            return 1000;
        }
    }
}
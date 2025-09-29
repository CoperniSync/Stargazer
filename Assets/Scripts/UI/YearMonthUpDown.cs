using Assets.Scripts.UI;
using System.Globalization;
using UnityEngine;
using UnityEngine.Windows;

public class YearMonthUpDown : MonoBehaviour
{

    InputContainer inputs;
    void Awake()
    {
        inputs = InputContainer.Container;
    }

    public void YearUp()
    {
        inputs.Time = inputs.Time.AddYears(1);
        gameObject.transform.GetComponent<Calendar>().UpdateButtons();
        Debug.Log("buttonPressed");
        Debug.Log(inputs.Time);
    }
    public void YearDown()
    {
        inputs.Time = inputs.Time.AddYears(-1);
        gameObject.transform.GetComponent<Calendar>().UpdateButtons();
        Debug.Log("buttonPressed");
    }

    public void MonthUp()
    {
        inputs.Time = inputs.Time.AddMonths(1);
        gameObject.transform.GetComponent<Calendar>().UpdateButtons();
        Debug.Log("buttonPressed");
    }
    public void MonthDown()
    {
        inputs.Time = inputs.Time.AddMonths(-1);
        gameObject.transform.GetComponent<Calendar>().UpdateButtons();
        Debug.Log("buttonPressed");
    }


}

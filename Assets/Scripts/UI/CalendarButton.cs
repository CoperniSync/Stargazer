using UnityEngine;

// Script for handling button input for the calender widget
// Author: Morgan Hendon
public class CalendarButton : MonoBehaviour
{
    public void onPress()
    {
        this.gameObject.transform.parent.transform.parent.transform.parent.transform.GetComponent<Calendar>().UpdateDate(this.gameObject);
    }
}

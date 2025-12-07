using UnityEngine;

namespace Assets.Scripts.UI
{

    // Script for handling button input for the calender widget
    // Author: Morgan Hendon
    public class CalendarButton : MonoBehaviour
    {
        public void onPress()
        {
            ///<summary>
            /// method that calls UpdateDate in the Calendar class
            ///</summary>
            gameObject.transform.parent.transform.parent.transform.parent.transform.GetComponent<Calendar>().UpdateDate(gameObject);
        }
    }
}
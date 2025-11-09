using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class StarTooltipText : MonoBehaviour
{
    Text textObject;
    void Start()
    {
        textObject = this.transform.GetChild(0).GetComponent<Text>();
    }

    public void updateData(string name, int id, float altitude, float azimuth, float distance)
    {
        textObject.text = name + "\nHIP: " + id + "\nAltitude: " + altitude.ToString("F") + "\nAzimuth " + azimuth.ToString("F") + "\nDistance" + distance + "LY";
    }
}

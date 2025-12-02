using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.CelestialBodies;

public class StarTooltipText : MonoBehaviour
{
    public static StarTooltipText Instance { get; private set; }

    [Header("UI References")]
    public RectTransform panel;  // the tooltip panel
    public Text textObject;      // the Text component showing the info

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Auto-wire child text
        if (textObject == null)
            textObject = GetComponentInChildren<Text>();

        if (panel == null)
            panel = GetComponent<RectTransform>();

        Hide();
    }

    /// <summary>
    /// Update tooltip using raw values.
    /// </summary>
    public void UpdateData(string name, int id, float altitude, float azimuth, float distance)
    {
        if (textObject == null) return;
        
        textObject.text =
            name + "\n" +
            "HIP: " + id + "\n" +
            "Altitude: " + altitude.ToString("F") + "\n" +
            "Azimuth: " + azimuth.ToString("F") + "\n" +
            "Distance: " + distance.ToString("F") + " LY";
    }

    /// <summary>
    /// update from a Star object and position at the mouse.
    /// </summary>
    public void ShowAtMouse(Star star)
    {
        if (star == null || panel == null) return;

        string name = star.StarName;     
        int hipId   = star.HipparcosId;

        var horizontalBody = star.HorizontalBody;

        float altitude = (float)horizontalBody.Altitude;    
        float azimuth  = (float)horizontalBody.Azimuth;
        float distance = (float)horizontalBody.Distance;

        UpdateData(name, hipId, altitude, azimuth, distance);

        panel.gameObject.SetActive(true);
        panel.position = Input.mousePosition;
    }

    public void ShowAtMouse()
    {
        if (panel == null) return;
        panel.gameObject.SetActive(true);
        panel.position = Input.mousePosition;
    }

    public void Hide()
    {
        if (panel != null)
            panel.gameObject.SetActive(false);
    }
}

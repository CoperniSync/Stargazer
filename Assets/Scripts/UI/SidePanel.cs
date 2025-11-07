using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class for changing tabs on the side panel.
/// </summary>
public class SidePanel : MonoBehaviour
{
    private bool hidden; // flag for if the pannel is hidden


    [Tooltip("List of all subpanels to be contained in the SidePanel")] 
    public List<GameObject> subPanels;

   
    void Start()
    { 
        hidden = true;
    }


    // turns a panel on and the rest off.
    // if a panel is already on, hide the sidePanel.
    public void Toggle(GameObject toToggle)
    {

        // update status of the whole panel
        if(toToggle.activeSelf == true && hidden == false)
        {
            this.transform.Translate(new(250, 0, 0));
            hidden = true;
        }
        else if (hidden == true)
        {
            hidden = false;
            this.transform.Translate(new(-250, 0, 0));
        }


        // update each subPanel
        foreach (GameObject g in subPanels)
        {
            if(toToggle == g)
            {
                g.SetActive(!g.activeSelf);
            }
            else
            {
                g.SetActive(false);
            }
        }   
    }
}

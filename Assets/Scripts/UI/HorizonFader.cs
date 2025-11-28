using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

using System;

/// <summary>
/// Class that controls the transparency of the horizion line
/// </summary>
public class HorizonFader : MonoBehaviour
{
    public bool enabled = true;

    // Update is called once per frame
    void Update()
    {
        if(enabled)
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = Math.Clamp((InputContainer.Container.RotationVector.z + 0.5f)/2, 0f, 0.3f);
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        else
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = 0f;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }

    
}

using UnityEngine;

public class StarTooltip : MonoBehaviour
{

    private bool isMouseOver = false;
    public GameObject canvas;
    TooltipGenerator tt;

    private void Start()
    {
       tt = canvas.GetComponent<TooltipGenerator>();

    }
    private void OnMouseOver()
    {
        
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

}

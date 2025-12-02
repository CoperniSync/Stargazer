using Assets.Scripts.CelestialBodies;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;   // Planet

namespace Assets.Scripts.UI.Tooltips
{
    public class MessierHoverDetector : MonoBehaviour
    {
        private Text textObject;
        public GameLoop gameLoop;
        private void Start()
        {
            textObject = StarTooltipText.Instance.textObject;     // the Text component showing the info
            gameLoop = GameObject.FindWithTag("MainCamera").GetComponent<GameLoop>();
        }
        private void OnMouseEnter()
        {
            foreach (MessierObject item in gameLoop.GetMessierList())
            {
                Debug.Log(item.MessierId.ToString());
                if (gameObject == item.Go)
                {
                    textObject.text =
                        item.MessierId + "\n" +
                        "Altitude: " + item.Altitude.ToString("F") + "\n" +
                        "Azimuth: " + item.Azimuth.ToString("F") + "\n" +
                        "Size: " + item.Size.ToString() + "\n" +
                        "Viewing Season: " + item.ViewingSeason;
                    StarTooltipText.Instance.ShowAtMouse();
                    return;
                }
            }

        }

        private void OnMouseExit()
        {
            StarTooltipText.Instance.Hide();
        }

    }
}

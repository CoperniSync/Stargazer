using Assets.Scripts.CelestialBodies;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;   // Planet

namespace Assets.Scripts.UI.Tooltips
{
    public class LocalHoverDetector : MonoBehaviour
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
            Debug.Log("MouseOver");
            var sun = gameLoop.GetSun();
            if (gameObject == sun.Go)
                {
                    textObject.text =
                        "The Sun\n" +
                        "Altitude: " + sun.Altitude.ToString("F") + "\n" +
                        "Azimuth: " + sun.Azimuth.ToString("F") + "\n" +
                        "Distance: " + sun.Distance.ToString("F") + " AU";
                    StarTooltipText.Instance.ShowAtMouse();
                    return;
                }
            var moon = gameLoop.GetSun();
            if (gameObject == moon.Go)
            {
                textObject.text =
                    "The Moon\n" +
                    "Altitude: " + moon.Altitude.ToString("F") + "\n" +
                    "Azimuth: " + moon.Azimuth.ToString("F") + "\n" +
                    "Distance: " + moon.Distance.ToString("F") + " AU";
                StarTooltipText.Instance.ShowAtMouse();
                return;
            }
            foreach (Planet item in gameLoop.GetPlanetList())
            {
                Debug.Log(item.PlanetName.ToString());
                if (gameObject == item.Go)
                {
                    Debug.Log(item.PlanetName.ToString() + " Detected");
                    textObject.text =
                        item.PlanetName + "\n" +
                        "Altitude: " + item.HorizontalPlanet.Altitude.ToString("F") + "\n" +
                        "Azimuth: " + item.HorizontalPlanet.Azimuth.ToString("F") + "\n" +
                        "Distance: " + item.HorizontalPlanet.Distance.ToString("F") + " AU";
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

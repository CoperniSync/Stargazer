using Assets.Scripts.Core;
using ChargerAstronomyShared.Domain;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{

    /// <summary>
    /// MonoBehaviour for updating the engine data displayed to the user
    /// Must be placed on the text object that it will be updating.
    /// </summary>
    public class EngineStateUI : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public GameObject gameLoopObject;
        private GameLoop gameLoop;
        private Text textComp;
        void Start()
        {
            gameLoop = gameLoopObject.GetComponent<GameLoop>();
            textComp = gameObject.transform.GetComponent<Text>();

        }

        // Update is called once per frame
        void Update()
        {

            gameLoop.GetEngineState(out Vector3 cameraDirection, out int year, out DateTime time, out Observer observer);
            Vector3.Angle(cameraDirection, new Vector3(1, 0, 0));
            string directionLabel = "";

            float northAngle = Vector3.Angle(cameraDirection, new Vector3(1, 0, 0));
            float southAngle = Vector3.Angle(cameraDirection, new Vector3(-1, 0, 0));
            float eastAngle = Vector3.Angle(cameraDirection, new Vector3(0, 1, 0));
            float westAngle = Vector3.Angle(cameraDirection, new Vector3(0, -1, 0));
            if (northAngle < westAngle &&
                northAngle < eastAngle)
            {
                directionLabel = "North";
            }
            else if (southAngle < westAngle &&
                southAngle < eastAngle)
            {
                directionLabel = "South";
            }
            else if (southAngle > westAngle &&
                northAngle > westAngle)
            {
                directionLabel = "East";
            }
            else if (southAngle > eastAngle &&
                northAngle > eastAngle)
            {
                directionLabel = "West";
            }

            textComp.text =
                        "Latitude: " + observer.latitude.ToString("F3") + " Longitude: " + observer.longitude.ToString("F3") +
                        "\nTime:" + year.ToString("D4") + "/" + time.Month.ToString("D2") + "/" + time.Day.ToString("D2") + " " +
                        time.Hour.ToString("D2") + ":" + time.Minute.ToString("D2") + ":" + time.Second.ToString("D2") + " UTC\n" +
                        "Facing: " + directionLabel;
        }
    }
}
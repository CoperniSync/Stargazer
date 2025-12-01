using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Author: Morgan Hendon FA 2025

namespace Assets.Scripts.UI.Visual
{
    internal class VisualToggle : MonoBehaviour
    {
        public GameObject horizonLine;
        public GameObject gameLoopObject;
        private GameLoop gameLoop;
        InputContainer inputs;
        void Start()
        {
            inputs = InputContainer.Container; //get the input container instance
            gameLoop = gameLoopObject.GetComponent<GameLoop>();

        }
        public void ToggleConstellations(Toggle toggle)
        {
            inputs.ConstellationsToggle = toggle.isOn;
            gameLoop.SetConstellationVisibility(toggle.isOn);
        }
        public void ToggleMessierObjects(Toggle toggle)
        {
            inputs.MessierToggle = toggle.isOn;
            gameLoop.SetMessierVisibility(toggle.isOn);
        }
        public void ToggleAzimuthGridlines(Toggle toggle)
        {
            inputs.AzimuthLines = toggle.isOn;
        }
        public void ToggleEquatorialGridlines(Toggle toggle)
        {
            inputs.EquatorialLines = toggle.isOn;
        }
        public void ToggleLabels(Toggle toggle)
        {
            inputs.ConstellationLabel = toggle.isOn;
            gameLoop.SetLabelVisibility(toggle.isOn);

        }

        public void ToggleHorizon(Toggle toggle)
        {
            horizonLine.SetActive(toggle.isOn);

        }
    }
}

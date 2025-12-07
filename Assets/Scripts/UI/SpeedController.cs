using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// handles UI input for speed controll
    /// </summary>
    public class SpeedController : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public GameObject gameLoopObject;
        private GameLoop gameLoop;
        InputContainer inputs;
        void Start()
        {
            inputs = InputContainer.Container; //get the input container instance
            gameLoop = gameLoopObject.GetComponent<GameLoop>();

        }

        public void UpdateSpeed()
        {
            var input = gameObject.transform.GetComponent<InputField>().text;
            float inputNumber = 1;

            // check if string is empty
            if (input == "")
            {
                return;
            }
            inputNumber = Math.Clamp(float.Parse(input), 0, 100000);
            if (inputNumber == 0)
            {
                inputNumber = 1;
            }
            gameObject.transform.GetComponent<InputField>().text = inputNumber.ToString();
            gameLoop.SetSpeedMultiplier(inputNumber);
            inputs.SecPerSec = inputNumber;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// Author: Morgan Hendon FA 2025

namespace Assets.Scripts.UI
{


    public class InputContainer
    {
        ///<summary>
        /// Singleton Container for storing user inputs from the UI and transfering them to the Core to be processed.
        ///</summary>



        // make a singleton
        private static InputContainer container = null;

        private InputContainer()
        {
        }
        public static InputContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new InputContainer();
                }
                return container;
            }
        }



        //location
        public int LongitudeDeg {  get; set; }
        public int LatitudeDeg { get; set; }
        public int LongitudeMin { get; set; }
        public int LatitudeMin { get; set; }

        //time
        private DateTime time = new DateTime(2000,1,01,0,0,0);
        public DateTime Time { get => time; set => time = value; } // do not use the built in year

        private int year = 2000;
        public int Year { get => year; set => year = value; }  // seperate variable for year to bypass date

        //labels
        public bool AzimuthLines { get; set; }
        public bool EquatorialLines { get; set; }
        public bool ConstellationLabel { get; set; }

        //space objects
        public bool ConstellationsToggle {  get; set; }
        public bool MessierToggle { get; set; }

        //speed
        public float SecPerSec { get; set; }


        //camera
        public Vector3 RotationVector { get; set; }
        public float HorizontalFOV { get; set; }

    }
}

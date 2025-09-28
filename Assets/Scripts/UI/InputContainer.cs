using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{


    public class InputContainer : MonoBehaviour
    {
        // make a singleton
        public static InputContainer container;
        private void Awake()
        {
            if (container == null)
                container = this;
        }

        //location
        public int LongitudeDeg {  get; set; }
        public int LatitudeDeg { get; set; }
        public int LongitudeMin { get; set; }
        public int LatitudeMin { get; set; }

        //time
        public DateTime Time { get; set; }

        //labels
        public bool AzimuthLabel { get; set; }
        public bool EquatorialLabel { get; set; }
        public bool ConstellationLabel { get; set; }

        //space objects
        public bool ConstellationsToggle {  get; set; }
        public bool MessierToggle { get; set; }

        //speed
        public uint DaysPerSec {  get; set; }
        public uint MinPerSec { get; set; }
        public uint SecPerSec { get; set; }

    }
}

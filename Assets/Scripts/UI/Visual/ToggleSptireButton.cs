using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Visual
{
    public class ToggleButtonSprite : MonoBehaviour
    {
        public Sprite spriteOn; // Assign your "on" sprite in the Inspector
        public Sprite spriteOff; // Assign your "off" sprite in the Inspector
        private Image buttonImage;
        private bool isOn = false; // Initial state

        void Start()
        {
            buttonImage = GetComponent<Image>();
        }

        public void ToggleSprite()
        {
            isOn = !isOn; // Invert the state
            buttonImage.sprite = isOn ? spriteOn : spriteOff; // Change sprite based on new state
        }
    }

}
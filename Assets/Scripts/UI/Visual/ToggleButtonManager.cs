using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Visual
{
    public class ButtonToggleGroup : MonoBehaviour
    {
        [System.Serializable]
        public class Item
        {
            public Button button;      // The button to click
            public Sprite offSprite;   // Default/off sprite
            public Sprite onSprite;    // Toggled/on sprite
        }

        [SerializeField] private List<Item> items = new();
        [SerializeField] private int defaultOn = -1;      // Index of default ON button (-1 = none)
        [SerializeField] private bool allowDeselect = false;

        private int current = -1;

        private void Awake()
        {
            // Assign each button click listener
            for (int i = 0; i < items.Count; i++)
            {
                int index = i; // local copy for lambda
                items[i].button.onClick.AddListener(() => OnClicked(index));
            }
        }

        private void Start()
        {
            // Initialize all buttons to OFF
            for (int i = 0; i < items.Count; i++)
                SetVisual(i, false);

            // Set the default ON button if specified
            if (defaultOn >= 0 && defaultOn < items.Count)
                SetSelected(defaultOn);
        }

        private void OnDestroy()
        {
            // Remove listeners (best practice)
            for (int i = 0; i < items.Count; i++)
            {
                int index = i;
                items[i].button.onClick.RemoveListener(() => OnClicked(index));
            }
        }

        private void OnClicked(int index)
        {
            if (index == current)
            {
                if (allowDeselect)
                    SetSelected(-1);  // turn all off if clicked again
                return;
            }

            SetSelected(index);
        }

        private void SetSelected(int index)
        {
            // Turn the previously selected button OFF
            if (current >= 0 && current < items.Count)
                SetVisual(current, false);

            current = index;

            // Turn the new selected button ON
            if (current >= 0 && current < items.Count)
                SetVisual(current, true);
        }

        private void SetVisual(int index, bool isOn)
        {
            var image = items[index].button.GetComponent<Image>();
            if (image != null)
                image.sprite = isOn ? items[index].onSprite : items[index].offSprite;
        }
    }
}

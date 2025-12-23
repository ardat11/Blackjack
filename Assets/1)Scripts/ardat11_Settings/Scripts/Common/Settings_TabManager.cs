using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ardat11_Settings
{
    public class Settings_TabManager : MonoBehaviour
    {
        [Header("Visual Settings")] [Tooltip("Clicked tab button color")] [SerializeField]
        private Color selectedColor = Color.white;

        [Tooltip("Unclicked tab button color")] [SerializeField]
        private Color deselectedColor = Color.gray;


        [Header("Panels")] [SerializeField] private List<CanvasGroup> panels;


        [Header("Buttons")] [SerializeField] private List<Button> tabButtons;

        private int selectedIndex;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            foreach (CanvasGroup panel in panels)
            {
                panel.SetGroupActive(false);
            }

            foreach (Button button in tabButtons)
            {
                UpdateButtonColor(button, deselectedColor);
            }

            panels[0].SetGroupActive(true);
            UpdateButtonColor(tabButtons[0], selectedColor);
            selectedIndex = 0;
        }

        public void OpenTab(int index)
        {
            if (index == selectedIndex) return;

            panels[selectedIndex].SetGroupActive(false);
            UpdateButtonColor(tabButtons[selectedIndex], deselectedColor);
            selectedIndex = index;
            panels[selectedIndex].SetGroupActive(true);
            UpdateButtonColor(tabButtons[selectedIndex], selectedColor);
        }

        private void UpdateButtonColor(Button button, Color color)
        {
            ColorBlock cb = button.colors;
            cb.normalColor = color;
            button.colors = cb;
        }
    }
}
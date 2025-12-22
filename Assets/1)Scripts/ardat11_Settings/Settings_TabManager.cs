using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Settings_TabManager : MonoBehaviour
{   
    [Header("Visual Settings")]
    
    [Tooltip("Clicked tab button color")]
    [SerializeField] private Color selectedColor = Color.white;
    
    [Tooltip("Unclicked tab button color")]
    [SerializeField] private  Color deselectedColor = Color.gray;

    
    [Header("Panels")] 
    
    [SerializeField] private List<GameObject> panels;
    
    
    [Header("Buttons")]
    
    [SerializeField] private List<Button> tabButtons;

    private int selectedIndex;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        foreach (Button button in tabButtons)
        {
            UpdateButtonColor(button,deselectedColor);
        }
        panels[0].SetActive(true);
        UpdateButtonColor(tabButtons[0],selectedColor);
        selectedIndex = 0;
    }
    
    public void OpenTab(int index)
    {
        if (index == selectedIndex) return;
        
        panels[selectedIndex].SetActive(false);
        UpdateButtonColor(tabButtons[selectedIndex],deselectedColor);
        selectedIndex = index;
        panels[selectedIndex].SetActive(true);
        UpdateButtonColor(tabButtons[selectedIndex],selectedColor);
    }
    private void UpdateButtonColor(Button button,Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }
}
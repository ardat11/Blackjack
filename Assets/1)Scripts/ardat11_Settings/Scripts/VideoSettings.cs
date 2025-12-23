using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ardat11_Settings
{
    public class VideoSettings : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Dropdown resDropdown;
        public TMP_Dropdown hzDropdown;
        public Toggle vsyncToggle;
        public Toggle fullscreenToggle;
        public TMP_Dropdown qualityDropdown;
    
        private Resolution[] allResolutions;
        private List<Resolution> uniqueResolutions = new List<Resolution>();
        private List<RefreshRate> currentResRefreshRates = new List<RefreshRate>();
    
        void Start()
        {
            allResolutions = Screen.resolutions;
            
            // Mevcut ayarları UI'ya yansıt
            fullscreenToggle.isOn = Screen.fullScreen;
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            qualityDropdown.value = QualitySettings.GetQualityLevel();
    
            SetupUniqueResolutions();
        }
    
        void SetupUniqueResolutions()
        {
            resDropdown.ClearOptions();
            var distinctRes = allResolutions
                .GroupBy(r => new { r.width, r.height })
                .Select(g => g.First())
                .OrderByDescending(r => r.width).ToList();
    
            List<string> options = new List<string>();
            int currentResIndex = 0;
            for (int i = 0; i < distinctRes.Count; i++)
            {
                uniqueResolutions.Add(distinctRes[i]);
                options.Add($"{distinctRes[i].width} x {distinctRes[i].height}");
                if (distinctRes[i].width == Screen.width && distinctRes[i].height == Screen.height) currentResIndex = i;
            }
            resDropdown.AddOptions(options);
            resDropdown.value = currentResIndex;
            resDropdown.RefreshShownValue();
            UpdateRefreshRateList(currentResIndex);
        }
    
        public void UpdateRefreshRateList(int resIndex)
        {
            hzDropdown.ClearOptions();
            currentResRefreshRates.Clear();
            Resolution selectedRes = uniqueResolutions[resIndex];
            var rates = allResolutions
                .Where(r => r.width == selectedRes.width && r.height == selectedRes.height)
                .Select(r => r.refreshRateRatio)
                .GroupBy(r => Mathf.RoundToInt((float)r.value))
                .Select(g => g.First()).OrderByDescending(r => r.value).ToList();
    
            List<string> options = new List<string>();
            foreach (var rate in rates)
            {
                currentResRefreshRates.Add(rate);
                options.Add($"{Mathf.RoundToInt((float)rate.value)} Hz");
            }
            hzDropdown.AddOptions(options);
            hzDropdown.RefreshShownValue();
        }
    
        public void ApplySettings()
        {
            Resolution res = uniqueResolutions[resDropdown.value];
            RefreshRate hz = currentResRefreshRates[hzDropdown.value];
        
            // bool yerine FullScreenMode enum'ı kullanıyoruz
            FullScreenMode mode = fullscreenToggle.isOn ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
    
            Screen.SetResolution(res.width, res.height, mode, hz);
        
            QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
            QualitySettings.SetQualityLevel(qualityDropdown.value);
        }
    }
}

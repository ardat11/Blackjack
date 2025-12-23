using System.Collections.Generic;
using ardat11_Localization;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ardat11_Settings
{
    public class GeneralSettings : MonoBehaviour
    {
        [Header("Feature Toggles")] [SerializeField]
        private bool useLocalization = true;

        [SerializeField] private bool useFOV = true;
        [SerializeField] private bool useSensitivity = true;

        [Header("Localization UI")] [SerializeField]
        private GameObject localizationGroup;

        [SerializeField] private TMP_Text languageText;

        [Header("FOV UI")] [SerializeField] private GameObject fovGroup;
        [SerializeField] private Slider fovSlider;
        [SerializeField] private TextMeshProUGUI fovValueText;
        [SerializeField] private Camera mainCamera;

        [Header("Sensitivity UI")] [SerializeField]
        private GameObject sensitivityGroup;

        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private TextMeshProUGUI sensValueText; // Yeni: Hassasiyet metni

        private List<string> availableLanguages = new List<string>();
        private int languageIndex;

        private void OnValidate()
        {
            if (localizationGroup != null) localizationGroup.SetActive(useLocalization);
            if (fovGroup != null) fovGroup.SetActive(useFOV);
            if (sensitivityGroup != null) sensitivityGroup.SetActive(useSensitivity);
        }

        private void Awake()
        {
            if (useLocalization) InitLanguageList();
        }

        void Start()
        {
            // FOV Yükleme
            if (useFOV)
            {
                if (mainCamera == null) mainCamera = Camera.main;
                float savedFOV = PlayerPrefs.GetFloat("PlayerFOV", 60f);
                fovSlider.value = savedFOV;
                mainCamera.fieldOfView = savedFOV;
                UpdateFOVText(savedFOV);
            }

            // Sensitivity Yükleme
            if (useSensitivity)
            {
                float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 1.000f);
                sensitivitySlider.value = savedSens;
                UpdateSensText(savedSens);
            }
        }

        // --- Localization ---
        private void InitLanguageList()
        {
            availableLanguages = LocalizationManager.GetAvailableLanguages();
            SetLanguageIndex(1);
        }

        public void ChangeLanguageIndex(int change)
        {
            if (!useLocalization) return;
            SetLanguageIndex(languageIndex + change);
        }

        private void SetLanguageIndex(int index)
        {
            int count = availableLanguages.Count;
            if (count == 0) return;
            languageIndex = (index % count + count) % count;
            LocalizationManager.CurrentLanguage = availableLanguages[languageIndex];
            UpdateLanguageText();
        }

        private void UpdateLanguageText()
        {
            if (languageText != null)
                languageText.text = LocalizationManager.Localize(availableLanguages[languageIndex]);
        }

        // --- FOV ---
        public void SetFOV(float value)
        {
            if (!useFOV) return;
            if (mainCamera != null) mainCamera.fieldOfView = value;
            PlayerPrefs.SetFloat("PlayerFOV", value);
            UpdateFOVText(value);
        }

        void UpdateFOVText(float value)
        {
            if (fovValueText != null)
                fovValueText.text = Mathf.RoundToInt(value).ToString();
        }

        // --- Sensitivity ---
        public void SetSensitivity(float value)
        {
            if (!useSensitivity) return;
            PlayerPrefs.SetFloat("MouseSensitivity", value);
            UpdateSensText(value);
        }

        void UpdateSensText(float value)
        {
            if (sensValueText != null)
                // "F3" virgülden sonra tam 3 basamak (örn: 1.250) gösterir.
                sensValueText.text = value.ToString("F3");
        }
    }
}
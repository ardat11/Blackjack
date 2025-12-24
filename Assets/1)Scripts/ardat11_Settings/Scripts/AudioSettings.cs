using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace ardat11_Settings
{

    public class AudioSettings : MonoBehaviour
    {
        [Header("Feature Toggles")] [SerializeField]
        public bool useMaster = true;

        [SerializeField] public bool useMusic = true;
        [SerializeField] public bool useSFX = true;

        [Header("References")] [SerializeField]
        private AudioMixer mainMixer;

        [Header("Master UI")] [SerializeField] private GameObject masterGroup;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private TextMeshProUGUI masterValueText;

        [Header("Music UI")] [SerializeField] private GameObject musicGroup;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private TextMeshProUGUI musicValueText;

        [Header("SFX UI")] [SerializeField] private GameObject sfxGroup;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TextMeshProUGUI sfxValueText;

        private void OnValidate()
        {
            if (masterGroup != null) masterGroup.SetActive(useMaster);
            if (musicGroup != null) musicGroup.SetActive(useMusic);
            if (sfxGroup != null) sfxGroup.SetActive(useSFX);
        }

        void Start()
        {
            if (useMaster) LoadVolume("MasterVol", masterSlider, masterValueText);
            if (useMusic) LoadVolume("MusicVol", musicSlider, musicValueText);
            if (useSFX) LoadVolume("SFXVol", sfxSlider, sfxValueText);
        }

        public void SetMasterVol(float val) => SetVolume("MasterVol", val, masterValueText);
        public void SetMusicVol(float val) => SetVolume("MusicVol", val, musicValueText);
        public void SetSFXVol(float val) => SetVolume("SFXVol", val, sfxValueText);

        private void SetVolume(string parameterName, float sliderValue, TextMeshProUGUI text)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20;
            mainMixer.SetFloat(parameterName, dB);
            PlayerPrefs.SetFloat(parameterName, sliderValue);

            if (text != null) text.text = Mathf.RoundToInt(sliderValue * 100) + "%";
        }

        private void LoadVolume(string parameterName, Slider slider, TextMeshProUGUI text)
        {
            float savedVal = PlayerPrefs.GetFloat(parameterName, 0.75f);
            slider.value = savedVal;
            SetVolume(parameterName, savedVal, text);
        }
    }
}
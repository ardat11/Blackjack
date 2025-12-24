using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ardat11_Localization
{
    [CreateAssetMenu(fileName = "LocalizationSettings", menuName = "Localization/Settings")]
    public class LocalizationSettings : ScriptableObject
    {
        [Header("Google Sheets Config")] 
        [Tooltip("Add multiple CSV links for different tabs/sheets here.")]
        public List<string> googleSheetsUrls = new List<string>();

        [Header("Language Setup")] 
        public string defaultLanguage = "en";
        public List<string> languageCodes = new List<string>();

        [Header("Storage")] 
        public string saveFileName = "LocalizationData";

        private void OnValidate()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ardat11_Localization
{
    public static class LocalizationManager
    {
        private static Dictionary<string, string[]> _database = new Dictionary<string, string[]>();
        private static Dictionary<string, int> _languageIndices = new Dictionary<string, int>();
        private static string _currentLanguage;
        private static LocalizationSettings _settings;

        public static event Action OnLanguageChanged;

        public static string CurrentLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_currentLanguage)) 
                {
                    CheckAndInitialize();
                }
                
                return string.IsNullOrEmpty(_currentLanguage) ? "" : _currentLanguage;
            }
            set
            {
                _currentLanguage = value;
                OnLanguageChanged?.Invoke();
            }
        }

        public static string Localize(string key)
        {
            CheckAndInitialize();

            string lang = CurrentLanguage;
            if (string.IsNullOrEmpty(lang)) return $"MISSING_{key}";

            if (_database.TryGetValue(key, out var translations))
            {
                if (_languageIndices.TryGetValue(lang, out int index))
                {
                    return index < translations.Length ? translations[index] : "NULL";
                }
            }
            return $"MISSING_{key}";
        }

        public static List<string> GetAvailableLanguages()
        {
            CheckAndInitialize();
            return _settings != null ? new List<string>(_settings.languageCodes) : new List<string>();
        }

        private static void CheckAndInitialize()
        {
            if (_database.Count == 0 || _settings == null)
            {
                _settings = Resources.Load<LocalizationSettings>("LocalizationSettings");
                if (_settings != null)
                {
                    Initialize(_settings);
                    
                    if (string.IsNullOrEmpty(_currentLanguage)) 
                        _currentLanguage = _settings.defaultLanguage;
                }
            }
        }

        public static void Initialize(LocalizationSettings settings)
        {
            _settings = settings;
            _database.Clear();
            _languageIndices.Clear();

            if (_settings.languageCodes != null)
            {
                for (int i = 0; i < _settings.languageCodes.Count; i++)
                {
                    _languageIndices[_settings.languageCodes[i]] = i;
                }
            }

            TextAsset asset = Resources.Load<TextAsset>(settings.saveFileName);
            if (asset != null)
            {
                Parse(asset.text);
                Debug.Log($"<color=cyan>Localization Initialized:</color> {_database.Count} keys loaded.");
            }
        }

        private static void Parse(string rawText)
        {
            string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1) return;

            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] cols = csvParser.Split(lines[i]);
                if (cols.Length < 2) continue;

                string key = cols[0].Trim().Trim('"');
                string[] translations = new string[_settings.languageCodes.Count];

                for (int j = 0; j < _settings.languageCodes.Count; j++)
                {
                    if (j + 1 < cols.Length)
                    {
                        translations[j] = cols[j + 1].Trim().Trim('"').Replace("\"\"", "\"");
                    }
                }
                _database[key] = translations;
            }
        }
    }
}
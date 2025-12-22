using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    private static Dictionary<string, Dictionary<string, string>> _database = new Dictionary<string, Dictionary<string, string>>();
    private static string _currentLanguage;
    private static LocalizationSettings _settings;

    public static event Action OnLanguageChanged;

    public static string CurrentLanguage
    {
        get
        {
            if (string.IsNullOrEmpty(_currentLanguage)) CheckAndInitialize();
            return _currentLanguage;
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

        if (_database.TryGetValue(key, out var translations))
        {
            return translations.ContainsKey(CurrentLanguage) ? translations[CurrentLanguage] : "NULL";
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
                // Note: We don't override _currentLanguage if it's already set by user
                if(string.IsNullOrEmpty(_currentLanguage)) _currentLanguage = _settings.defaultLanguage;
                Initialize(_settings);
            }
        }
    }

    public static void Initialize(LocalizationSettings settings)
    {
        _settings = settings;
        _database.Clear();

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
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 2) continue;

            string key = cols[0].Trim();
            var dict = new Dictionary<string, string>();

            for (int j = 0; j < _settings.languageCodes.Count; j++)
            {
                if (j + 1 < cols.Length)
                    dict.Add(_settings.languageCodes[j], cols[j + 1].Trim());
            }
            _database[key] = dict;
        }
    }
}
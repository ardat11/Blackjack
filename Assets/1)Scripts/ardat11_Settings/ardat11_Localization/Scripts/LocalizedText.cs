using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Settings")]
    public string localizationKey;
    
    [SerializeField] private TextMeshProUGUI _tmp;
    

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= Refresh;
    }

    public void Refresh()
    {
        if (_tmp != null)
        {
            _tmp.text = LocalizationManager.Localize(localizationKey);
        }
    }
}
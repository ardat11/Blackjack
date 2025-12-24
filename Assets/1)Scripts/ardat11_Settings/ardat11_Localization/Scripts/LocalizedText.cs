using UnityEngine;
using TMPro;

namespace ardat11_Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string localizationKey;

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
}
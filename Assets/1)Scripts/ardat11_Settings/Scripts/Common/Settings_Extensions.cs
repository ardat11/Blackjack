using UnityEngine;

namespace ardat11_Settings
{
    public static class CanvasGroupExtensions
    {
        public static void SetGroupActive(this CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1f : 0f;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ardat11_Localization
{
    public class LocalizationEditor : EditorWindow
    {
        private LocalizationSettings settings;

        [MenuItem("Tools/Localization/Open Sync Window")]
        public static void ShowWindow() => GetWindow<LocalizationEditor>("Localizer Sync");

        private void OnGUI()
        {
            settings = (LocalizationSettings)EditorGUILayout.ObjectField("Settings Asset", settings, typeof(LocalizationSettings), false);

            if (settings == null) {
                EditorGUILayout.HelpBox("Please drag and drop the LocalizationSettings asset!", MessageType.Info);
                return;
            }

            if (GUILayout.Button("Download & Merge All Sheets"))
            {
                DownloadCSV();
            }

            GUI.color = Color.red; 
            if (GUILayout.Button("Reset Static Manager & Cache"))
            {
                ResetManager();
            }
            GUI.color = Color.white;
        }

        private void DownloadCSV()
        {
            string combinedContent = "";
            int successfulDownloads = 0;

            foreach (string url in settings.googleSheetsUrls)
            {
                if (string.IsNullOrEmpty(url)) continue;

                UnityWebRequest www = UnityWebRequest.Get(url);
                var op = www.SendWebRequest();
                while (!op.isDone) { }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string content = www.downloadHandler.text;

                    // Skip header row for subsequent sheets to avoid duplicate keys
                    if (successfulDownloads > 0)
                    {
                        int firstNewLine = content.IndexOf('\n');
                        if (firstNewLine != -1) content = content.Substring(firstNewLine + 1);
                    }

                    combinedContent += content + "\n";
                    successfulDownloads++;
                }
            }

            if (successfulDownloads > 0)
            {
                SaveFile(combinedContent);
                Debug.Log($"<color=green>Success!</color> {successfulDownloads} sheets merged and downloaded.");
            }
        }

        private void SaveFile(string content)
        {
            var script = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            string editorFolder = Path.GetDirectoryName(scriptPath);
            string parentPath = Path.GetDirectoryName(editorFolder);
            string resourcesPath = Path.Combine(parentPath, "Resources");

            if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);

            string finalPath = Path.Combine(resourcesPath, settings.saveFileName + ".txt");
            File.WriteAllText(finalPath, content);
        
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ResetManager();
        }

        private void ResetManager()
        {
            if (settings != null)
            {
                LocalizationManager.Initialize(settings);
                // Refresh all UI elements in the scene immediately
                foreach (var textElement in FindObjectsOfType<LocalizedText>())
                {
                    textElement.Refresh();
                }
                Debug.Log("<color=yellow>Localization Manager and Scene UI have been reset.</color>");
            }
        }
    }
}
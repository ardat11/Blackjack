using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ardat11_Localization
{
    public class LocalizationEditor : EditorWindow
    {
        private LocalizationSettings settings;
        private Queue<string> downloadQueue = new Queue<string>();
        private string combinedContent = "";
        private int totalToDownload;
        private int currentDownloadedCount;

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
                StartDownload();
            }

            GUI.color = Color.red; 
            if (GUILayout.Button("Reset Static Manager & Cache"))
            {
                ResetManager();
            }
            GUI.color = Color.white;
        }

        private void StartDownload()
        {
            combinedContent = "";
            downloadQueue = new Queue<string>(settings.googleSheetsUrls);
            totalToDownload = downloadQueue.Count;
            currentDownloadedCount = 0;
            
            if (totalToDownload == 0) return;

            ProcessNextInQueue();
        }

        private void ProcessNextInQueue()
        {
            if (downloadQueue.Count == 0)
            {
                EditorUtility.ClearProgressBar();
                SaveFile(combinedContent);
                return;
            }

            // Show progress in Unity
            float progress = (float)currentDownloadedCount / totalToDownload;
            EditorUtility.DisplayProgressBar("Localization Sync", $"Downloading sheet {currentDownloadedCount + 1} of {totalToDownload}...", progress);

            string url = downloadQueue.Dequeue();
            if (string.IsNullOrEmpty(url))
            {
                currentDownloadedCount++;
                ProcessNextInQueue();
                return;
            }

            UnityWebRequest www = UnityWebRequest.Get(url);
            var operation = www.SendWebRequest();

            EditorApplication.CallbackFunction updateCallback = null;
            updateCallback = () =>
            {
                if (operation.isDone)
                {
                    EditorApplication.update -= updateCallback;
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        string content = www.downloadHandler.text;
                        if (combinedContent != "")
                        {
                            int firstLine = content.IndexOf('\n');
                            if (firstLine != -1) content = content.Substring(firstLine + 1);
                        }
                        combinedContent += content + "\n";
                    }
                    
                    currentDownloadedCount++;
                    ProcessNextInQueue();
                }
            };
            EditorApplication.update += updateCallback;
        }

        private void SaveFile(string content)
        {
            var script = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            string resourcesPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)), "Resources");

            if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);

            File.WriteAllText(Path.Combine(resourcesPath, settings.saveFileName + ".txt"), content);
        
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ResetManager();
            Debug.Log($"<color=green>Localization Download Complete!</color> Merged {totalToDownload} sheets.");
        }

        private void ResetManager()
        {
            if (settings != null)
            {
                LocalizationManager.Initialize(settings);
                foreach (var textElement in FindObjectsOfType<LocalizedText>())
                {
                    textElement.Refresh();
                }
            }
        }
    }
}



namespace ardat11_Settings
{
    #if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(GeneralSettings))]
    public class GeneralSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Feature Toggles", EditorStyles.boldLabel);

            SerializedProperty useLoc = serializedObject.FindProperty("useLocalization");
            SerializedProperty useFOV = serializedObject.FindProperty("useFOV");
            SerializedProperty useSens = serializedObject.FindProperty("useSensitivity");

            EditorGUILayout.PropertyField(useLoc);
            EditorGUILayout.PropertyField(useFOV);
            EditorGUILayout.PropertyField(useSens);

            EditorGUILayout.Space(10);

            if (useLoc.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Localization Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("localizationGroup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("languageText"));
                EditorGUILayout.EndVertical();
            }

            if (useFOV.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("FOV Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fovGroup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fovSlider"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fovValueText"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCamera"));
                EditorGUILayout.EndVertical();
            }

            if (useSens.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Sensitivity Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivityGroup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivitySlider"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sensValueText"));
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
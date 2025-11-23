#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace GameplayTags.Editor
{
    public class GameplayTagJsonImporterWindow : EditorWindow
    {
        private TextAsset jsonFile;
        private string outputFolder = "Assets/Tags";

        private Vector2 scroll;
        private string log = "";

        [MenuItem("Tools/Gameplay Tags/JSON Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameplayTagJsonImporterWindow>("Gameplay Tag JSON Importer");
            window.minSize = new Vector2(500, 350);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Gameplay Tag JSON Importer", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "This tool reads a JSON file containing Gameplay Tags and generates/updates ScriptableObjects for each tag.\n\n" +
                "Format:\n{\n  \"tags\": [\n    {\"tag\": \"character.player\", \"description\": \"The player character.\"}\n  ]\n}",
                MessageType.Info);

            EditorGUILayout.Space();

            jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Tags", GUILayout.Height(30)))
            {
                GenerateTags();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Console Output:", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(150));
            EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void GenerateTags()
        {
            log = "";
            if (jsonFile == null)
            {
                LogError("No JSON file assigned!");
                return;
            }

            try
            {
                var json = JsonUtility.FromJson<TagListWrapper>(jsonFile.text);
                if (json == null || json.tags == null || json.tags.Length == 0)
                {
                    LogError("JSON file is empty or invalid format.");
                    return;
                }

                if (!AssetDatabase.IsValidFolder(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                    AssetDatabase.Refresh();
                }

                int createdTags = 0;
                string[] allGuids = AssetDatabase.FindAssets("t:GameplayTag");

                foreach (var entry in json.tags)
                {
                    if (string.IsNullOrWhiteSpace(entry.tag))
                        continue;

                    string tagName = entry.tag;

                    if (!Utility.GameplayTagUtility.IsValidTagName(tagName))
                    {
                        LogError($"Invalid tag name: {tagName}. Skipping.", false);
                        continue;
                    }

                    string description = entry.description ?? "";
                    string assetPath = $"{outputFolder}/{SanitizeFileName(tagName)}.asset";

                    GameplayTag existing = AssetDatabase.LoadAssetAtPath<GameplayTag>(assetPath);

                    if (CheckDuplicate(tagName, ref allGuids))
                    {
                        continue;
                    }

                    if (existing)
                    {
                        UpdateDescription(existing, description);
                        Log($"Updated existing tag: {tagName}");
                        continue;
                    }

                    var newTag = GameplayTag.CreateInstance<GameplayTag>();
                    newTag.name = tagName;

                    SetDescription(newTag, description);

                    AssetDatabase.CreateAsset(newTag, assetPath);
                    createdTags++;

                    Log($"Created new tag: {tagName}",false);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();


                Log($"Finished generating {createdTags} GameplayTags.",false);
            }
            catch (Exception ex)
            {
                LogError($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private bool CheckDuplicate(string currentName, ref string[] allGuids)
        {
            foreach (var guid in allGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameplayTag other = AssetDatabase.LoadAssetAtPath<GameplayTag>(path);

                if (other)
                {
                    string otherName = Path.GetFileNameWithoutExtension(path);
                    if (otherName == currentName.ToLower())
                        return true;
                }
            }

            return false;
        }

        private static void SetDescription(GameplayTag tag, string description)
        {
            var so = new SerializedObject(tag);
            var prop = so.FindProperty("Description");
            if (prop != null)
            {
                prop.stringValue = description;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void UpdateDescription(GameplayTag tag, string newDesc)
        {
            var so = new SerializedObject(tag);
            var prop = so.FindProperty("Description");
            if (prop != null && prop.stringValue != newDesc)
            {
                prop.stringValue = newDesc;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(tag);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private void Log(string message, bool showInConsole = true)
        {
            log += $"✅ {message}\n";
            if (showInConsole)
            {
                Debug.Log(message);
            }
        }

        private void LogError(string message, bool showInConsole = true)
        {
            log += $"❌ {message}\n";
            if (showInConsole)
            {
                Debug.LogError(message);
            }
        }

        [Serializable]
        private class TagListWrapper
        {
            public TagEntry[] tags;
        }

        [Serializable]
        private class TagEntry
        {
            public string tag;
            public string description;
        }
    }
}
#endif
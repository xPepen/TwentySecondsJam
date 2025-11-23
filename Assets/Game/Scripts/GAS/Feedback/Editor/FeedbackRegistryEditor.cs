namespace Game.Scripts.GAS.Feedback.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;

    public class FeedbackRegistryEditor : AssetPostprocessor
    {
        private const string RegistryPath = "Assets/Resources/FeedbackRegistry/FeedbackRegistry.asset";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool shouldUpdate = false;

            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".asset") &&
                    AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(FeedbackBaseTemplate))
                {
                    shouldUpdate = true;
                    break;
                }
            }

            foreach (string path in deletedAssets)
            {
                if (path.EndsWith(".asset"))
                {
                    shouldUpdate = true;
                    break;
                }
            }

            if (shouldUpdate)
                UpdateRegistry();
        }

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            // Run once on editor load
            EditorApplication.delayCall += UpdateRegistry;
        }

        private static void UpdateRegistry()
        {
            string[] guids = AssetDatabase.FindAssets("t:FeedbackBaseTemplate");
            List<FeedbackBaseTemplate> templates = new();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<FeedbackBaseTemplate>(path);
                if (asset != null && !templates.Contains(asset))
                    templates.Add(asset);
            }

            // Ensure directory exists
            string dir = Path.GetDirectoryName(RegistryPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                AssetDatabase.Refresh();
            }

            // Load or create registry
            FeedbackRegistryAsset registry = AssetDatabase.LoadAssetAtPath<FeedbackRegistryAsset>(RegistryPath);
            if (registry == null)
            {
                registry = ScriptableObject.CreateInstance<FeedbackRegistryAsset>();
                AssetDatabase.CreateAsset(registry, RegistryPath);
                Debug.Log($"[FeedbackRegistryUpdater] Created new registry asset at {RegistryPath}");
            }

            registry.SetTemplates(templates);
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            // Debug.Log($"[FeedbackRegistryUpdater] Updated Feedback Registry with {templates.Count} templates.");
        }
    }
#endif
}
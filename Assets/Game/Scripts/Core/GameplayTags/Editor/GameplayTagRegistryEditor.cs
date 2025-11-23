#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameplayTags.Editor
{
    public class GameplayTagRegistryEditor : AssetPostprocessor
    {
        private const string RegistryPath = "Assets/Resources/Registry/GameplayTagRegistry.asset";
        private const string assetExtension = ".asset";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool shouldUpdate = false;

            foreach (string path in importedAssets)
            {
                shouldUpdate = path.EndsWith(assetExtension) && AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(GameplayTag);
              
                if (shouldUpdate)
                {
                    break;
                }
            }

            foreach (string path in deletedAssets)
            {
                shouldUpdate = path.EndsWith(assetExtension);

                if(shouldUpdate)
                {
                    break;
                }
            }

            if (shouldUpdate)
            {
                UpdateRegistry();
            }
        }

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorApplication.delayCall += UpdateRegistry;
        }

        private static void UpdateRegistry()
        {
            string[] guids = AssetDatabase.FindAssets("t:GameplayTag");
            List<GameplayTag> templates = new();
            HashSet<string> normalizedNames = new();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameplayTag>(path);
                if (asset == null)
                    continue;

                string tagName = asset.name;

                // Reject empty or invalid names
                if (string.IsNullOrWhiteSpace(tagName))
                {
                    Debug.LogWarning($"[GameplayTagRegistryEditor] Skipping unnamed GameplayTag at {path}");
                    continue;
                }

                //Reject any tag that contains a digit
                if (!Utility.GameplayTagUtility.IsValidTagName(tagName))
                {
                    Debug.LogWarning(
                        $"[GameplayTagRegistryEditor] Tag '{tagName}' at {path} contains a invalid syntax and will be excluded. " +
                        "Tag names must only contain letters and dots.");
                    continue;
                }

                // Normalize name (remove non-letters and lowercase)
                string normalized = NormalizeTagName(tagName);

                // Skip if duplicate normalized name already exists
                if (!normalizedNames.Add(normalized))
                {
                    Debug.LogWarning(
                        $"[GameplayTagRegistryEditor] Duplicate or similar tag detected: '{tagName}' at {path}. " +
                        "It will be excluded from the registry.");
                    continue;
                }

                templates.Add(asset);
            }

            string dir = System.IO.Path.GetDirectoryName(RegistryPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
                AssetDatabase.Refresh();
            }

            // Load or create registry
            GameplayTagsRegistry registry =
                AssetDatabase.LoadAssetAtPath<GameplayTagsRegistry>(RegistryPath);

            if (registry == null)
            {
                registry = ScriptableObject.CreateInstance<GameplayTagsRegistry>();
                AssetDatabase.CreateAsset(registry, RegistryPath);
                Debug.Log($"[GameplayTagRegistryEditor] Created new registry at {RegistryPath}");
            }

            registry.SetTemplates(templates);
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
        }

        private static string NormalizeTagName(string input)
        {
            return Regex.Replace(input, "[^a-zA-Z]", "").ToLowerInvariant();
        }
    }
}
#endif
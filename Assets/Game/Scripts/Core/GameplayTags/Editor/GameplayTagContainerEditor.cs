#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameplayTags.Editor
{
    [CustomPropertyDrawer(typeof(GameplayTagContainer))]
    public class GameplayTagContainerDrawer : PropertyDrawer
    {
        private static Dictionary<string, int> lastSizes = new Dictionary<string, int>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty tagsProp = property.FindPropertyRelative("tags");
            EditorGUI.PropertyField(position, tagsProp, label, true);

            if (tagsProp != null && tagsProp.isArray)
            {
                int size = tagsProp.arraySize;
                string path = tagsProp.propertyPath;

                if (!lastSizes.TryGetValue(path, out var lastSize))
                    lastSize = size;

                // Detect new element added
                if (size > lastSize)
                {
                    SerializedProperty newElement = tagsProp.GetArrayElementAtIndex(size - 1);
                    if (newElement != null)
                    {
                        newElement.objectReferenceValue = null; // force new entries to null
                    }
                }

                lastSizes[path] = size;

                // 🔒 Deduplicate
                HashSet<Object> seen = new HashSet<Object>();
                for (int i = tagsProp.arraySize - 1; i >= 0; i--)
                {
                    SerializedProperty element = tagsProp.GetArrayElementAtIndex(i);
                    Object value = element.objectReferenceValue;
                    if (value != null && !seen.Add(value))
                    {
                        element.objectReferenceValue = null;
                        tagsProp.DeleteArrayElementAtIndex(i);
                    }
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("tags"), label, true);
        }
    }
}
#endif
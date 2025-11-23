#if UNITY_EDITOR

namespace GameplayTags.Editor
{
    [UnityEditor.CustomEditor(typeof(GameplayTagsRegistry))]
    public class GameplayTagRegistryAssetEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty _gameplayTagTemplatesProp;

        private void OnEnable()
        {
            _gameplayTagTemplatesProp = serializedObject.FindProperty("listOfTags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Make the list read-only
            UnityEngine.GUI.enabled = false;

            UnityEditor.EditorGUILayout.PropertyField(_gameplayTagTemplatesProp,
                new UnityEngine.GUIContent("Registered GameplayTag Template"),
                true);
            UnityEngine.GUI.enabled = true;

            UnityEditor.EditorGUILayout.Space(10);
            UnityEditor.EditorGUILayout.HelpBox(
                "This list is auto-generated. Do not modify manually.\n" +
                "It updates automatically when FeedbackBaseTemplate assets are created, moved, or deleted.",
                UnityEditor.MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
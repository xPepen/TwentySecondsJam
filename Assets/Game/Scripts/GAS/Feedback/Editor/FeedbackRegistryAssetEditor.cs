namespace Game.Scripts.GAS.Feedback
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(FeedbackRegistryAsset))]
    public class FeedbackRegistryAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty feedbackTemplatesProp;

        private void OnEnable()
        {
            feedbackTemplatesProp = serializedObject.FindProperty("feedbackTemplates");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false; // make entire section read-only
            EditorGUILayout.PropertyField(feedbackTemplatesProp, new GUIContent("Registered Feedback Templates"), true);
            GUI.enabled = true;

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "This list is auto-generated. Do not modify manually.\n" +
                "It updates automatically when FeedbackBaseTemplate assets are created, moved, or deleted.",
                MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}
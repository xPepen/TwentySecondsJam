using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Runtime.Utility.Editor
{
    [CustomEditor(typeof(ApplyValueToEntity))]
    public class ApplyValueToEntityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            ApplyValueToEntity applyDamage = (ApplyValueToEntity)target;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Apply Damage to Player"))
                {
                    applyDamage.ApplyValue(null);
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("You must be in Play Mode to apply damage.", MessageType.Info);
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Runtime.Utility.Editor
{
    [CustomEditor(typeof(ApplyValueToEntityMono))]
    public class ApplyValueToEntityMonoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            ApplyValueToEntityMono applyDamage = (ApplyValueToEntityMono)target;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Apply Damage to Entity"))
                {
                    applyDamage.ApplyValue();
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("You must be in Play Mode to apply damage.", MessageType.Info);
            }
        }
    }
}
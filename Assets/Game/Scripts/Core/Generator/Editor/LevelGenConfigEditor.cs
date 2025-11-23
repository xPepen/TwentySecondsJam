using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Core.Generator.Editor
{
    [CustomEditor(typeof(LevelGenConfig))]
    public class LevelGenConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            LevelGenConfig config = (LevelGenConfig)target;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Generate"))
                {
                    LevelGenerator LG = Subsystem.Get<LevelGenerator>();
                    LG.SetConfig(config);
                    LG.Generate();
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("You must be in Play Mode to generate.", MessageType.Info);
            }
        }
    }
}
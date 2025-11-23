namespace Game.Scripts.GAS.Feedback
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FeedbackRegistryAsset : ScriptableObject
    {
        [SerializeField] private List<FeedbackBaseTemplate> feedbackTemplates = new();
        public IReadOnlyList<FeedbackBaseTemplate> FeedbackTemplates => feedbackTemplates;

#if UNITY_EDITOR
        public void SetTemplates(List<FeedbackBaseTemplate> list)
        {
            feedbackTemplates = list;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
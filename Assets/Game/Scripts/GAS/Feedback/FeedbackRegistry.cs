using GameplayTags;

namespace Game.Scripts.GAS.Feedback
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class FeedbackRegistry
    {
        private static readonly Dictionary<GameplayTag, FeedbackBaseTemplate> Feedbacks = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitManager()
        {
            var feedbackRegistry = Resources.Load<FeedbackRegistryAsset>("FeedbackRegistry/FeedbackRegistry");

            if (feedbackRegistry == null)
            {
                Debug.LogWarning("No FeedbackRegistry found in Resources.");
                return;
            }

            foreach (var feedback in feedbackRegistry.FeedbackTemplates)
            {
                var gameplayTag = feedback.FeedbackTag;
                
                if (gameplayTag == null)
                {
                    Debug.LogWarning($"feedback {feedback} has no tag");
                    continue;
                }

                Feedbacks.TryAdd(gameplayTag, feedback);
            }
        }

        public static void PlayFeedback(GameplayTag tag, object source, Vector3 location)
        {
            if (TryGet(tag,out var feedbackTemplate))
            {
                feedbackTemplate.PlayFeedback(source, location);
            }
        }

        private static FeedbackBaseTemplate Get(GameplayTag tag)
        {
            if (tag == null)
                return null;

            Feedbacks.TryGetValue(tag, out var feedback);
            return feedback;
        }

        public static IEnumerable<FeedbackBaseTemplate> GetAll() => Feedbacks.Values;

        public static bool TryGet(GameplayTag tag, out FeedbackBaseTemplate feedback)
        {
            if (tag == null)
            {
                feedback = null;
                return false;
            }
            return Feedbacks.TryGetValue(tag, out feedback);
        }
    }
}
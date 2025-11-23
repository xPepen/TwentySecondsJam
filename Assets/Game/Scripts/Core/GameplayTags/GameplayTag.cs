using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayTags
{
    [CreateAssetMenu(menuName = "GameplayTag", fileName = "NewGameplayTag", order = 0)]
    public class GameplayTag : ScriptableObject
    {
        //~Start of static Tag handling
        private static readonly Dictionary<int, GameplayTag> GameplayTags = new();
        public static readonly bool ShouldShowLOG = false;

        //Create a new tag at runtime will not be visible for editor
        public static GameplayTag CreateTag(string newTagName)
        {
            if (string.IsNullOrWhiteSpace(newTagName))
                return null;

            newTagName = newTagName.ToLowerInvariant();

            int tagID = Utility.GameplayTagUtility.GetStableHash(newTagName);
            GameplayTag tag = RequestTagInternal(tagID);

            if (tag)
                return tag;

            if (!Utility.GameplayTagUtility.IsValidTagName(newTagName))
            {
                if (ShouldShowLOG)
                    Debug.LogError($"Runtime creation gameplayTag : {newTagName} failed. Invalid tag name.");
                return null;
            }

            var gameplayTag = CreateInstance<GameplayTag>();
            gameplayTag.name = newTagName;
            gameplayTag._id = tagID;

            GameplayTags.Add(tagID, gameplayTag);
            return gameplayTag;
        }

        //Note: Every GameplayTag will be registered as lower and compared as lower case
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitManager()
        {
            var registry = Resources.Load<GameplayTagsRegistry>("Registry/GameplayTagRegistry");
            if (registry == null)
                return;

            foreach (var gameplayTag in registry.Items)
            {
                var tagLower = gameplayTag.tag.ToLowerInvariant();
                int id = Utility.GameplayTagUtility.GetStableHash(tagLower);
                GameplayTags.TryAdd(id, gameplayTag);

                if (ShouldShowLOG)
                    Debug.Log($"Registered GameplayTag: {gameplayTag.tag} with ID: {id}");
            }
        }

        public static GameplayTag RequestTag(ReadOnlySpan<char> tagName)
        {
            if (tagName.IsEmpty)
                return null;

            string lower = tagName.ToString().ToLowerInvariant();
            int id = Utility.GameplayTagUtility.GetStableHash(lower);

            GameplayTags.TryGetValue(id, out var gameplayTag);
            return gameplayTag;
        }

        private static GameplayTag RequestTagInternal(int id)
        {
            GameplayTags.TryGetValue(id, out var gameplayTag);
            return gameplayTag;
        }

        public static bool TagsEqual(GameplayTag tagA, GameplayTag tagB)
        {
            if (tagA == null || tagB == null)
                return false;

            return tagA.GetID() == tagB.GetID();
        }

        public static bool TagsEqual(int tagA, int tagB)
        {
            return tagA == tagB;
        }

        public static bool IsValid(GameplayTag gameplayTag)
        {
            if (gameplayTag == null)
                return false;

            int id = gameplayTag.GetID();
            return GameplayTags.ContainsKey(id);
        }
        
        public static bool IsValid(int tagID)
        {
            return GameplayTags.ContainsKey(tagID);
        }

        public static bool IsValid(ReadOnlySpan<char> tag)
        {
            string lower = tag.ToString().ToLowerInvariant();
            int id = Utility.GameplayTagUtility.GetStableHash(lower);
            return GameplayTags.ContainsKey(id);
        }

        private static bool TagExistInternal(int id)
        {
            return GameplayTags.ContainsKey(id);
        }

        //~End of static Tag handling

        private int _id = -1; // -1 means invalid tag
        private string tag => name.ToLowerInvariant();

        [Tooltip("Description of the Gameplay Tag - for documentation purposes only")] [Multiline] [SerializeField]
        private string Description;

        public bool IsEqualToTag(GameplayTag otherTag)
        {
            return otherTag && GetID() == otherTag.GetID();
        }

        public bool IsEqualToTag(ReadOnlySpan<char> tagName)
        {
            string lower = tagName.ToString().ToLowerInvariant();
            int otherId = Utility.GameplayTagUtility.GetStableHash(lower);

            if (!TagExistInternal(otherId))
            {
                return false;
            }

            return GetID() == otherId;
        }

        public int GetID()
        {
            if (_id == -1)
            {
                string lower = tag.ToLowerInvariant();
                _id = Utility.GameplayTagUtility.GetStableHash(lower);
            }

            return _id;
        }

        public static implicit operator int(GameplayTag gameplayTag)
        {
            return gameplayTag ? gameplayTag.GetID() : -1;
        }

        public static implicit operator string(GameplayTag gameplayTag)
        {
            return gameplayTag ? gameplayTag.tag : string.Empty;
        }
    }
}
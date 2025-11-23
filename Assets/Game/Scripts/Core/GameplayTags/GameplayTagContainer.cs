using System.Collections.Generic;
using UnityEngine;


namespace GameplayTags
{
    [System.Serializable]
    public class GameplayTagContainer
    {
        [field: UnityEngine.SerializeField] private List<GameplayTag> tags = new List<GameplayTag>();

        public void AddTag(GameplayTag tag)
        {
            if (tag == null || MatchTag(tag))
            {
                return;
            }

            if (!MatchTag(tag))
            {
                tags.Add(tag);
            }
        }

        public void RemoveTag(GameplayTag tag)
        {
            if (tag == null)
            {
                return;
            }

            if (MatchTag(tag))
            {
                tags.Remove(tag);
            }
        }

        public GameplayTag GetTag(GameplayTag tag)
        {
            return !tag ? null : tags.Find(t => t.Equals(tag));
        }

        public GameplayTag GetTag(int index)
        {
            if (tags.Count == 0 )
            {
                return null;
            }

            //means out of range, return last tag
            if (index > tags.Count - 1)
            {
                return tags[^1];
            }
            
            return tags[UnityEngine.Mathf.Max(0, index)];
        }

        public bool MatchAllTags(GameplayTagContainer other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.tags.Count == 0)
            {
                return false;
            }

            foreach (var tag in other.tags)
            {
                if (!MatchTag(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public bool MatchTag(GameplayTag tag)
        {
            return tag != null && tags.Contains(tag);
        }

        public void Clear()
        {
            tags.Clear();
        }
    }
}
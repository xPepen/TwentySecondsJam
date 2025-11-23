using System.Collections.Generic;

namespace GameplayTags
{
    public class GameplayTagsRegistry : UnityEngine.ScriptableObject
    {
        [field: UnityEngine.SerializeField] private List<GameplayTag> listOfTags = new List<GameplayTag>();

        public IReadOnlyList<GameplayTag> Items => listOfTags;

#if UNITY_EDITOR
        public void SetTemplates(List<GameplayTag> list)
        {
            listOfTags = list;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void AddTemplatesItem(List<GameplayTag> list)
        {
            foreach (var gameplayTag in list)
            {
                if (!listOfTags.Contains(gameplayTag))
                {
                    listOfTags.Add(gameplayTag);
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
    }
}
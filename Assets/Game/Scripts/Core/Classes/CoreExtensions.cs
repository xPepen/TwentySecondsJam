using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.Classes
{
    public static class CoreExtensions
    {
        public static bool IsA<T>(this GameObject obj)
        {
            if (obj.GetComponent<T>() != null)
            {
                return true;
            }

            return obj.GetComponentInChildren<T>() != null;
        }

        public static bool IsA<T>(this GameObject obj, out T outObj)
        {
            outObj = obj.GetComponent<T>();
            if (outObj != null)
            {
                return true;
            }

            outObj = obj.GetComponentInChildren<T>();
            return outObj != null;
        }

        public static bool HasTag(this GameObject obj, GameplayTag tag)
        {
            if (obj.IsA<Braviour>(out var braviour))
            {
                return braviour.HasTag(tag);
            }

            return false;
        }

        public static T GetInternalComponent<T>(this MonoBehaviour mono, GameObject obj) where T : UnityEngine.Component
        {
            T component = obj.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            component = obj.GetComponentInChildren<T>();

            if (component != null)
            {
                return component;
            }

            component = obj.GetComponentInParent<T>();
            if (component != null)
            {
                return component;
            }

            return null;
        }
        
        public static Texture2D LoadTexture(this MonoBehaviour mono, string filePath)
        {
            if (System.IO.File.Exists(Application.dataPath + filePath))
            {
                Texture2D newTexture = new Texture2D(2, 2);

                byte[] Data = System.IO.File.ReadAllBytes(Application.dataPath + filePath);
                newTexture.LoadImage(Data);
                return newTexture;
            }

            return null;
        }
    }
}
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.Classes
{
    public static class CoreExtensions
    {
        public static bool IsA<T>(this GameObject obj)
        {
            return obj.GetInternalComponent<T>() != null;
        }

        public static bool IsA<T>(this GameObject obj, out T outObj)
        {
            outObj = obj.GetInternalComponent<T>();
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

        public static T GetInternalComponent<T>(this GameObject obj)
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
            //at this point @component could be null
            return component;
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
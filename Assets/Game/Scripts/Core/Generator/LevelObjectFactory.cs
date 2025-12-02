using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Runtime.LightAndShadow;

namespace Game.Scripts.Core.Generator
{
    public static class LevelObjectFactory
    {
        public static GameObject CreateRoomObject(LevelGenConfig config, string name, Transform parent)
        {
            return CreateObjectCommon(config, name, parent);
        }

        public static GameObject CreateCorridorObject(LevelGenConfig config, string name, Transform parent)
        {
            return CreateObjectCommon(config, name, parent);
        }

        private static GameObject CreateObjectCommon(LevelGenConfig config, string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);

            List<Material> mats = new();
            List<Color> colors = new();

            // Order matters: 0=Floor, 1=Wall, 2=Roof
            string[] keys = { "floor", "wall", "roof" }; 

            foreach (var key in keys)
            {
                var data = config.ObjectData.Find(x => x.ObjectName.ToLower() == key);
                if (data != null)
                {
                    mats.Add(data.Material);
                    colors.Add(data.InitialColor);
                }
                else
                {
                    if(key == "roof" && mats.Count > 0) 
                         mats.Add(mats[0]); // Use floor material as fallback
                }
            }

            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
            obj.AddComponent<MeshCollider>();
            obj.AddComponent<EnviroObject>().Init(colors);

            return obj;
        }
    }
}
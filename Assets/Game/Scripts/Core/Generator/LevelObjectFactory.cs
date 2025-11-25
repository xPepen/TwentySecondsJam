using UnityEngine;
using Game.Scripts.Runtime.LightAndShadow;

namespace Game.Scripts.Core.Generator
{
    public static class LevelObjectFactory
    {
        public static GameObject CreateRoomObject( LevelGenConfig config, string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);

            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>().sharedMaterials =
                new Material[] { config.FloorMaterial, config.WallMaterial };
            obj.AddComponent<MeshCollider>();

            var enviro = obj.AddComponent<EnviroObject>();
            Subsystem.Get<LightAndShadowSubsystem>().Add(enviro);
            enviro.SetMultipleColor(config.InitialFloorColor, config.InitialWallColor);
            
            return obj;
        }

        public static GameObject CreateCorridorObject( LevelGenConfig config, string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);

            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>().sharedMaterials =
                new Material[] { config.FloorMaterial, config.WallMaterial };
            obj.AddComponent<MeshCollider>();

            var enviro = obj.AddComponent<EnviroObject>();
            Subsystem.Get<LightAndShadowSubsystem>().Add(enviro);

            return obj;
        }
    }
}
using System;
using System.Collections.Generic;
using Game.Scripts.Core.Classes;
using Unity.AI.Navigation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core.Generator
{
    public class Room
    {
        //X = minX , y = maxX, z = minY, w =maxY
        public Vector4 Boundary { get; private set; }

        public Rect Rect { get; private set; }
        public bool IsSingleDoorRoom { get; set; } = false;

        public GameObject RoomObject { get; set; }

        public NavMeshSurface NavMeshSurface { get; set; }

        public List<GameObject> Doors { get; private set; } = new List<GameObject>();
        public List<GameObject> RoomObjects { get; private set; } = new List<GameObject>();

        public Room(Rect rect)
        {
            this.Rect = rect;
        }


        public void AddNavMesh()
        {
            if (!NavMeshSurface)
            {
                NavMeshSurface = RoomObject.AddComponent<NavMeshSurface>();
                NavMeshSurface.center = RoomObject.transform.position;

                BuildNavMesh(CollectObjects.Children);
            }
        }

        public void BuildNavMesh(CollectObjects collectObjects)
        {
            if (NavMeshSurface)
            {
                NavMeshSurface.collectObjects = collectObjects;

                NavMeshSurface.BuildNavMesh();
            }
        }

        public void AddObject(GameObject prefab, Action<GameObject> onObjectAdded = null, Vector3 position = default,
            Quaternion rotation = default)
        {
            bool isRequestValid = IsInsideBoundary(position) && prefab;

            if (!isRequestValid)
            {
                return;
            }

            var obj = Object.Instantiate(prefab, position, rotation);
            RoomObjects.Add(obj);
            onObjectAdded?.Invoke(prefab);
            // if(obj.IsA<Braviour>())
        }
        

        public void AddObjectAtRandomPos(GameObject prefab, Action<GameObject> onObjectAdded = null)
        {
            if (NavMeshUtils.TryGetRandomPoint(Center3D, GetInnerRadius(), out var pos))
            {
                AddObject(prefab, onObjectAdded, pos);
                return;
            }

            Debug.Log("Can't find random position for " + this);
        }

        public void ClearObjectInRoom()
        {
            for (int i = RoomObjects.Count - 1; i >= 0; i--)
            {
                RoomObjects.RemoveAt(i);
            }

            RoomObjects.Clear();
        }


        public Vector4 GetBoundary()
        {
            //Vector4.zero mean not init yet.
            if (Boundary == Vector4.zero)
            {
                //note maybe this will change if room size can scale dynamically
                var center = Center;
                Boundary = new Vector4(center.x - Rect.width, center.x + Rect.width, center.y - Rect.height,
                    center.y + Rect.height);
            }


            return Boundary;
        }

        public bool IsInsideBoundary(Vector3 pos)
        {
            var c = Center;
            const int offSet = 10; //unit
            if (pos.y < 0 || pos.y > offSet)
            {
                return false;
            }

            var b = GetBoundary();
            return pos.x > b.x && pos.x < b.y && pos.z > b.z && pos.z < b.w;
        }

        public Vector2 Center => new Vector2(Rect.x + Rect.width / 2f, Rect.y + Rect.height / 2f);
        public Vector3 Center3D => new Vector3(Rect.x + Rect.width / 2f, 0, Rect.y + Rect.height / 2f);
        public float GetInnerRadius() => Mathf.Min(Rect.width, Rect.height) * 0.5f;
    }
}
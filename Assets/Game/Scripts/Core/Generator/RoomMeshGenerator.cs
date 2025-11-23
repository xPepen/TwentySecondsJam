using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class RoomMeshGenerator
    {
        private LevelGenConfig config;
        private GameObject container;

        public RoomMeshGenerator(LevelGenConfig config)
        {
            this.config = config;
            this.container = new GameObject("Rooms");
        }

        public void Generate(RoomGraph graph)
        {
            foreach (var room in graph.Rooms)
            {
                CreateRoomMesh(room, graph.Neighbors[room]);
            }
        }

        private void CreateRoomMesh(Room room, List<Room> neighbors)
        {
            GameObject roomObj = new GameObject($"Room_{room.Rect.center}");
            roomObj.transform.SetParent(container.transform);

            MeshFilter mf = roomObj.AddComponent<MeshFilter>();
            MeshRenderer mr = roomObj.AddComponent<MeshRenderer>();
            MeshCollider mc = roomObj.AddComponent<MeshCollider>();

            mr.sharedMaterials = new Material[] { config.FloorMaterial, config.WallMaterial };

            MeshData data = new MeshData();
            AddFloor(data, room.Rect);
            AddWalls(data, room, neighbors);

            Mesh mesh = new Mesh();
            mesh.vertices = data.Vertices.ToArray();
            mesh.uv = data.UVs.ToArray();
            mesh.subMeshCount = 2;
            mesh.SetTriangles(data.FloorTriangles.ToArray(), 0);
            mesh.SetTriangles(data.WallTriangles.ToArray(), 1);
            
            mesh.RecalculateNormals();
            
            mf.mesh = mesh;
            mc.sharedMesh = mesh;
        }

        private void AddFloor(MeshData data, Rect r)
        {
            Vector3 v0 = new Vector3(r.xMin, 0, r.yMin);
            Vector3 v1 = new Vector3(r.xMax, 0, r.yMin);
            Vector3 v2 = new Vector3(r.xMin, 0, r.yMax);
            Vector3 v3 = new Vector3(r.xMax, 0, r.yMax);

            int idx = data.Vertices.Count;
            data.Vertices.AddRange(new[] { v0, v1, v2, v3 });
            data.UVs.AddRange(new[] { new Vector2(v0.x, v0.z), new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z) });
            data.FloorTriangles.AddRange(new[] { idx, idx + 2, idx + 1, idx + 2, idx + 3, idx + 1 });
        }

        private void AddWalls(MeshData data, Room room, List<Room> neighbors)
        {
            Rect r = room.Rect;
            // Check logical neighbors to decide if we need a hole
            bool n = HasNeighbor(room, neighbors, Vector2.up);
            bool s = HasNeighbor(room, neighbors, Vector2.down);
            bool e = HasNeighbor(room, neighbors, Vector2.right);
            bool w = HasNeighbor(room, neighbors, Vector2.left);

            Vector3 bl = new Vector3(r.xMin, 0, r.yMin);
            Vector3 br = new Vector3(r.xMax, 0, r.yMin);
            Vector3 tl = new Vector3(r.xMin, 0, r.yMax);
            Vector3 tr = new Vector3(r.xMax, 0, r.yMax);

            BuildWall(data, tl, tr, n); // North
            BuildWall(data, br, bl, s); // South
            BuildWall(data, tr, br, e); // East
            BuildWall(data, bl, tl, w); // West
        }

        private void BuildWall(MeshData data, Vector3 start, Vector3 end, bool hasDoor)
        {
            if (!hasDoor)
            {
                AddQuad(data, start, end);
            }
            else
            {
                Vector3 mid = (start + end) * 0.5f;
                Vector3 dir = (end - start).normalized;
                float halfDoor = config.DoorWidth * 0.5f;

                AddQuad(data, start, mid - dir * halfDoor);
                AddQuad(data, mid + dir * halfDoor, end);
            }
        }

        private void AddQuad(MeshData data, Vector3 start, Vector3 end)
        {
            float h = config.WallHeight;
            int idx = data.Vertices.Count;
            
            data.Vertices.Add(start);
            data.Vertices.Add(end);
            data.Vertices.Add(start + Vector3.up * h);
            data.Vertices.Add(end + Vector3.up * h);

            float len = Vector3.Distance(start, end);
            data.UVs.Add(new Vector2(0, 0));
            data.UVs.Add(new Vector2(len, 0));
            data.UVs.Add(new Vector2(0, h));
            data.UVs.Add(new Vector2(len, h));

            data.WallTriangles.AddRange(new[] { idx, idx + 2, idx + 1, idx + 2, idx + 3, idx + 1 });
        }

        private bool HasNeighbor(Room r, List<Room> neighbors, Vector2 dir)
        {
            foreach (var n in neighbors)
            {
                if (Vector2.Dot((n.Center - r.Center).normalized, dir) > 0.9f) return true;
            }
            return false;
        }

        private class MeshData
        {
            public List<Vector3> Vertices = new();
            public List<Vector2> UVs = new();
            public List<int> FloorTriangles = new();
            public List<int> WallTriangles = new();
        }
    }
}